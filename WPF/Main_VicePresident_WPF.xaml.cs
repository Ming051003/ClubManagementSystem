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
using System.Windows.Shapes;
using FontAwesome.Sharp;
using WPF.Admin;
using WPF.VicePresident;
using Model.Models;

namespace WPF
{
    public partial class Main_VicePresident_WPF : Window
    {
        private Event _selectedEvent;
        
        public Main_VicePresident_WPF()
        {
            InitializeComponent();
        }
        private void pnlControlBar_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            DragMove();
        }

        private void pnlControlBar_MouseEnter(object sender, MouseEventArgs e)
        {
            this.MaxHeight = SystemParameters.MaximizedPrimaryScreenHeight;
        }

        private void btnClose_Click(object sender, RoutedEventArgs e)
        {
            Application.Current.Shutdown();
        }

        private void btnMinimize_Click(object sender, RoutedEventArgs e)
        {
            this.WindowState = WindowState.Minimized;
        }
        private void btnMaximize_Click(object sender, RoutedEventArgs e)
        {
            if (this.WindowState == WindowState.Normal)
                this.WindowState = WindowState.Maximized;
            else this.WindowState = WindowState.Normal;
        }

        private void UC_VicePresident_Member_Checked(object sender, RoutedEventArgs e)
        {
            MainContent.Content = new AccountManagementByVicePresedent();
        }
        
        private void UC_VicePresident_Event_Checked(object sender, RoutedEventArgs e)
        {
            var eventManagement = new EventManagement();
            eventManagement.ViewParticipantsRequested += OnViewParticipantsRequested;
            MainContent.Content = eventManagement;
        }
        
        private void OnViewParticipantsRequested(object sender, Event eventData)
        {
            _selectedEvent = eventData;
            rbEventParticipant.IsChecked = true;
        }
        
        private void UC_VicePresident_EventParticipant_Checked(object sender, RoutedEventArgs e)
        {
            if (_selectedEvent != null)
            {
                MainContent.Content = new EventParticipantManagement(_selectedEvent);
                _selectedEvent = null;
            }
            else
            {
                MainContent.Content = new EventParticipantManagement();
            }
        }
        
        private void UC_VicePresident_Notification_Checked(object sender, RoutedEventArgs e)
        {
            NavigateToNotificationTab("All Events");
        }
        
        public void NavigateToNotificationTab(string eventName)
        {
            rbNotification.IsChecked = true;
            var notificationWindow = new NotificationWindow(eventName);
            
            notificationWindow.SendClicked += (s, args) => {
                MessageBox.Show($"Notification sent to {args.RecipientType} with subject: {args.Subject}", 
                    "Notification Sent", MessageBoxButton.OK, MessageBoxImage.Information);
            };
            
            notificationWindow.CancelClicked += (s, args) => {
                // If needed, handle cancel action
            };
            
            MainContent.Content = notificationWindow;
        }

        private void UC_VicePresident_Logout_Checked(object sender, RoutedEventArgs e)
        {
            LoginAccount loginAccount = new LoginAccount();
            loginAccount.Show();
            this.Close();
        }

        private void UC_VicePresident_Report_Checked(object sender, RoutedEventArgs e)
        {
        }
    }
}
