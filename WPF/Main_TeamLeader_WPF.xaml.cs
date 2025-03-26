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
using WPF.Admin;
using WPF.Member;
using WPF.TeamLeader;
using WPF.VicePresident;

namespace WPF
{
    /// <summary>
    /// Interaction logic for Main_TeamLeader_WPF.xaml
    /// </summary>
    public partial class Main_TeamLeader_WPF : Window
    {
        public Main_TeamLeader_WPF()
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

        private void UC_TeamLeader_Profile_Checked(object sender, RoutedEventArgs e)
        {
            MainContent.Content = new ProfileManagementByTeamLeader();
        }

        private void UC_TeamLeader_Team_Checked(object sender, RoutedEventArgs e)
        {
            MainContent.Content = new TeamManagementByTeamLeader();
        }

        private void UC_TeamLeader_Event_Checked(object sender, RoutedEventArgs e)
        {
            MainContent.Content = new EventEnrollmentByTeamLeader();

        }

        private void UC_TeamLeader_History_Checked(object sender, RoutedEventArgs e)
        {
            MainContent.Content = new EventHistoryByTeamLeader();

        }

        private void UC_TeamLeader_Notifcation_Checked(object sender, RoutedEventArgs e)
        {

        }

        private void UC_TeamLeader_Logout_Checked(object sender, RoutedEventArgs e)
        {
            LoginAccount loginAccount = new LoginAccount();
            loginAccount.Show();
            this.Close();
        }
    }
}
