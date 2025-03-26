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

                _allParticipations = _eventParticipantService.GetEventParticipantsByUser(User.Current.UserId).ToList();
                
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
            
            string statusFilter = ((ComboBoxItem)cmbStatus.SelectedItem)?.Content.ToString();
            if (statusFilter != "All Registrations" && !string.IsNullOrEmpty(statusFilter))
            {
                filteredParticipations = filteredParticipations.Where(e => e.Status == statusFilter).ToList();
            }
            
            if (!string.IsNullOrWhiteSpace(txtSearch.Text))
            {
                string searchText = txtSearch.Text.ToLower();
                filteredParticipations = filteredParticipations.Where(e => 
                    e.Event.EventName.ToLower().Contains(searchText) || 
                    (e.Event.Location != null && e.Event.Location.ToLower().Contains(searchText))).ToList();
            }
            
            var participationViewModels = filteredParticipations.Select(p => new
            {
                EventParticipantId = p.EventParticipantId,
                EventId = p.EventId,
                UserId = p.UserId,
                Status = p.Status,
                RegistrationDate = p.RegistrationDate,
                Event = p.Event,
                User = p.User
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
    }
}
