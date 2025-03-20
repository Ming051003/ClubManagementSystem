using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using BLL.BusinessInterfaces;
using BLL.BusinessService;
using DAL.Interfaces;
using DAL.Repositories;
using Model.Contexts;
using Model.Models;

namespace WPF
{
    /// <summary>
    /// Interaction logic for EventManagement.xaml
    /// </summary>
    public partial class EventManagement : Window
    {
        private readonly IEventService _eventService;
        private readonly IUserService _userService;
        private readonly IEventParticipantService _eventParticipantService;
        
        private List<Event> _events;
        private Event _selectedEvent;
        private List<EventParticipant> _eventParticipants;
        private List<User> _availableMembers;

        public EventManagement()
        {
            InitializeComponent();
            
            // Create context and repositories
            var context = new ClubManagementContext();
            IEventRepository eventRepository = new EventRepository(context);
            IUserRepository userRepository = new UserRepository(context);
            IEventParticipantRepository eventParticipantRepository = new EventParticipantRepository(context);
            
            // Create services
            _eventService = new EventService(eventRepository);
            _userService = new UserService(userRepository);
            _eventParticipantService = new EventParticipantService(eventParticipantRepository);
            
            // Load events
            LoadEvents();
        }

        private void LoadEvents()
        {
            _events = _eventService.GetAllEvents().ToList();
            dgEvents.ItemsSource = _events;
        }

        private void LoadEventParticipants()
        {
            if (_selectedEvent != null)
            {
                _eventParticipants = _eventParticipantService.GetEventParticipantsByEvent(_selectedEvent.EventId);
                dgParticipants.ItemsSource = _eventParticipants;
                UpdateParticipantStats();
            }
        }

        private void LoadAvailableMembers()
        {
            if (_selectedEvent != null)
            {
                var allUsers = _userService.GetAllUsers();
                var currentParticipantIds = _eventParticipants.Select(p => p.UserId).ToList();
                _availableMembers = allUsers.Where(u => !currentParticipantIds.Contains(u.UserId)).ToList();
                cmbAvailableMembers.ItemsSource = _availableMembers;
            }
        }

        private void UpdateParticipantStats()
        {
            if (_eventParticipants != null)
            {
                txtRegisteredCount.Text = _eventParticipants.Count(p => p.Status == "Registered").ToString();
                txtAttendedCount.Text = _eventParticipants.Count(p => p.Status == "Attended").ToString();
                txtAbsentCount.Text = _eventParticipants.Count(p => p.Status == "Absent").ToString();
                txtTotalCount.Text = _eventParticipants.Count.ToString();
            }
        }

        private void UpdateEventDetails()
        {
            if (_selectedEvent != null)
            {
                txtEventName.Text = _selectedEvent.EventName;
                txtEventDate.Text = _selectedEvent.EventDate.ToString("d");
                txtEventStatus.Text = _selectedEvent.Status;
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
            if (string.IsNullOrEmpty(searchText))
            {
                dgEvents.ItemsSource = _events;
            }
            else
            {
                dgEvents.ItemsSource = _events.Where(ev => 
                    ev.EventName.ToLower().Contains(searchText) ||
                    ev.Description.ToLower().Contains(searchText) ||
                    ev.Location.ToLower().Contains(searchText) ||
                    ev.Status.ToLower().Contains(searchText)
                ).ToList();
            }
        }

        private void dgEvents_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            _selectedEvent = dgEvents.SelectedItem as Event;
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
            if (_selectedEvent != null)
            {
                var eventEditWindow = new EventEditWindow(_selectedEvent);
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
            if (_selectedEvent != null)
            {
                var result = MessageBox.Show($"Are you sure you want to delete the event '{_selectedEvent.EventName}'?", 
                    "Confirm Delete", MessageBoxButton.YesNo, MessageBoxImage.Question);
                
                if (result == MessageBoxResult.Yes)
                {
                    try
                    {
                        _eventService.DeleteEvent(_selectedEvent.EventId);
                        LoadEvents();
                        _selectedEvent = null;
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
            if (_selectedEvent != null)
            {
                var participants = new ObservableCollection<EventParticipant>(_eventParticipants);
                var participantWindow = new ParticipantWindow(_selectedEvent, _eventParticipantService, participants);
                participantWindow.ShowDialog();
                LoadEventParticipants(); // Refresh after returning
            }
            else
            {
                MessageBox.Show("Please select an event to view participants.", "No Event Selected", MessageBoxButton.OK, MessageBoxImage.Information);
            }
        }

        private void btnSendNotification_Click(object sender, RoutedEventArgs e)
        {
            if (_selectedEvent != null)
            {
                var notificationWindow = new NotificationWindow(_selectedEvent.EventName);
                notificationWindow.ShowDialog();
            }
            else
            {
                MessageBox.Show("Please select an event to send notifications.", "No Event Selected", MessageBoxButton.OK, MessageBoxImage.Information);
            }
        }

        private void ChangeEventStatus(string newStatus)
        {
            if (_selectedEvent != null)
            {
                try
                {
                    _selectedEvent.Status = newStatus;
                    _eventService.UpdateEvent(_selectedEvent);
                    LoadEvents();
                    UpdateEventDetails();
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Error updating event status: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
            else
            {
                MessageBox.Show("Please select an event to change its status.", "No Event Selected", MessageBoxButton.OK, MessageBoxImage.Information);
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

        private void btnAddParticipant_Click(object sender, RoutedEventArgs e)
        {
            if (_selectedEvent != null && cmbAvailableMembers.SelectedItem != null)
            {
                var selectedUser = cmbAvailableMembers.SelectedItem as User;
                
                if (selectedUser != null)
                {
                    try
                    {
                        var newParticipant = new EventParticipant
                        {
                            EventId = _selectedEvent.EventId,
                            UserId = selectedUser.UserId,
                            Status = "Registered",
                            RegistrationDate = DateTime.Now
                        };
                        
                        _eventParticipantService.AddEventParticipant(newParticipant);
                        LoadEventParticipants();
                        LoadAvailableMembers();
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show($"Error adding participant: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                    }
                }
            }
            else
            {
                MessageBox.Show("Please select an event and a member to add.", "Selection Required", MessageBoxButton.OK, MessageBoxImage.Information);
            }
        }
    }
}
