﻿using System;
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

        //private void UC_Admin_Staff_Checked(object sender, RoutedEventArgs e)
        //{
        //    MainContent.Content = new AccountManagementByVicePresedent();
        //}

        private void UC_Admin_Event_Checked(object sender, RoutedEventArgs e)
        {
            MainContent.Content = new TeamManagementByVicePresident();
        }

        private void UC_Admin_Inventory_Checked(object sender, RoutedEventArgs e)
        {
            MainContent.Content = new EventManagement();
        }

        private void RadioButton_Checked(object sender, RoutedEventArgs e)
        {
            LoginAccount loginAccount = new LoginAccount();
            loginAccount.Show();
            this.Close();
        }

        private void UC_Admin_Invoice_Checked(object sender, RoutedEventArgs e)
        {
            MainContent.Content = new EventParticipantManagement();

        }

        private void UC_Admin_Worktime_Checked(object sender, RoutedEventArgs e)
        {

        }
    }
}
