using System.Windows;
using System.Windows.Input;
using WPF.Admin;
using WPF.VicePresident;

namespace WPF
{
    /// <summary>
    /// Interaction logic for Main_Admin_WPF.xaml
    /// </summary>
    public partial class Main_Admin_WPF : Window
    {
        public Main_Admin_WPF()
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
            {
                this.WindowState = WindowState.Maximized;
            }
            else
            {
                this.WindowState = WindowState.Normal;
            }
        }
        private void UC_Admin_Member_Checked(object sender, RoutedEventArgs e)
        {
            MainContent.Content = new AccountManagement();
        }

        private void UC_Admin_Club_Checked(object sender, RoutedEventArgs e)
        {
            MainContent.Content = new ClubManagement();
        }

        private void UC_Admin_Event_Checked(object sender, RoutedEventArgs e)
        {
            MainContent.Content = new EventManagement();
        }

        private void UC_Admin_EventParticipant_Checked(object sender, RoutedEventArgs e)
        {
            MainContent.Content = new EventParticipantManagement();
        }

        private void UC_Admin_Notification_Checked(object sender, RoutedEventArgs e)
        {

        }

        private void UC_Admin_Report_Checked(object sender, RoutedEventArgs e)
        {

        }

        private void UC_Admin_Logout_Checked(object sender, RoutedEventArgs e)
        {
            LoginAccount loginAccount = new LoginAccount();
            loginAccount.Show();
            this.Close();
        }


    }
}
