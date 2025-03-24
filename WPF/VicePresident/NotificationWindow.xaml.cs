using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using BLL.BusinessInterfaces;
using Microsoft.Extensions.DependencyInjection;
using Model.Models;

namespace WPF
{
    public partial class NotificationWindow : UserControl
    {
        private readonly IEventService _eventService;
        private Event _selectedEvent;
        private List<Event> _allEvents = new List<Event>();

        public event EventHandler<NotificationEventArgs> SendClicked;
        public event EventHandler CancelClicked;

        public NotificationWindow(string defaultEventName)
        {
            InitializeComponent();
            var serviceProvider = ((App)Application.Current).ServiceProvider;
            _eventService = serviceProvider.GetRequiredService<IEventService>();
            txtSubject.Text = $"Notification about: {defaultEventName}";
            rbAllParticipants.IsChecked = true;
            LoadEvents();
            if (defaultEventName != "All Events")
            {
                SelectEventByName(defaultEventName);
            }
        }

        private void LoadEvents()
        {
            try
            {
                _allEvents = _eventService.GetAllEvents().ToList();
                dgEvents.ItemsSource = _allEvents;
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error loading events: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void dgEvents_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            var selectedEvent = dgEvents.SelectedItem as Event;
            if (selectedEvent != null)
            {
                _selectedEvent = selectedEvent;
                UpdateEventDetails();
                txtSubject.Text = $"Notification about: {_selectedEvent.EventName}";
            }
        }

        private void UpdateEventDetails()
        {
            if (_selectedEvent != null)
            {
                txtEventTitle.Text = $"Selected Event: {_selectedEvent.EventName}";
                txtEventDate.Text = _selectedEvent.EventDate.ToString("d");
                txtEventLocation.Text = _selectedEvent.Location;
                txtEventStatus.Text = _selectedEvent.Status;
            }
            else
            {
                txtEventTitle.Text = "Selected Event: None";
                txtEventDate.Text = "";
                txtEventLocation.Text = "";
                txtEventStatus.Text = "";
            }
        }

        private void txtEventSearch_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                SearchEvents();
            }
        }

        private void btnEventSearch_Click(object sender, RoutedEventArgs e)
        {
            SearchEvents();
        }

        private void SearchEvents()
        {
            string searchTerm = txtEventSearch.Text.Trim().ToLower();
            if (string.IsNullOrEmpty(searchTerm))
            {
                dgEvents.ItemsSource = _allEvents;
            }
            else
            {
                var filteredEvents = _allEvents.Where(e => 
                    e.EventName.ToLower().Contains(searchTerm) || 
                    e.Description.ToLower().Contains(searchTerm) ||
                    e.Location.ToLower().Contains(searchTerm)).ToList();
                dgEvents.ItemsSource = filteredEvents;
            }
        }

        private void btnSend_Click(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrWhiteSpace(txtSubject.Text))
            {
                MessageBox.Show("Please enter a subject for the notification.", "Missing Subject", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            if (string.IsNullOrWhiteSpace(txtMessage.Text))
            {
                MessageBox.Show("Please enter a message for the notification.", "Missing Message", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }
            
            if (_selectedEvent == null)
            {
                MessageBox.Show("Please select an event to send notification for.", "No Event Selected", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            string recipientType;
            if (rbAllParticipants.IsChecked == true)
                recipientType = "All Participants";
            else if (rbRegisteredOnly.IsChecked == true)
                recipientType = "Registered Participants";
            else if (rbAttendedOnly.IsChecked == true)
                recipientType = "Attended Participants";
            else if (rbAbsentOnly.IsChecked == true)
                recipientType = "Absent Participants";
            else
                recipientType = "All Participants";
            
            string subject = txtSubject.Text;
            string message = txtMessage.Text;
            SendClicked?.Invoke(this, new NotificationEventArgs(subject, message, recipientType, _selectedEvent));
        }

        private void btnCancel_Click(object sender, RoutedEventArgs e)
        {
            CancelClicked?.Invoke(this, EventArgs.Empty);
        }
        
        private void SelectEventByName(string eventName)
        {
            var eventToSelect = _allEvents.FirstOrDefault(e => e.EventName == eventName);
            if (eventToSelect != null)
            {
                dgEvents.SelectedItem = eventToSelect;
                _selectedEvent = eventToSelect;
                UpdateEventDetails();
            }
        }
    }
    
    public class NotificationEventArgs : EventArgs
    {
        public string Subject { get; }
        public string Message { get; }
        public string RecipientType { get; }
        public Event TargetEvent { get; }
        
        public NotificationEventArgs(string subject, string message, string recipientType, Event targetEvent = null)
        {
            Subject = subject;
            Message = message;
            RecipientType = recipientType;
            TargetEvent = targetEvent;
        }
    }
}
