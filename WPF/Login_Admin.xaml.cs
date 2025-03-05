using BusinessObjects.Models;
using Microsoft.Extensions.DependencyInjection;
using Services.BusinessService;
using Services.Interface;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace WpfApp1
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class Login_Admin : Window
    {
        private readonly IAdminService adminService;
        public Login_Admin()
        {
            InitializeComponent();
            adminService = ((App)Application.Current).ServiceProvider.GetRequiredService<IAdminService>() ?? throw new ArgumentNullException(nameof(AdminService));
        }

        private async void btnLogin_Click(object sender, RoutedEventArgs e)
        {
            if(txtUsername.Text == null || txtPassword.Text == null)
            {
                MessageBox.Show("Incorrect username/password", "Error Message", MessageBoxButton.OK, MessageBoxImage.Error);
            }
            else
            {
                Admin? admin = await adminService.CheckLogin(txtUsername.Text, txtPassword.Text);
                if (admin != null) {
                    MessageBox.Show("Login Successfully!", "Information Message", MessageBoxButton.OK, MessageBoxImage.Information);

                }
            }
        }
        private void Border_MouseDown(object sender, MouseEventArgs e) {
            if (e.LeftButton == MouseButtonState.Pressed)
            {
                DragMove();
            }
        }
        private void btnExit_Click(object sender, RoutedEventArgs e)
        {
            MessageBoxResult result = MessageBox.Show("Are you sure you want to exit?", "Exit", MessageBoxButton.YesNo, MessageBoxImage.Question);
            if (result == MessageBoxResult.Yes) { 
                this.Close();
            }
        }
    }
}