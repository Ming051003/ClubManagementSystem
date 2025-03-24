using BLL.BusinessInterfaces;
using Microsoft.Extensions.DependencyInjection;
using Model.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace WPF.Member
{
    /// <summary>
    /// Interaction logic for EventHistory.xaml
    /// </summary>
    public partial class EventHistory : UserControl
    {
        private readonly IEventService _eventService;
        private readonly IEventParticipantService _eventParticipantService;
        private List<EventParticipant> _allParticipations;

        public EventHistory()
        {
            InitializeComponent();
            _eventService = ((App)Application.Current).ServiceProvider.GetRequiredService<IEventService>()
                ?? throw new ArgumentNullException(nameof(IEventService));
            _eventParticipantService = ((App)Application.Current).ServiceProvider.GetRequiredService<IEventParticipantService>()
                ?? throw new ArgumentNullException(nameof(IEventParticipantService));
            LoadEventHistory();
        }

        private void LoadEventHistory()
        {
            try
            {
                if (User.Current == null)
                {
                    MessageBox.Show("You are not logged in.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                    return;
                }

                // Get all participations for the current user
                _allParticipations = _eventParticipantService.GetEventParticipantsByUser(User.Current.UserId).ToList();
                
                // Apply initial filter (All Participations)
                ApplyFilters();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error loading event history: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void ApplyFilters()
        {
            if (_allParticipations == null) return;

            var filteredParticipations = _allParticipations;
            
            // Apply status filter
            string statusFilter = ((ComboBoxItem)cmbStatus.SelectedItem)?.Content.ToString();
            if (statusFilter != "All Registrations" && !string.IsNullOrEmpty(statusFilter))
            {
                filteredParticipations = filteredParticipations.Where(e => e.Status == statusFilter).ToList();
            }
            
            // Apply search text filter
            if (!string.IsNullOrWhiteSpace(txtSearch.Text))
            {
                string searchText = txtSearch.Text.ToLower();
                filteredParticipations = filteredParticipations.Where(e => 
                    e.Event.EventName.ToLower().Contains(searchText) || 
                    (e.Event.Location != null && e.Event.Location.ToLower().Contains(searchText))).ToList();
            }
            
            // Create view models with unenroll capability
            var participationViewModels = filteredParticipations.Select(p => new
            {
                EventParticipantId = p.EventParticipantId,
                EventId = p.EventId,
                UserId = p.UserId,
                Status = p.Status,
                RegistrationDate = p.RegistrationDate,
                Event = p.Event,
                User = p.User,
                // Can unenroll if event status is Upcoming or Cancelled
                CanUnenroll = p.Event.Status == "Upcoming" || p.Event.Status == "Cancelled"
            }).ToList();
            
            dgEventHistory.ItemsSource = participationViewModels;
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

        private void dgEventHistory_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            var selectedParticipation = dgEventHistory.SelectedItem as dynamic;
            if (selectedParticipation != null && selectedParticipation.Event != null)
            {
                txtEventName.Text = selectedParticipation.Event.EventName;
                txtClub.Text = selectedParticipation.Event.Club?.ClubName ?? "N/A";
                txtEventDate.Text = selectedParticipation.Event.EventDate.ToString("dd/MM/yyyy");
                txtRegistrationDate.Text = selectedParticipation.RegistrationDate?.ToString("dd/MM/yyyy") ?? "N/A";
                txtDescription.Text = selectedParticipation.Event.Description ?? "No description available.";
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
            txtEventDate.Text = string.Empty;
            txtRegistrationDate.Text = string.Empty;
            txtDescription.Text = string.Empty;
        }

        private void btnUnenroll_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                // Get the event participation from the button's DataContext
                var button = sender as Button;
                var viewModel = button.DataContext as dynamic;
                
                if (viewModel == null || User.Current == null)
                {
                    MessageBox.Show("Unable to unenroll. Please try again.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                    return;
                }
                
                int eventId = viewModel.EventId;
                string eventName = viewModel.Event.EventName;
                
                // Confirm unenrollment
                var result = MessageBox.Show($"Are you sure you want to unenroll from the event '{eventName}'?", 
                    "Confirm Unenrollment", MessageBoxButton.YesNo, MessageBoxImage.Question);
                
                if (result == MessageBoxResult.Yes)
                {
                    // Delete the event participation
                    _eventParticipantService.DeleteEventParticipant(eventId, User.Current.UserId);
                    
                    MessageBox.Show($"You have successfully unenrolled from the event '{eventName}'.", 
                        "Unenrollment Successful", MessageBoxButton.OK, MessageBoxImage.Information);
                    
                    // Refresh event history to update status
                    LoadEventHistory();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error unenrolling from event: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }
    }
}
