using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using BLL.BusinessInterfaces;
using BLL.BusinessService;
using BLL.Repositories;
using DAL.Interfaces;
using DAL.Repositories;
using Microsoft.Extensions.DependencyInjection;
using Model.Contexts;
using Model.Models;

namespace WPF.VicePresident
{
    public partial class EventManagementByVicePresident : UserControl
    {
        private IEventService _eventService;
        private IAccountService _accountService;
        private IEventParticipantService _eventParticipantService;
        private IClubService _clubService;
        private List<Club> _clubs;
        private Window _hostWindow;
        public event EventHandler<Event> ViewParticipantsRequested;

        public EventManagementByVicePresident()
        {
            InitializeComponent();
            _eventService = ((App)Application.Current).ServiceProvider.GetRequiredService<IEventService>()
               ?? throw new ArgumentNullException(nameof(EventService));
            _accountService = ((App)Application.Current).ServiceProvider.GetRequiredService<IAccountService>()
               ?? throw new ArgumentNullException(nameof(AccountService));
            _eventParticipantService = ((App)Application.Current).ServiceProvider.GetRequiredService<IEventParticipantService>()
               ?? throw new ArgumentNullException(nameof(EventParticipantService));
            _clubService = ((App)Application.Current).ServiceProvider.GetRequiredService<IClubService>()
               ?? throw new ArgumentNullException(nameof(ClubService));
            
            LoadClubs();
            LoadEvents();
            ClearForm();
        }

        private void LoadClubs()
        {
            string username = User.Current?.UserName;
            int? clubIdFromCurrentUser = _accountService.GetClubIdByUsername(username);

            if (clubIdFromCurrentUser == null)
            {
                MessageBox.Show("No club found for the current user.", "Club Not Found", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            _clubs = _clubService.GetClubs().Where(c => c.ClubId == clubIdFromCurrentUser).ToList();
            
            if (_clubs.Count > 0)
            {
                txtCurrentClub.Text = $"Current Club: {_clubs[0].ClubName}";
            }
        }

        private void LoadEvents()
        {
            try
            {
                string username = User.Current?.UserName;
                
                if (string.IsNullOrEmpty(username))
                {
                    MessageBox.Show("User is not logged in.", "Login Required", MessageBoxButton.OK, MessageBoxImage.Warning);
                    return;
                }

                int? clubIdFromCurrentUser = _accountService.GetClubIdByUsername(username);

                if (clubIdFromCurrentUser == null)
                {
                    MessageBox.Show("No club found for the current user.", "Club Not Found", MessageBoxButton.OK, MessageBoxImage.Warning);
                    return;
                }

                var events = _eventService.GetAllEvents()
                    .Where(e => e.ClubId == clubIdFromCurrentUser)
                    .ToList();
                
                ApplyStatusFilter(ref events);
                
                if (!string.IsNullOrWhiteSpace(txtSearch.Text))
                {
                    string searchText = txtSearch.Text.Trim().ToLower();
                    events = events.Where(ev => 
                        ev.EventName.ToLower().Contains(searchText) ||
                        ev.Description.ToLower().Contains(searchText) ||
                        ev.Location.ToLower().Contains(searchText) ||
                        ev.Status.ToLower().Contains(searchText)
                    ).ToList();
                }
                
                dgEvents.ItemsSource = events;
            }
            catch (Exception ex) { }
        }
        
        private void ApplyStatusFilter(ref List<Event> events)
        {
            if (cmbFilterStatus?.SelectedItem is ComboBoxItem selectedStatus && 
                selectedStatus.Content.ToString() != "All Statuses")
            {
                string statusFilter = selectedStatus.Content.ToString();
                events = events.Where(e => e.Status == statusFilter).ToList();
            }
        }

        private void ClearForm()
        {
            txtEventName.Text = string.Empty;
            txtDescription.Text = string.Empty;
            dpEventDate.SelectedDate = DateTime.Today;
            txtLocation.Text = string.Empty;
            txtCapacity.Text = string.Empty;
            cmbStatus.SelectedIndex = 0;
            btnAddEvent.Content = "Add Event";
        }

        private void SearchEvents()
        {
            try
            {
                string username = User.Current?.UserName;
                
                if (string.IsNullOrEmpty(username))
                {
                    MessageBox.Show("User is not logged in.", "Login Required", MessageBoxButton.OK, MessageBoxImage.Warning);
                    return;
                }

                int? clubIdFromCurrentUser = _accountService.GetClubIdByUsername(username);

                if (clubIdFromCurrentUser == null)
                {
                    MessageBox.Show("No club found for the current user.", "Club Not Found", MessageBoxButton.OK, MessageBoxImage.Warning);
                    return;
                }

                var events = _eventService.GetAllEvents()
                    .Where(e => e.ClubId == clubIdFromCurrentUser)
                    .ToList();
                
                ApplyStatusFilter(ref events);
                
                if (!string.IsNullOrWhiteSpace(txtSearch?.Text))
                {
                    string searchText = txtSearch.Text.Trim().ToLower();
                    events = events.Where(ev => 
                        ev.EventName.ToLower().Contains(searchText) ||
                        ev.Description.ToLower().Contains(searchText) ||
                        ev.Location.ToLower().Contains(searchText) ||
                        ev.Status.ToLower().Contains(searchText)
                    ).ToList();
                }
                
                dgEvents.ItemsSource = events;
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error searching events: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }
        
        private void cmbFilterClub_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            LoadEvents();
        }

        private void cmbFilterStatus_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            LoadEvents();
        }

        private void btnSearch_Click(object sender, RoutedEventArgs e)
        {
            SearchEvents();
        }
        
        private void txtSearch_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                SearchEvents();
            }
        }

        private void dgEvents_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            UpdateEventDetails();
        }

        private void UpdateEventDetails()
        {
            var selectedEvent = dgEvents.SelectedItem as Event;
            if (selectedEvent != null)
            {
                txtEventName.Text = selectedEvent.EventName;
                txtDescription.Text = selectedEvent.Description;
                dpEventDate.SelectedDate = selectedEvent.EventDate;
                txtLocation.Text = selectedEvent.Location;
                txtCapacity.Text = selectedEvent.Capacity.ToString();
                
                foreach (ComboBoxItem item in cmbStatus.Items)
                {
                    if (item.Content.ToString() == selectedEvent.Status)
                    {
                        cmbStatus.SelectedItem = item;
                        break;
                    }
                }
            }
            else
            {
                ClearForm();
            }
        }

        private void btnAddEvent_Click(object sender, RoutedEventArgs e)
        {
            btnAddNewEvent_Click(sender, e);
        }

        private void btnAddNewEvent_Click(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrWhiteSpace(txtEventName.Text))
            {
                MessageBox.Show("Event name is required.", "Validation Error", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }
            
            if (dpEventDate.SelectedDate == null)
            {
                MessageBox.Show("Event date is required.", "Validation Error", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }
            
            if (string.IsNullOrWhiteSpace(txtLocation.Text))
            {
                MessageBox.Show("Location is required.", "Validation Error", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }
            
            var eventDate = dpEventDate.SelectedDate.Value;
            var eventName = txtEventName.Text;
            var description = txtDescription.Text;
            var location = txtLocation.Text;
            var capacityText = txtCapacity.Text;
            var status = (cmbStatus.SelectedItem as ComboBoxItem)?.Content.ToString() ?? "Upcoming";

            if (!int.TryParse(capacityText, out int capacity))
            {
                MessageBox.Show("Please enter a valid number for capacity.", "Invalid Capacity", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            try
            {
                string username = User.Current?.UserName;
                if (string.IsNullOrEmpty(username))
                {
                    MessageBox.Show("User is not logged in.", "Login Required", MessageBoxButton.OK, MessageBoxImage.Warning);
                    return;
                }

                int? clubIdFromCurrentUser = _accountService.GetClubIdByUsername(username);
                if (clubIdFromCurrentUser == null)
                {
                    MessageBox.Show("No club found for the current user.", "Club Not Found", MessageBoxButton.OK, MessageBoxImage.Warning);
                    return;
                }

                var existingEvents = _eventService.GetAllEvents()
                    .Where(e => e.ClubId == clubIdFromCurrentUser)
                    .ToList();
                var duplicateEvent = existingEvents.FirstOrDefault(e => 
                    e.EventName.Equals(eventName, StringComparison.OrdinalIgnoreCase) && 
                    e.EventDate.Date == eventDate.Date);
                    
                if (duplicateEvent != null)
                {
                    MessageBox.Show("An event with the same name already exists on this date.", "Duplicate Event", MessageBoxButton.OK, MessageBoxImage.Warning);
                    return;
                }

                var newEvent = new Event
                {
                    EventName = eventName,
                    Description = description,
                    EventDate = eventDate,
                    Location = location,
                    ClubId = (int)clubIdFromCurrentUser,
                    Capacity = capacity,
                    Status = status
                };
                
                _eventService.AddEvent(newEvent);
                MessageBox.Show("Event added successfully!", "Success", MessageBoxButton.OK, MessageBoxImage.Information);
                
                LoadEvents();
                ClearForm();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error adding event: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void btnSaveChanges_Click(object sender, RoutedEventArgs e)
        {
            var selectedEvent = dgEvents.SelectedItem as Event;
            if (selectedEvent == null)
            {
                MessageBox.Show("Please select an event to update.", "No Event Selected", MessageBoxButton.OK, MessageBoxImage.Information);
                return;
            }
            
            if (string.IsNullOrWhiteSpace(txtEventName.Text))
            {
                MessageBox.Show("Event name is required.", "Validation Error", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }
            
            if (dpEventDate.SelectedDate == null)
            {
                MessageBox.Show("Event date is required.", "Validation Error", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }
            
            if (string.IsNullOrWhiteSpace(txtLocation.Text))
            {
                MessageBox.Show("Location is required.", "Validation Error", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }
            
            if (!int.TryParse(txtCapacity.Text, out int capacity))
            {
                MessageBox.Show("Capacity must be a valid number.", "Validation Error", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }
            
            string username = User.Current?.UserName;
            if (string.IsNullOrEmpty(username))
            {
                MessageBox.Show("User is not logged in.", "Login Required", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            int? clubIdFromCurrentUser = _accountService.GetClubIdByUsername(username);
            if (clubIdFromCurrentUser == null)
            {
                MessageBox.Show("No club found for the current user.", "Club Not Found", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }
            
            if (selectedEvent.ClubId != clubIdFromCurrentUser)
            {
                MessageBox.Show("You can only update events for your own club.", "Permission Denied", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }
            
            var eventDate = dpEventDate.SelectedDate.Value;
            var eventName = txtEventName.Text;
            
            var existingEvents = _eventService.GetAllEvents().ToList();
            var duplicateEvent = existingEvents.FirstOrDefault(e => 
                e.EventName.Equals(eventName, StringComparison.OrdinalIgnoreCase) && 
                e.EventDate.Date == eventDate.Date &&
                e.EventId != selectedEvent.EventId);
                
            if (duplicateEvent != null)
            {
                MessageBox.Show("An event with the same name already exists on this date.", "Duplicate Event", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }
            
            try
            {
                selectedEvent.EventName = txtEventName.Text;
                selectedEvent.Description = txtDescription.Text;
                selectedEvent.EventDate = dpEventDate.SelectedDate.Value;
                selectedEvent.Location = txtLocation.Text;
                selectedEvent.Capacity = capacity;
                selectedEvent.Status = (cmbStatus.SelectedItem as ComboBoxItem)?.Content.ToString() ?? "Upcoming";
                
                _eventService.UpdateEvent(selectedEvent);
                MessageBox.Show("Event updated successfully.", "Success", MessageBoxButton.OK, MessageBoxImage.Information);
                
                LoadEvents();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error updating event: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void btnDeleteEvent_Click(object sender, RoutedEventArgs e)
        {
            var selectedEvent = dgEvents.SelectedItem as Event;
            if (selectedEvent != null)
            {
                MessageBoxResult result = MessageBox.Show(
                    $"Are you sure you want to delete the event '{selectedEvent.EventName}'?",
                    "Confirm Deletion",
                    MessageBoxButton.YesNo,
                    MessageBoxImage.Warning);
                
                if (result == MessageBoxResult.Yes)
                {
                    try
                    {
                        _eventService.DeleteEvent(selectedEvent.EventId);
                        LoadEvents();
                        ClearForm();
                        MessageBox.Show("Event deleted successfully.", "Success", MessageBoxButton.OK, MessageBoxImage.Information);
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
                    UpdateEventDetails();
                    MessageBox.Show($"Event status changed to {newStatus}.", "Status Updated", MessageBoxButton.OK, MessageBoxImage.Information);
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Error changing event status: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
            else
            {
                MessageBox.Show("Please select an event to change status.", "No Event Selected", MessageBoxButton.OK, MessageBoxImage.Information);
            }
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

        private void btnViewParticipants_Click(object sender, RoutedEventArgs e)
        {
            var selectedEvent = dgEvents.SelectedItem as Event;
            if (selectedEvent != null)
            {
                ViewParticipantsRequested?.Invoke(this, selectedEvent);
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
                var mainWindow = Window.GetWindow(this) as Main_VicePresident_WPF;
                if (mainWindow != null)
                {
                    mainWindow.NavigateToNotificationTab(selectedEvent.EventName);
                }
            }
            else
            {
                MessageBox.Show("Please select an event first.", "No Event Selected", MessageBoxButton.OK, MessageBoxImage.Information);
            }
        }
        
        private void btnClearForm_Click(object sender, RoutedEventArgs e)
        {
            ClearForm();
            dgEvents.SelectedItem = null;
        }
    }
}
