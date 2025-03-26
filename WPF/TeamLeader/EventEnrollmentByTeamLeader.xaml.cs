using BLL.BusinessInterfaces;
using Microsoft.Extensions.DependencyInjection;
using Model.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace WPF.TeamLeader
{
    /// <summary>
    /// Interaction logic for EventEnrollment.xaml
    /// </summary>
    public partial class EventEnrollmentByTeamLeader : UserControl
    {
        private readonly IEventService _eventService;
        private readonly IEventParticipantService _eventParticipantService;
        private List<Event> _allEvents;
        private Dictionary<int, int> _eventParticipantCounts = new Dictionary<int, int>();
        private Dictionary<int, bool> _eventEnrollmentStatus = new Dictionary<int, bool>();

        public EventEnrollmentByTeamLeader()
        {
            InitializeComponent();
            _eventService = ((App)Application.Current).ServiceProvider.GetRequiredService<IEventService>();
            _eventParticipantService = ((App)Application.Current).ServiceProvider.GetRequiredService<IEventParticipantService>();
            LoadEvents();
        }

        private void LoadEvents()
        {
            try
            {
                if (User.Current == null)
                {
                    MessageBox.Show("You are not logged in.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                    return;
                }

                // Get the current user's club ID
                int? clubId = User.Current.ClubId;

                if (clubId == null)
                {
                    MessageBox.Show("You are not associated with any club.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                    return;
                }

                // Get only events from the user's club
                _allEvents = _eventService.GetAllEvents()
                    .Where(e => e.ClubId == clubId)
                    .ToList();

                // Get user's participations
                var userParticipations = _eventParticipantService.GetEventParticipantsByUser(User.Current.UserId);

                // Add participation status to events
                foreach (var evt in _allEvents)
                {
                    // Count participants for this event
                    var participants = _eventParticipantService.GetEventParticipantsByEvent(evt.EventId);
                    _eventParticipantCounts[evt.EventId] = participants.Count;

                    // Check if user is already participating
                    var isParticipating = userParticipations.Any(p => p.EventId == evt.EventId);

                    // Determine if user can enroll
                    // User can enroll if:
                    // 1. Event is not full (enrolled < capacity)
                    // 2. Event status is "Upcoming" or "Ongoing"
                    // 3. User is not already participating
                    bool isFull = _eventParticipantCounts[evt.EventId] >= evt.Capacity;
                    bool isActive = evt.Status == "Upcoming" || evt.Status == "Ongoing";

                    _eventEnrollmentStatus[evt.EventId] = !isParticipating && !isFull && isActive;
                }

                // Apply initial filter (All Events)
                ApplyFilters();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error loading events: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void ApplyFilters()
        {
            if (_allEvents == null) return;

            var filteredEvents = _allEvents;

            // Apply status filter
            string statusFilter = ((ComboBoxItem)cmbStatus.SelectedItem)?.Content.ToString();
            if (statusFilter != "All Events" && !string.IsNullOrEmpty(statusFilter))
            {
                filteredEvents = filteredEvents.Where(e => e.Status == statusFilter).ToList();
            }

            // Apply search text filter
            if (!string.IsNullOrWhiteSpace(txtSearch.Text))
            {
                string searchText = txtSearch.Text.ToLower();
                filteredEvents = filteredEvents.Where(e =>
                    e.EventName.ToLower().Contains(searchText) ||
                    (e.Location != null && e.Location.ToLower().Contains(searchText))).ToList();
            }

            // Create view models with participant count and enrollment status
            var eventViewModels = filteredEvents.Select(e => new
            {
                Event = e,
                ParticipantCount = _eventParticipantCounts.ContainsKey(e.EventId) ? _eventParticipantCounts[e.EventId] : 0,
                CanEnroll = _eventEnrollmentStatus.ContainsKey(e.EventId) ? _eventEnrollmentStatus[e.EventId] : false
            }).ToList();

            dgEvents.ItemsSource = eventViewModels;
        }

        private void btnSearch_Click(object sender, RoutedEventArgs e)
        {
            ApplyFilters();
        }

        private void txtSearch_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                ApplyFilters();
            }
        }

        private void cmbStatus_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            ApplyFilters();
        }

        private void dgEvents_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            var selectedItem = dgEvents.SelectedItem as dynamic;
            if (selectedItem != null)
            {
                var selectedEvent = selectedItem.Event as Event;
                txtEventName.Text = selectedEvent.EventName;
                txtClub.Text = selectedEvent.Club?.ClubName ?? "N/A";
                txtDate.Text = selectedEvent.EventDate.ToString("dd/MM/yyyy");
                txtLocation.Text = selectedEvent.Location ?? "N/A";
                txtDescription.Text = selectedEvent.Description ?? "No description available.";
            }
            else
            {
                ClearEventDetails();
            }
        }

        private void ClearEventDetails()
        {
            txtEventName.Text = string.Empty;
            txtClub.Text = string.Empty;
            txtDate.Text = string.Empty;
            txtLocation.Text = string.Empty;
            txtDescription.Text = string.Empty;
        }

        private void btnEnroll_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                // Get the event from the button's DataContext
                var button = sender as Button;
                var viewModel = button.DataContext as dynamic;
                var selectedEvent = viewModel.Event as Event;

                if (selectedEvent == null || User.Current == null)
                {
                    MessageBox.Show("Unable to enroll. Please try again.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                    return;
                }

                // Confirm enrollment
                var result = MessageBox.Show($"Are you sure you want to enroll in the event '{selectedEvent.EventName}'?",
                    "Confirm Enrollment", MessageBoxButton.YesNo, MessageBoxImage.Question);

                if (result == MessageBoxResult.Yes)
                {
                    // Create new event participant
                    var participant = new EventParticipant
                    {
                        EventId = selectedEvent.EventId,
                        UserId = User.Current.UserId,
                        RegistrationDate = DateTime.Now,
                        Status = "Registered"
                    };

                    // Add participant
                    _eventParticipantService.AddEventParticipant(participant);

                    MessageBox.Show($"You have successfully enrolled in the event '{selectedEvent.EventName}'.",
                        "Enrollment Successful", MessageBoxButton.OK, MessageBoxImage.Information);

                    // Refresh events to update enrollment status
                    LoadEvents();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error enrolling in event: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }
    }
}
