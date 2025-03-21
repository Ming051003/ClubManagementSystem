using System;
using System.Windows;
using System.Windows.Controls;
using Model.Models;

namespace WPF
{
    public partial class EventEditWindow : Window
    {
        public Event EditedEvent { get; private set; }
        public bool IsSaved { get; private set; }

        public EventEditWindow(Event eventToEdit)
        {
            InitializeComponent();
            
            Owner = Application.Current.MainWindow;
            
            if (eventToEdit != null)
            {
                EditedEvent = new Event
                {
                    EventId = eventToEdit.EventId,
                    EventName = eventToEdit.EventName,
                    Description = eventToEdit.Description,
                    EventDate = eventToEdit.EventDate,
                    Location = eventToEdit.Location,
                    ClubId = eventToEdit.ClubId,
                    Capacity = eventToEdit.Capacity,
                    Status = eventToEdit.Status
                };
                
                LoadEventData();
            }
        }

        private void LoadEventData()
        {
            txtEventName.Text = EditedEvent.EventName;
            txtDescription.Text = EditedEvent.Description;
            dpEventDate.SelectedDate = EditedEvent.EventDate;
            txtLocation.Text = EditedEvent.Location;
            txtClubId.Text = EditedEvent.ClubId.ToString();
            txtCapacity.Text = EditedEvent.Capacity?.ToString();
            
            // Set the selected status
            foreach (ComboBoxItem item in cmbStatus.Items)
            {
                if (item.Content.ToString() == EditedEvent.Status)
                {
                    cmbStatus.SelectedItem = item;
                    break;
                }
            }
        }

        private void btnSave_Click(object sender, RoutedEventArgs e)
        {
            // Validate inputs
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

            if (string.IsNullOrWhiteSpace(txtClubId.Text) || !int.TryParse(txtClubId.Text, out int clubId))
            {
                MessageBox.Show("Club ID must be a valid integer.", "Validation Error", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            int? capacity = null;
            if (!string.IsNullOrWhiteSpace(txtCapacity.Text) && !int.TryParse(txtCapacity.Text, out int capacityVal))
            {
                MessageBox.Show("Capacity must be a valid integer.", "Validation Error", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }
            else if (!string.IsNullOrWhiteSpace(txtCapacity.Text))
            {
                capacity = int.Parse(txtCapacity.Text);
            }

            if (cmbStatus.SelectedItem == null)
            {
                MessageBox.Show("Please select a status.", "Validation Error", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            // Update the event with form values
            EditedEvent.EventName = txtEventName.Text;
            EditedEvent.Description = txtDescription.Text;
            EditedEvent.EventDate = dpEventDate.SelectedDate.Value;
            EditedEvent.Location = txtLocation.Text;
            EditedEvent.ClubId = clubId;
            EditedEvent.Capacity = capacity;
            EditedEvent.Status = cmbStatus.SelectedValue?.ToString() ?? string.Empty;

            IsSaved = true;
            DialogResult = true;
        }

        private void btnCancel_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = false;
        }
    }
}
