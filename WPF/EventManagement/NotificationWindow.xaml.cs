using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using Model.Models;

namespace WPF
{
    public partial class NotificationWindow : Window
    {
        public string Subject { get; private set; }
        public string Message { get; private set; }
        public string RecipientType { get; private set; }
        public bool IsSent { get; private set; }

        public NotificationWindow(string eventName)
        {
            InitializeComponent();
            
            // Set default subject with event name
            txtSubject.Text = $"Notification about: {eventName}";
            
            // Set owner to ensure it's centered on parent window
            Owner = Application.Current.MainWindow;
        }

        private void btnSend_Click(object sender, RoutedEventArgs e)
        {
            // Validate inputs
            if (string.IsNullOrWhiteSpace(txtSubject.Text))
            {
                MessageBox.Show("Please enter a subject for the notification.", "Missing Subject", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            if (string.IsNullOrWhiteSpace(txtMessage.Text))
            {
                MessageBox.Show("Please enter a message for the notification.", "Missing Message", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            // Get recipient type
            if (rbAllParticipants.IsChecked == true)
                RecipientType = "All";
            else if (rbRegisteredOnly.IsChecked == true)
                RecipientType = "Registered";
            else if (rbAttendedOnly.IsChecked == true)
                RecipientType = "Attended";
            else if (rbAbsentOnly.IsChecked == true)
                RecipientType = "Absent";

            // Set properties
            Subject = txtSubject.Text;
            Message = txtMessage.Text;
            IsSent = true;

            // Close dialog
            DialogResult = true;
        }

        private void btnCancel_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = false;
        }
    }
}
