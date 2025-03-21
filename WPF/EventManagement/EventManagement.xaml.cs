using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using BLL.BusinessInterfaces;
using BLL.BusinessService;
using BLL.Repositories;
using DAL.Interfaces;
using DAL.Repositories;
using Microsoft.Extensions.DependencyInjection;
using Model.Contexts;
using Model.Models;

namespace WPF
{
    public partial class EventManagement : Window
    {
        private IEventService _eventService;
        private IAccountService _accountService;
        private IEventParticipantService _eventParticipantService;

        public EventManagement()
        {
            InitializeComponent();
            _eventService = ((App)Application.Current).ServiceProvider.GetRequiredService<IEventService>()
               ?? throw new ArgumentNullException(nameof(EventService));
            _accountService = ((App)Application.Current).ServiceProvider.GetRequiredService<IAccountService>()
               ?? throw new ArgumentNullException(nameof(AccountService));
            _eventParticipantService = ((App)Application.Current).ServiceProvider.GetRequiredService<IEventParticipantService>()
               ?? throw new ArgumentNullException(nameof(EventParticipantService));
            LoadEvents();
        }

        private void LoadEvents()
        {
            var events = _eventService.GetAllEvents().ToList();
            dgEvents.ItemsSource = events;
        }

        private void LoadEventParticipants()
        {
            var selectedEvent = dgEvents.SelectedItem as Event;
            if (selectedEvent != null)
            {
                var eventParticipants = _eventParticipantService.GetEventParticipantsByEvent(selectedEvent.EventId);
                dgParticipants.ItemsSource = eventParticipants;
                UpdateParticipantStats(eventParticipants);
            }
        }

        private void LoadAvailableMembers()
        {
            var selectedEvent = dgEvents.SelectedItem as Event;
            if (selectedEvent != null)
            {
                var eventParticipants = dgParticipants.ItemsSource as List<EventParticipant>;
                if (eventParticipants != null)
                {
                    var allUsers = _accountService.GetAll();
                    var currentParticipantIds = eventParticipants.Select(p => p.UserId).ToList();
                    var availableMembers = allUsers.Where(u => !currentParticipantIds.Contains(u.UserId)).ToList();
                    cmbAvailableMembers.ItemsSource = availableMembers;
                }
            }
        }

        private void UpdateParticipantStats(List<EventParticipant> eventParticipants)
        {
            if (eventParticipants != null)
            {
                txtRegisteredCount.Text = eventParticipants.Count(p => p.Status == "Registered").ToString();
                txtAttendedCount.Text = eventParticipants.Count(p => p.Status == "Attended").ToString();
                txtAbsentCount.Text = eventParticipants.Count(p => p.Status == "Absent").ToString();
                txtTotalCount.Text = eventParticipants.Count.ToString();
            }
        }

        private void UpdateEventDetails()
        {
            var selectedEvent = dgEvents.SelectedItem as Event;
            if (selectedEvent != null)
            {
                txtEventName.Text = selectedEvent.EventName;
                txtEventDate.Text = selectedEvent.EventDate.ToString("d");
                txtEventStatus.Text = selectedEvent.Status;
                borderParticipants.Visibility = Visibility.Visible;
            }
            else
            {
                borderParticipants.Visibility = Visibility.Collapsed;
            }
        }

        private void btnSearch_Click(object sender, RoutedEventArgs e)
        {
            string searchText = txtSearch.Text.Trim().ToLower();
            var events = _eventService.GetAllEvents().ToList();
            
            if (string.IsNullOrEmpty(searchText))
            {
                dgEvents.ItemsSource = events;
            }
            else
            {
                dgEvents.ItemsSource = events.Where(ev => 
                    ev.EventName.ToLower().Contains(searchText) ||
                    ev.Description.ToLower().Contains(searchText) ||
                    ev.Location.ToLower().Contains(searchText) ||
                    ev.Status.ToLower().Contains(searchText)
                ).ToList();
            }
        }

        private void dgEvents_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            UpdateEventDetails();
            LoadEventParticipants();
            LoadAvailableMembers();
        }

        private void btnAddEvent_Click(object sender, RoutedEventArgs e)
        {
            var eventEditWindow = new EventEditWindow(null);
            if (eventEditWindow.ShowDialog() == true)
            {
                LoadEvents();
            }
        }

        private void btnEditEvent_Click(object sender, RoutedEventArgs e)
        {
            var selectedEvent = dgEvents.SelectedItem as Event;
            if (selectedEvent != null)
            {
                var eventEditWindow = new EventEditWindow(selectedEvent);
                if (eventEditWindow.ShowDialog() == true)
                {
                    LoadEvents();
                }
            }
            else
            {
                MessageBox.Show("Please select an event to edit.", "No Event Selected", MessageBoxButton.OK, MessageBoxImage.Information);
            }
        }

        private void btnDeleteEvent_Click(object sender, RoutedEventArgs e)
        {
            var selectedEvent = dgEvents.SelectedItem as Event;
            if (selectedEvent != null)
            {
                var result = MessageBox.Show($"Are you sure you want to delete the event '{selectedEvent.EventName}'?", 
                    "Confirm Delete", MessageBoxButton.YesNo, MessageBoxImage.Question);
                
                if (result == MessageBoxResult.Yes)
                {
                    try
                    {
                        _eventService.DeleteEvent(selectedEvent.EventId);
                        LoadEvents();
                        UpdateEventDetails();
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show($"Error deleting event: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                    }
                }
            }
            else
            {
                MessageBox.Show("Please select an event to delete.", "No Event Selected", MessageBoxButton.OK, MessageBoxImage.Information);
            }
        }

        private void btnViewParticipants_Click(object sender, RoutedEventArgs e)
        {
            var selectedEvent = dgEvents.SelectedItem as Event;
            if (selectedEvent != null)
            {
                var eventParticipants = dgParticipants.ItemsSource as List<EventParticipant>;
                if (eventParticipants != null)
                {
                    var participants = new ObservableCollection<EventParticipant>(eventParticipants);
                    var participantWindow = new ParticipantWindow(selectedEvent, _eventParticipantService, participants);
                    participantWindow.ShowDialog();
                    LoadEventParticipants(); // Refresh after returning
                }
            }
            else
            {
                MessageBox.Show("Please select an event to view participants.", "No Event Selected", MessageBoxButton.OK, MessageBoxImage.Information);
            }
        }

        private void btnSendNotification_Click(object sender, RoutedEventArgs e)
        {
            var selectedEvent = dgEvents.SelectedItem as Event;
            if (selectedEvent != null)
            {
                var notificationWindow = new NotificationWindow(selectedEvent.EventName);
                notificationWindow.ShowDialog();
            }
            else
            {
                MessageBox.Show("Please select an event to send notifications.", "No Event Selected", MessageBoxButton.OK, MessageBoxImage.Information);
            }
        }

        private void ChangeEventStatus(string newStatus)
        {
            var selectedEvent = dgEvents.SelectedItem as Event;
            if (selectedEvent != null)
            {
                try
                {
                    selectedEvent.Status = newStatus;
                    _eventService.UpdateEvent(selectedEvent);
                    LoadEvents();
                    MessageBox.Show($"Event status changed to {newStatus}.", "Status Updated", MessageBoxButton.OK, MessageBoxImage.Information);
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Error changing event status: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
            else
            {
                MessageBox.Show("Please select an event to change its status.", "No Event Selected", MessageBoxButton.OK, MessageBoxImage.Information);
            }
        }

        private void btnMarkComplete_Click(object sender, RoutedEventArgs e)
        {
            ChangeEventStatus("Completed");
        }

        private void btnMarkCancelled_Click(object sender, RoutedEventArgs e)
        {
            ChangeEventStatus("Cancelled");
        }

        private void btnStatusUpcoming_Click(object sender, RoutedEventArgs e)
        {
            ChangeEventStatus("Upcoming");
        }

        private void btnStatusOngoing_Click(object sender, RoutedEventArgs e)
        {
            ChangeEventStatus("Ongoing");
        }

        private void btnStatusCompleted_Click(object sender, RoutedEventArgs e)
        {
            ChangeEventStatus("Completed");
        }

        private void btnStatusCancelled_Click(object sender, RoutedEventArgs e)
        {
            ChangeEventStatus("Cancelled");
        }

        private void btnAddParticipant_Click(object sender, RoutedEventArgs e)
        {
            var selectedEvent = dgEvents.SelectedItem as Event;
            var selectedMember = cmbAvailableMembers.SelectedItem as User;
            
            if (selectedEvent != null && selectedMember != null)
            {
                try
                {
                    var participant = new EventParticipant
                    {
                        EventId = selectedEvent.EventId,
                        UserId = selectedMember.UserId,
                        Status = "Registered",
                        RegistrationDate = DateTime.Now
                    };
                    
                    _eventParticipantService.AddEventParticipant(participant);
                    LoadEventParticipants();
                    LoadAvailableMembers();
                    MessageBox.Show($"{selectedMember.FullName} has been added to the event.", "Participant Added", MessageBoxButton.OK, MessageBoxImage.Information);
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Error adding participant: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
            else
            {
                MessageBox.Show("Please select both an event and a member to add.", "Selection Required", MessageBoxButton.OK, MessageBoxImage.Information);
            }
        }
    }
}
