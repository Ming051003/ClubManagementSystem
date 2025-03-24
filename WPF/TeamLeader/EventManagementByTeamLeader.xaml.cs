using BLL.BusinessInterfaces;
using BLL.BusinessService;
using Microsoft.Extensions.DependencyInjection;
using Model.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace WPF.TeamLeader
{
    /// <summary>
    /// Interaction logic for EventManagementByTeamLeader.xaml
    /// </summary>
    public partial class EventManagementByTeamLeader : UserControl
    {
        private IEventService _eventService;
        private IAccountService _accountService;
        private IClubService _clubService;
        private List<Club> _clubs;

        public EventManagementByTeamLeader()
        {
            InitializeComponent();
            _eventService = ((App)Application.Current).ServiceProvider.GetRequiredService<IEventService>()
                ?? throw new ArgumentNullException(nameof(EventService));
            _accountService = ((App)Application.Current).ServiceProvider.GetRequiredService<IAccountService>()
                ?? throw new ArgumentNullException(nameof(AccountService));
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
                cmbClub.ItemsSource = _clubs;
                cmbClub.SelectedValue = _clubs[0].ClubId;
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
            catch (Exception ex)
            {
                MessageBox.Show($"Error loading events: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void ApplyStatusFilter(ref List<Event> events)
        {
            if (cmbStatus?.SelectedItem is ComboBoxItem selectedStatus &&
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

        private void cmbStatus_SelectionChanged(object sender, SelectionChangedEventArgs e)
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

        private void btnProposeEvent_Click(object sender, RoutedEventArgs e)
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

            int capacity;
            if (!int.TryParse(txtCapacity.Text, out capacity))
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

            var eventDate = dpEventDate.SelectedDate.Value;
            var eventName = txtEventName.Text;

            var existingEvents = _eventService.GetAllEvents().ToList();
            var duplicateEvent = existingEvents.FirstOrDefault(e =>
                e.EventName.Equals(eventName, StringComparison.OrdinalIgnoreCase) &&
                e.EventDate.Date == eventDate.Date);

            if (duplicateEvent != null)
            {
                MessageBox.Show("An event with the same name already exists on this date.", "Duplicate Event", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            try
            {
                var newEvent = new Event
                {
                    EventName = txtEventName.Text,
                    Description = txtDescription.Text,
                    EventDate = dpEventDate.SelectedDate.Value,
                    Location = txtLocation.Text,
                    ClubId = clubIdFromCurrentUser.Value,
                    Capacity = capacity,
                    Status = (cmbStatus.SelectedItem as ComboBoxItem)?.Content.ToString() ?? "Upcoming"
                };

                _eventService.AddEvent(newEvent);
                MessageBox.Show("Event proposed successfully.", "Success", MessageBoxButton.OK, MessageBoxImage.Information);

                LoadEvents();
                ClearForm();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error proposing event: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void btnUpdateMemberStatus_Click(object sender, RoutedEventArgs e)
        {
            var selectedEvent = dgEvents.SelectedItem as Event;
            if (selectedEvent != null)
            {
                // Implement the logic to update member status
                MessageBox.Show("Update member status functionality is not implemented yet.", "Info", MessageBoxButton.OK, MessageBoxImage.Information);
            }
            else
            {
                MessageBox.Show("Please select an event to update member status.", "No Event Selected", MessageBoxButton.OK, MessageBoxImage.Information);
            }
        }
    }
}
