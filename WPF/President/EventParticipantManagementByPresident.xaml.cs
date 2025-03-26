using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using BLL.BusinessInterfaces;
using Microsoft.Extensions.DependencyInjection;
using Model.Models;

namespace WPF.President
{
    public partial class EventParticipantManagementByPresident : UserControl
    {
        private readonly IEventService _eventService;
        private readonly IAccountService _userService;
        private readonly IEventParticipantService _eventParticipantService;
        private Event _selectedEvent;
        private Window _hostWindow;
        private List<User> _availableMembers = new List<User>();
        private List<Event> _allEvents = new List<Event>();

        // Event to notify when an event is selected
        public event EventHandler<Event> EventSelected;

        public EventParticipantManagementByPresident()
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

        // Constructor that accepts a specific event to display
        public EventParticipantManagementByPresident(Event eventToDisplay)
        {
            InitializeComponent();

            // Get services
            _eventService = ((App)Application.Current).ServiceProvider.GetRequiredService<IEventService>()
               ?? throw new ArgumentNullException(nameof(IEventService));
            _userService = ((App)Application.Current).ServiceProvider.GetRequiredService<IAccountService>()
               ?? throw new ArgumentNullException(nameof(IAccountService));
            _eventParticipantService = ((App)Application.Current).ServiceProvider.GetRequiredService<IEventParticipantService>()
               ?? throw new ArgumentNullException(nameof(IEventParticipantService));

            // Load all events first
            LoadEvents();
            
            // Initialize status combo box
            cmbStatus.SelectedIndex = 0;

            // Subscribe to selection changed event for participants
            dgParticipants.SelectionChanged += dgParticipants_SelectionChanged;

            // Select the specified event
            if (eventToDisplay != null)
            {
                _selectedEvent = eventToDisplay;
                
                // Select the event in the DataGrid
                foreach (var evt in dgEvents.Items)
                {
                    if (evt is Event e && e.EventId == eventToDisplay.EventId)
                    {
                        dgEvents.SelectedItem = evt;
                        break;
                    }
                }
                
                LoadEventParticipants();
                UpdateEventDetails();
                LoadAvailableMembers();
            }
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
                _allEvents = _eventService.GetAllEvents()
                    .Where(e => e.ClubId == clubIdFromCurrentUser)
                    .ToList();
                
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

                // Notify parent control if needed
                EventSelected?.Invoke(this, selectedEvent);

                // Load participants for the selected event
                LoadEventParticipants();

                // Update event details
                UpdateEventDetails();

                // Load available members
                LoadAvailableMembers();
            }
        }

        private void LoadEventParticipants()
        {
            if (_selectedEvent != null)
            {
                var eventParticipants = _eventParticipantService.GetEventParticipantsByEvent(_selectedEvent.EventId);
                
                // Apply status filter if selected
                if (cmbStatusFilter != null && cmbStatusFilter.SelectedItem != null)
                {
                    var selectedStatus = ((ComboBoxItem)cmbStatusFilter.SelectedItem).Content.ToString();
                    if (selectedStatus != "All Statuses")
                    {
                        eventParticipants = eventParticipants.Where(p => p.Status == selectedStatus).ToList();
                    }
                }
                
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

        private void cmbStatusFilter_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            LoadEventParticipants();
        }

        private void txtEventSearch_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                btnEventSearch_Click(sender, e);
            }
        }

        private void btnEventSearch_Click(object sender, RoutedEventArgs e)
        {
            string searchTerm = txtEventSearch.Text.Trim().ToLower();

            try
            {
                if (string.IsNullOrWhiteSpace(searchTerm))
                {
                    // If search is empty, show all events
                    dgEvents.ItemsSource = _allEvents;
                    return;
                }

                // Filter events by name
                var filteredEvents = _allEvents
                    .Where(ev => ev.EventName.ToLower().Contains(searchTerm))
                    .ToList();

                // Update the DataGrid
                dgEvents.ItemsSource = filteredEvents;

                if (filteredEvents.Count == 0)
                {
                    MessageBox.Show("No events found matching your search.", "No Results", MessageBoxButton.OK, MessageBoxImage.Information);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error searching events: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
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
                // Find the parent Main_VicePresident_WPF window
                var mainWindow = Window.GetWindow(this) as Main_VicePresident_WPF;
                if (mainWindow != null)
                {
                    // Navigate to the notification tab with the selected event
                    mainWindow.NavigateToNotificationTab(_selectedEvent.EventName);
                }
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

        private void LoadAvailableMembers()
        {
            if (_selectedEvent == null)
            {
                lbAvailableMembers.ItemsSource = null;
                return;
            }

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

                // Get all participants for this event
                var participants = _eventParticipantService.GetEventParticipantsByEvent(_selectedEvent.EventId);
                var participantUserIds = participants.Select(p => p.UserId).ToList();

                // Get all users who are not participants AND are in the same club as the current user
                _availableMembers = _userService.GetAll()
                    .Where(u => !participantUserIds.Contains(u.UserId) && u.ClubId == clubIdFromCurrentUser)
                    .OrderBy(u => u.FullName)
                    .ToList();

                // Update the list box
                lbAvailableMembers.ItemsSource = _availableMembers;
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error loading available members: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void txtSearch_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                btnSearchMembers_Click(sender, e);
            }
        }

        private void btnSearchMembers_Click(object sender, RoutedEventArgs e)
        {
            if (_selectedEvent == null)
            {
                MessageBox.Show("Please select an event first.", "No Event Selected", MessageBoxButton.OK, MessageBoxImage.Information);
                return;
            }

            string searchTerm = txtSearch.Text.Trim().ToLower();

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

                if (string.IsNullOrWhiteSpace(searchTerm))
                {
                    // If search is empty, show all available members
                    LoadAvailableMembers();
                    return;
                }

                // Get all participants for this event
                var participants = _eventParticipantService.GetEventParticipantsByEvent(_selectedEvent.EventId);
                var participantUserIds = participants.Select(p => p.UserId).ToList();

                // Filter available members by search term AND club ID
                var filteredMembers = _userService.GetAll()
                    .Where(u => !participantUserIds.Contains(u.UserId) && 
                           u.ClubId == clubIdFromCurrentUser &&
                           (u.FullName.ToLower().Contains(searchTerm) ||
                           (u.Email != null && u.Email.ToLower().Contains(searchTerm))))
                    .OrderBy(u => u.FullName)
                    .ToList();

                // Update the list box
                lbAvailableMembers.ItemsSource = filteredMembers;

                if (filteredMembers.Count == 0)
                {
                    MessageBox.Show("No members found matching your search.", "No Results", MessageBoxButton.OK, MessageBoxImage.Information);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error searching members: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void btnAddSpecificParticipant_Click(object sender, RoutedEventArgs e)
        {
            if (_selectedEvent == null)
            {
                MessageBox.Show("Please select an event first.", "No Event Selected", MessageBoxButton.OK, MessageBoxImage.Information);
                return;
            }

            var button = sender as Button;
            if (button != null)
            {
                var user = button.DataContext as User;
                if (user != null)
                {
                    AddParticipantToEvent(user);
                }
            }
        }

        private void AddParticipantToEvent(User user)
        {
            try
            {
                // Check if user is already a participant
                var existingParticipants = _eventParticipantService.GetEventParticipantsByEvent(_selectedEvent.EventId);
                if (existingParticipants.Any(p => p.UserId == user.UserId))
                {
                    MessageBox.Show($"{user.FullName} is already a participant in this event.",
                        "Already Registered", MessageBoxButton.OK, MessageBoxImage.Information);
                    return;
                }

                // Create new participant
                var newParticipant = new EventParticipant
                {
                    EventId = _selectedEvent.EventId,
                    UserId = user.UserId,
                    Status = "Registered",
                    RegistrationDate = DateTime.Now
                };

                _eventParticipantService.AddEventParticipant(newParticipant);

                // Refresh participants
                LoadEventParticipants();

                MessageBox.Show($"{user.FullName} has been added to the event.",
                    "Participant Added", MessageBoxButton.OK, MessageBoxImage.Information);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error adding participant: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void lbAvailableMembers_MouseDoubleClick(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            if (_selectedEvent == null)
            {
                MessageBox.Show("Please select an event first.", "No Event Selected", MessageBoxButton.OK, MessageBoxImage.Information);
                return;
            }
            
            var selectedUser = lbAvailableMembers.SelectedItem as User;
            if (selectedUser != null)
            {
                AddParticipantToEvent(selectedUser);
            }
        }

        private void btnAddSelectedMember_Click(object sender, RoutedEventArgs e)
        {
            if (_selectedEvent == null)
            {
                MessageBox.Show("Please select an event first.", "No Event Selected", MessageBoxButton.OK, MessageBoxImage.Information);
                return;
            }
            
            var selectedUser = lbAvailableMembers.SelectedItem as User;
            if (selectedUser != null)
            {
                AddParticipantToEvent(selectedUser);
            }
            else
            {
                MessageBox.Show("Please select a member from the list.", "No Member Selected", MessageBoxButton.OK, MessageBoxImage.Information);
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
