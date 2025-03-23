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

namespace WPF
{
    /// <summary>
    /// Interaction logic for Main_VicePresident_WPF.xaml
    /// </summary>
    public partial class Main_VicePresident_WPF : Window
    {
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
            {
                this.WindowState = WindowState.Maximized;
            }
            else
            {
                this.WindowState = WindowState.Normal;
            }
        }

        private void UC_Admin_Staff_Checked(object sender, RoutedEventArgs e)
        {

        }

        private void UC_Admin_Event_Checked(object sender, RoutedEventArgs e)
        {

        }

        private void UC_Admin_Inventory_Checked(object sender, RoutedEventArgs e)
        {

        }

        private void UC_Admin_Invoice_Checked(object sender, RoutedEventArgs e)
        {

        }

        private void UC_Admin_Worktime_Checked(object sender, RoutedEventArgs e)
        {

        }

        private void UC_Admin_WorkDay_Checked(object sender, RoutedEventArgs e)
        {

        }
    }
}
