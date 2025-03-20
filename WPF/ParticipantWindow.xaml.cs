using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using BLL.BusinessInterfaces;
using Model.Models;

namespace WPF
{
    public partial class ParticipantWindow : Window
    {
        private readonly IEventParticipantService _eventParticipantService;
        private readonly Event _event;
        private ObservableCollection<EventParticipant> _participants;
        
        public ParticipantWindow(Event eventItem, IEventParticipantService eventParticipantService, ObservableCollection<EventParticipant> participants)
        {
            InitializeComponent();
            
            _event = eventItem;
            _eventParticipantService = eventParticipantService;
            _participants = participants;
            
            // Set window owner
            Owner = Application.Current.MainWindow;
            
            // Initialize UI with event data
            InitializeEventData();
            
            // Load participants
            LoadParticipants();
        }
        
        private void InitializeEventData()
        {
            txtEventTitle.Text = $"Participants for: {_event.EventName}";
            txtEventDate.Text = _event.EventDate.ToString("d");
            txtEventLocation.Text = _event.Location;
            txtEventStatus.Text = _event.Status;
            
            // Update statistics
            UpdateParticipantStatistics();
        }
        
        private void LoadParticipants()
        {
            dgParticipants.ItemsSource = _participants;
        }
        
        private void UpdateParticipantStatistics()
        {
            int registered = _participants.Count(p => p.Status == "Registered");
            int attended = _participants.Count(p => p.Status == "Attended");
            int absent = _participants.Count(p => p.Status == "Absent");
            int total = _participants.Count;
            
            txtRegisteredCount.Text = registered.ToString();
            txtAttendedCount.Text = attended.ToString();
            txtAbsentCount.Text = absent.ToString();
            txtTotalCount.Text = total.ToString();
        }
        
        private void btnChangeStatus_Click(object sender, RoutedEventArgs e)
        {
            var selectedParticipant = dgParticipants.SelectedItem as EventParticipant;
            if (selectedParticipant == null)
            {
                MessageBox.Show("Please select a participant first.", "No Selection", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }
            
            string newStatus = (cmbStatus.SelectedItem as ComboBoxItem)?.Content.ToString();
            if (string.IsNullOrEmpty(newStatus))
            {
                MessageBox.Show("Please select a status.", "No Status Selected", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }
            
            // Update participant status
            selectedParticipant.Status = newStatus;
            _eventParticipantService.UpdateEventParticipant(selectedParticipant);
            
            // Refresh UI
            dgParticipants.Items.Refresh();
            UpdateParticipantStatistics();
            
            MessageBox.Show($"Participant status updated to {newStatus}.", "Status Updated", MessageBoxButton.OK, MessageBoxImage.Information);
        }
        
        private void btnRemoveParticipant_Click(object sender, RoutedEventArgs e)
        {
            var selectedParticipant = dgParticipants.SelectedItem as EventParticipant;
            if (selectedParticipant == null)
            {
                MessageBox.Show("Please select a participant first.", "No Selection", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }
            
            var result = MessageBox.Show($"Are you sure you want to remove {selectedParticipant.User.FullName} from this event?", 
                "Confirm Removal", MessageBoxButton.YesNo, MessageBoxImage.Question);
                
            if (result == MessageBoxResult.Yes)
            {
                // Remove participant
                _eventParticipantService.DeleteEventParticipant(selectedParticipant.EventId, selectedParticipant.UserId);
                _participants.Remove(selectedParticipant);
                
                // Refresh UI
                UpdateParticipantStatistics();
                
                MessageBox.Show("Participant has been removed from the event.", "Participant Removed", MessageBoxButton.OK, MessageBoxImage.Information);
            }
        }
        
        private void btnClose_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }
    }
}
