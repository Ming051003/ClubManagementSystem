using BLL.BusinessInterfaces;
using Model.Models;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;

namespace WPF.ViewModels
{
    public class EventViewModel : ViewModelBase
    {
        private readonly IEventService _eventService;
        private readonly IUserService _userService;
        private readonly IEventParticipantService _eventParticipantService;

        private ObservableCollection<Event> _events;
        private Event _selectedEvent;
        private ObservableCollection<EventParticipant> _eventParticipants;
        private ObservableCollection<User> _availableMembers;
        private User _selectedMember;
        private string _searchText;
        private Dictionary<string, int> _participantStats;
        private bool _hasSelectedEvent;

        public EventViewModel(IEventService eventService, IUserService userService, IEventParticipantService eventParticipantService)
        {
            _eventService = eventService;
            _userService = userService;
            _eventParticipantService = eventParticipantService;

            // Initialize collections
            Events = new ObservableCollection<Event>();
            EventParticipants = new ObservableCollection<EventParticipant>();
            AvailableMembers = new ObservableCollection<User>();
            ParticipantStats = new Dictionary<string, int>();

            // Initialize commands
            AddEventCommand = new RelayCommand(param => AddEvent());
            EditEventCommand = new RelayCommand(param => EditEvent(), param => SelectedEvent != null);
            DeleteEventCommand = new RelayCommand(param => DeleteEvent(), param => SelectedEvent != null);
            ViewParticipantsCommand = new RelayCommand(param => ViewParticipants(), param => SelectedEvent != null);
            AddParticipantCommand = new RelayCommand(param => AddParticipant(), param => CanAddParticipant());
            SendNotificationCommand = new RelayCommand(param => SendNotification(), param => SelectedEvent != null);
            ChangeEventStatusCommand = new RelayCommand(param => ChangeEventStatus(param as string), param => SelectedEvent != null);
            SearchCommand = new RelayCommand(param => Search());

            // Load data
            LoadEvents();
        }

        public ObservableCollection<Event> Events
        {
            get => _events;
            set => SetProperty(ref _events, value);
        }

        public Event SelectedEvent
        {
            get => _selectedEvent;
            set
            {
                if (SetProperty(ref _selectedEvent, value) && value != null)
                {
                    // Load participants data when an event is selected
                    RefreshParticipantData();
                    
                    // Set HasSelectedEvent property
                    HasSelectedEvent = true;
                }
                else if (value == null)
                {
                    HasSelectedEvent = false;
                }
            }
        }

        public ObservableCollection<EventParticipant> EventParticipants
        {
            get => _eventParticipants;
            set => SetProperty(ref _eventParticipants, value);
        }

        public ObservableCollection<User> AvailableMembers
        {
            get => _availableMembers;
            set => SetProperty(ref _availableMembers, value);
        }

        public User SelectedMember
        {
            get => _selectedMember;
            set => SetProperty(ref _selectedMember, value);
        }

        public string SearchText
        {
            get => _searchText;
            set => SetProperty(ref _searchText, value);
        }

        public Dictionary<string, int> ParticipantStats
        {
            get => _participantStats;
            set => SetProperty(ref _participantStats, value);
        }

        public bool HasSelectedEvent
        {
            get => _hasSelectedEvent;
            set => SetProperty(ref _hasSelectedEvent, value);
        }

        public ICommand AddEventCommand { get; }
        public ICommand EditEventCommand { get; }
        public ICommand DeleteEventCommand { get; }
        public ICommand ViewParticipantsCommand { get; }
        public ICommand AddParticipantCommand { get; }
        public ICommand SendNotificationCommand { get; }
        public ICommand ChangeEventStatusCommand { get; }
        public ICommand SearchCommand { get; }

        private async void LoadEvents()
        {
            try
            {
                var events = await Task.Run(() => _eventService.GetAllEvents());
                Events.Clear();
                foreach (var evt in events)
                {
                    Events.Add(evt);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error loading events: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void AddEvent()
        {
            // Create a new EventEditWindow for adding an event
            var newEvent = new Event
            {
                EventDate = DateTime.Now.AddDays(7), // Default to one week from now
                Status = "Upcoming" // Default status
            };
            
            try
            {
                var editWindow = new EventEditWindow(newEvent);
                var result = editWindow.ShowDialog();
                
                if (result == true && editWindow.IsSaved)
                {
                    var addedEvent = editWindow.EditedEvent;
                    
                    // Save to database asynchronously
                    Task.Run(() => _eventService.AddEvent(addedEvent)).ContinueWith(t => 
                    {
                        Application.Current.Dispatcher.Invoke(() => 
                        {
                            // Update UI on main thread
                            Events.Add(addedEvent);
                            SelectedEvent = addedEvent;
                            MessageBox.Show("Event added successfully.", "Success", MessageBoxButton.OK, MessageBoxImage.Information);
                        });
                    });
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error adding event: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private async void EditEvent()
        {
            if (SelectedEvent == null) return;

            try
            {
                // Create and show the event edit window
                var editWindow = new EventEditWindow(SelectedEvent);
                var result = editWindow.ShowDialog();
                
                // If user clicked Save
                if (result == true && editWindow.IsSaved)
                {
                    // Get the edited event
                    var editedEvent = editWindow.EditedEvent;
                    
                    // Save changes to database
                    await Task.Run(() => _eventService.UpdateEvent(editedEvent));
                    
                    // Update the collection with the edited event
                    int index = Events.IndexOf(SelectedEvent);
                    if (index >= 0)
                    {
                        Events[index] = editedEvent;
                        SelectedEvent = editedEvent;
                    }
                    
                    MessageBox.Show("Event updated successfully.", "Success", MessageBoxButton.OK, MessageBoxImage.Information);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error updating event: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private async void DeleteEvent()
        {
            if (SelectedEvent == null) return;

            var result = MessageBox.Show($"Are you sure you want to delete the event '{SelectedEvent.EventName}'?",
                "Confirm Delete", MessageBoxButton.YesNo, MessageBoxImage.Question);

            if (result == MessageBoxResult.Yes)
            {
                try
                {
                    await Task.Run(() => _eventService.DeleteEvent(SelectedEvent.EventId));
                    Events.Remove(SelectedEvent);
                    MessageBox.Show("Event deleted successfully.", "Success", MessageBoxButton.OK, MessageBoxImage.Information);
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Error deleting event: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
        }

        private async void ViewParticipants()
        {
            if (SelectedEvent == null) return;

            try
            {
                // Load participants for the selected event
                var participants = await Task.Run(() => 
                    _eventParticipantService.GetEventParticipantsByEvent(SelectedEvent.EventId));
                
                EventParticipants.Clear();
                foreach (var participant in participants)
                {
                    EventParticipants.Add(participant);
                }

                // Load available members who haven't registered yet
                var allMembers = await Task.Run(() => _userService.GetAllUsers());
                var registeredUserIds = participants.Select(p => p.UserId).ToList();
                
                AvailableMembers.Clear();
                foreach (var member in allMembers.Where(m => !registeredUserIds.Contains(m.UserId)))
                {
                    AvailableMembers.Add(member);
                }

                // Update participant statistics
                UpdateParticipantStatistics();
                
                // Create and show the participant window
                var participantWindow = new ParticipantWindow(SelectedEvent, _eventParticipantService, EventParticipants);
                participantWindow.ShowDialog();
                
                // Refresh data after the window is closed
                RefreshParticipantData();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error loading participants: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }
        
        private async void RefreshParticipantData()
        {
            if (SelectedEvent == null) return;
            
            try
            {
                // Load participants for the selected event
                var participants = await Task.Run(() => 
                    _eventParticipantService.GetEventParticipantsByEvent(SelectedEvent.EventId));
                
                EventParticipants.Clear();
                foreach (var participant in participants)
                {
                    EventParticipants.Add(participant);
                }

                // Load available members who haven't registered yet
                var allMembers = await Task.Run(() => _userService.GetAllUsers());
                var registeredUserIds = participants.Select(p => p.UserId).ToList();
                
                AvailableMembers.Clear();
                foreach (var member in allMembers.Where(m => !registeredUserIds.Contains(m.UserId)))
                {
                    AvailableMembers.Add(member);
                }

                // Update participant statistics
                UpdateParticipantStatistics();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error refreshing participant data: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private async void AddParticipant()
        {
            if (SelectedEvent == null || SelectedMember == null) return;

            try
            {
                var newParticipant = new EventParticipant
                {
                    EventId = SelectedEvent.EventId,
                    UserId = SelectedMember.UserId,
                    Status = "Registered",
                    RegistrationDate = DateTime.Now
                };

                await Task.Run(() => _eventParticipantService.AddEventParticipant(newParticipant));
                
                // Refresh the participants list
                RefreshParticipantData();
                
                MessageBox.Show($"{SelectedMember.FullName} has been registered for the event.", 
                    "Success", MessageBoxButton.OK, MessageBoxImage.Information);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error adding participant: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private bool CanAddParticipant()
        {
            return SelectedEvent != null && SelectedMember != null;
        }

        private void SendNotification()
        {
            if (SelectedEvent == null) return;

            try
            {
                // Create and show the notification window
                var notificationWindow = new NotificationWindow(SelectedEvent.EventName);
                
                // Show the window as a dialog
                var result = notificationWindow.ShowDialog();
                
                // If user clicked Send
                if (result == true && notificationWindow.IsSent)
                {
                    // Get notification details
                    string subject = notificationWindow.Subject;
                    string message = notificationWindow.Message;
                    string recipientType = notificationWindow.RecipientType;
                    
                    // In a real application, this would send emails or notifications based on recipient type
                    // For now, we'll just show a message box with the details
                    MessageBox.Show($"Notification sent!\n\nSubject: {subject}\nTo: {recipientType} participants\nMessage: {message}",
                        "Notification Sent", MessageBoxButton.OK, MessageBoxImage.Information);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error sending notifications: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private async void ChangeEventStatus(string newStatus)
        {
            if (SelectedEvent == null || string.IsNullOrEmpty(newStatus)) return;

            try
            {
                // Validate the status
                if (!new[] { "Upcoming", "Ongoing", "Completed", "Cancelled" }.Contains(newStatus))
                {
                    MessageBox.Show("Invalid status value.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                    return;
                }

                // Update the event status
                SelectedEvent.Status = newStatus;
                await Task.Run(() => _eventService.UpdateEvent(SelectedEvent));
                
                // Refresh the events list
                LoadEvents();
                
                MessageBox.Show($"Event status updated to '{newStatus}'.", 
                    "Success", MessageBoxButton.OK, MessageBoxImage.Information);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error changing event status: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void Search()
        {
            if (string.IsNullOrWhiteSpace(SearchText))
            {
                LoadEvents();
                return;
            }

            try
            {
                var filteredEvents = _eventService.GetAllEvents()
                    .Where(e => e.EventName.Contains(SearchText, StringComparison.OrdinalIgnoreCase) ||
                                e.Description.Contains(SearchText, StringComparison.OrdinalIgnoreCase) ||
                                e.Location.Contains(SearchText, StringComparison.OrdinalIgnoreCase));

                Events.Clear();
                foreach (var evt in filteredEvents)
                {
                    Events.Add(evt);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error searching events: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void UpdateParticipantStatistics()
        {
            if (SelectedEvent == null) return;

            try
            {
                var participants = _eventParticipantService.GetEventParticipantsByEvent(SelectedEvent.EventId);

                var stats = new Dictionary<string, int>
                {
                    { "Registered", participants.Count(p => p.Status == "Registered") },
                    { "Attended", participants.Count(p => p.Status == "Attended") },
                    { "Absent", participants.Count(p => p.Status == "Absent") },
                    { "Total", participants.Count() }
                };

                ParticipantStats = stats;
                OnPropertyChanged(nameof(ParticipantStats));
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error updating participant statistics: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }
    }
}
