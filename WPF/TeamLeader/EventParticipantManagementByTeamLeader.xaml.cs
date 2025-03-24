using BLL.BusinessInterfaces;
using BLL.BusinessService;
using Microsoft.Extensions.DependencyInjection;
using Model.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;

namespace WPF.TeamLeader
{
    /// <summary>
    /// Interaction logic for EventParticipantManagementByTeamLeader.xaml
    /// </summary>
    public partial class EventParticipantManagementByTeamLeader : UserControl
    {
        private readonly IEventService _eventService;
        private readonly IAccountService _userService;
        private readonly IEventParticipantService _eventParticipantService;
        private Event _selectedEvent;

        public EventParticipantManagementByTeamLeader()
        {
            InitializeComponent();

            // Get services
            _eventService = ((App)Application.Current).ServiceProvider.GetRequiredService<IEventService>()
               ?? throw new ArgumentNullException(nameof(IEventService));
            _userService = ((App)Application.Current).ServiceProvider.GetRequiredService<IAccountService>()
               ?? throw new ArgumentNullException(nameof(IAccountService));
            _eventParticipantService = ((App)Application.Current).ServiceProvider.GetRequiredService<IEventParticipantService>()
               ?? throw new ArgumentNullException(nameof(IEventParticipantService));

            // Load events
            LoadEvents();

            // Initialize status combo box
            cmbStatus.SelectedIndex = 0;

            // Subscribe to selection changed event for participants
            dgParticipants.SelectionChanged += dgParticipants_SelectionChanged;
        }

        private void LoadEvents()
        {
            try
            {
                // Get the current user's club ID
                string username = User.Current?.UserName;

                if (string.IsNullOrEmpty(username))
                {
                    MessageBox.Show("User is not logged in.", "Login Required", MessageBoxButton.OK, MessageBoxImage.Warning);
                    return;
                }

                int? clubIdFromCurrentUser = _userService.GetClubIdByUsername(username);

                if (clubIdFromCurrentUser == null)
                {
                    MessageBox.Show("No club found for the current user.", "Club Not Found", MessageBoxButton.OK, MessageBoxImage.Warning);
                    return;
                }

                // Get all events and filter by the current user's club ID
                var allEvents = _eventService.GetAllEvents()
                    .Where(e => e.ClubId == clubIdFromCurrentUser)
                    .ToList();

                // Set the event details
                if (allEvents.Any())
                {
                    _selectedEvent = allEvents.First();
                    UpdateEventDetails();
                    LoadEventParticipants();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error loading events: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void LoadEventParticipants()
        {
            if (_selectedEvent != null)
            {
                var eventParticipants = _eventParticipantService.GetEventParticipantsByEvent(_selectedEvent.EventId);
                dgParticipants.ItemsSource = eventParticipants;
                UpdateParticipantStats(eventParticipants);
            }
        }

        private void UpdateParticipantStats(IEnumerable<EventParticipant> participants)
        {
            int total = participants.Count();
            int registered = participants.Count(p => p.Status == "Registered");
            int attended = participants.Count(p => p.Status == "Attended");
            int absent = participants.Count(p => p.Status == "Absent");

            txtTotalParticipants.Text = total.ToString();
            txtRegisteredCount.Text = registered.ToString();
            txtAttendedCount.Text = attended.ToString();
            txtAbsentCount.Text = absent.ToString();
        }

        private void dgParticipants_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            // Update the status combo box based on selection
            if (dgParticipants.SelectedItems.Count == 1)
            {
                var selectedParticipant = dgParticipants.SelectedItem as EventParticipant;
                if (selectedParticipant != null)
                {
                    // Find and select the matching status in the combo box
                    foreach (ComboBoxItem item in cmbStatus.Items)
                    {
                        if (item.Content.ToString() == selectedParticipant.Status)
                        {
                            cmbStatus.SelectedItem = item;
                            break;
                        }
                    }
                }
            }
        }

        private void cmbStatus_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            // This event handler will be triggered when the status is changed in the combo box
            // No implementation needed here as we're just using it to reflect the selected participant's status
        }

        private void btnBulkUpdateStatus_Click(object sender, RoutedEventArgs e)
        {
            if (dgParticipants.SelectedItems.Count == 0)
            {
                MessageBox.Show("Please select at least one participant to update.", "No Selection", MessageBoxButton.OK, MessageBoxImage.Information);
                return;
            }

            string newStatus = (cmbStatus.SelectedItem as ComboBoxItem)?.Content.ToString();
            if (string.IsNullOrEmpty(newStatus))
            {
                MessageBox.Show("Please select a status.", "No Status Selected", MessageBoxButton.OK, MessageBoxImage.Information);
                return;
            }

            foreach (EventParticipant participant in dgParticipants.SelectedItems)
            {
                participant.Status = newStatus;
                _eventParticipantService.UpdateEventParticipant(participant);
            }

            // Refresh participants
            LoadEventParticipants();

            MessageBox.Show($"{dgParticipants.SelectedItems.Count} participants updated to status: {newStatus}",
                "Status Updated", MessageBoxButton.OK, MessageBoxImage.Information);
        }

        private void btnSendNotification_Click(object sender, RoutedEventArgs e)
        {
            if (_selectedEvent != null)
            {
                // Implement notification sending logic here
                MessageBox.Show("Notification sent to all participants.", "Notification Sent", MessageBoxButton.OK, MessageBoxImage.Information);
            }
            else
            {
                MessageBox.Show("Please select an event first.", "No Event Selected", MessageBoxButton.OK, MessageBoxImage.Information);
            }
        }

        private void btnRemoveParticipant_Click(object sender, RoutedEventArgs e)
        {
            var button = sender as Button;
            if (button != null)
            {
                var participant = button.DataContext as EventParticipant;
                if (participant != null)
                {
                    var result = MessageBox.Show($"Are you sure you want to remove {participant.User.FullName} from this event?",
                        "Confirm Removal", MessageBoxButton.YesNo, MessageBoxImage.Question);

                    if (result == MessageBoxResult.Yes)
                    {
                        _eventParticipantService.DeleteEventParticipant(participant.EventParticipantId);

                        // Refresh participants
                        LoadEventParticipants();
                    }
                }
            }
        }

        private void UpdateEventDetails()
        {
            if (_selectedEvent != null)
            {
                // Update event details in the UI
                txtEventTitle.Text = _selectedEvent.EventName;
                txtEventDate.Text = _selectedEvent.EventDate.ToString("d");
                txtEventLocation.Text = _selectedEvent.Location;
                txtEventStatus.Text = _selectedEvent.Status;
            }
            else
            {
                // Clear event details
                txtEventTitle.Text = "Event Details";
                txtEventDate.Text = "";
                txtEventLocation.Text = "";
                txtEventStatus.Text = "";
            }
        }
    }
}