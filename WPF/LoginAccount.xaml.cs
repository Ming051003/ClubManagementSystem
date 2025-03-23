using BLL.BusinessInterfaces;
using BLL.BusinessService;
using Microsoft.Extensions.DependencyInjection;
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
    /// Interaction logic for LoginAccount.xaml
    /// </summary>
    public partial class LoginAccount : Window
    {
        private readonly IAccountService _accountService;
        public LoginAccount()
        {
            InitializeComponent();
            _accountService = ((App)Application.Current).ServiceProvider.GetRequiredService<IAccountService>() 
                ?? throw new ArgumentNullException(nameof(AccountService));

        }
        private void Window_KeyDown(object sender, KeyEventArgs e)
        {
            // Kiểm tra nếu người dùng nhấn phím Enter
            if (e.Key == Key.Enter)
            {
                // Gọi sự kiện btnLogin_Click
                btnLogin_Click(sender, e);
            }
        }
        private async void btnLogin_Click(object sender, RoutedEventArgs e)
        {
            string ussername = txtUsername.Text;
            string password = txtPassword.Text;


            if (txtUsername.Text == null || txtPassword.Text == null)
            {
                MessageBox.Show("Incorrect username/password", "Error Message", MessageBoxButton.OK, MessageBoxImage.Error);
            }
            else
            {
                var result = _accountService.Login(ussername, password);
                if (result != null) {
                    if (result.Status == true && result.Role == "Admin")
                    {
                        Main_Admin_WPF main_Admin_WPF = new Main_Admin_WPF();
                        main_Admin_WPF.Show();
                        MessageBox.Show("Login Successfully!", "Information Message", MessageBoxButton.OK, MessageBoxImage.Information);
                        this.Close();
                    }
                    else if (result.Status == true && result.Role == "President")
                    {
                        Main_President_WPF main_President_WPF = new Main_President_WPF();
                        main_President_WPF.Show();
                        MessageBox.Show("Login Successfully!", "Information Message", MessageBoxButton.OK, MessageBoxImage.Information);
                        this.Close();
                    }
                    else if (result.Status == true && result.Role == "VicePresident")
                    {
                        Main_VicePresident_WPF main_VicePresident_WPF = new();
                        main_VicePresident_WPF.Show();
                        MessageBox.Show("Login Successfully!", "Information Message", MessageBoxButton.OK, MessageBoxImage.Information);
                        this.Close();
                    }
                    else
                    {
                        MessageBox.Show("Your account is not active. Please contact the administrator.", "Error Message", MessageBoxButton.OK, MessageBoxImage.Error);
                    }
                }
                else
                {
                    MessageBox.Show("Invalid email or password!", "Error Message", MessageBoxButton.OK, MessageBoxImage.Error);
                }

            }
        }
        private void Border_MouseDown(object sender, MouseEventArgs e)
        {
            //if (e.LeftButton == MouseButtonState.Pressed)
            //{
            //    DragMove();
            //}
        }
        private void btnExit_Click(object sender, RoutedEventArgs e)
        {
            MessageBoxResult result = MessageBox.Show("Are you sure you want to exit?", "Exit", MessageBoxButton.YesNo, MessageBoxImage.Question);
            if (result == MessageBoxResult.Yes)
            {
                this.Close();
            }
        }

        // Sự kiện khi nhấn nút "Quên mật khẩu"
        private void ChangePassword_Click(object sender, RoutedEventArgs e)
        {
            // Thực hiện hành động quên mật khẩu, ví dụ mở cửa sổ quên mật khẩu
            MessageBox.Show("Redirecting to Forgot Password page");

            // Ví dụ: Mở cửa sổ "Quên mật khẩu"
            // ForgotPasswordWindow forgotPasswordWindow = new ForgotPasswordWindow();
            // forgotPasswordWindow.Show();
            // this.Close();
        }

        // Sự kiện khi nhấn nút "Tạo tài khoản"
        private void CreateAccount_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                // Tạo một cửa sổ đăng ký mới
                CreateAccount registrationWindow = new CreateAccount();

                // Hiển thị cửa sổ đăng ký
                registrationWindow.Show();

                // Đóng cửa sổ đăng nhập (LoginWindow)
                this.Close();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

    }
}
