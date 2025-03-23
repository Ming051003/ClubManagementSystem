using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using Model.Models;

namespace WPF
{
    public partial class NotificationWindow : UserControl
    {
        public string Subject { get; private set; }
        public string Message { get; private set; }
        public string RecipientType { get; private set; }
        public bool IsSent { get; private set; }
        
        // Events to notify parent when actions occur
        public event EventHandler<NotificationEventArgs> SendClicked;
        public event EventHandler CancelClicked;

        public NotificationWindow(string eventName)
        {
            InitializeComponent();
            
            // Set default subject with event name
            txtSubject.Text = $"Notification about: {eventName}";
            
            // Default to "All Participants" radio button
            rbAllParticipants.IsChecked = true;
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

            // Determine recipient type based on selected radio button
            if (rbAllParticipants.IsChecked == true)
                RecipientType = "All Participants";
            else if (rbRegisteredOnly.IsChecked == true)
                RecipientType = "Registered Participants";
            else if (rbAttendedOnly.IsChecked == true)
                RecipientType = "Attended Participants";
            else if (rbAbsentOnly.IsChecked == true)
                RecipientType = "Absent Participants";
            else
                RecipientType = "All Participants"; // Default

            // Set properties
            Subject = txtSubject.Text;
            Message = txtMessage.Text;
            
            // Instead of DialogResult, use the event
            IsSent = true;
            SendClicked?.Invoke(this, new NotificationEventArgs(Subject, Message, RecipientType));
        }

        private void btnCancel_Click(object sender, RoutedEventArgs e)
        {
            // Instead of DialogResult, use the event
            CancelClicked?.Invoke(this, EventArgs.Empty);
        }
    }
    
    // Custom event args class to pass notification data
    public class NotificationEventArgs : EventArgs
    {
        public string Subject { get; }
        public string Message { get; }
        public string RecipientType { get; }
        
        public NotificationEventArgs(string subject, string message, string recipientType)
        {
            Subject = subject;
            Message = message;
            RecipientType = recipientType;
        }
    }
}
