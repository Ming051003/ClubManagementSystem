using BLL.BusinessInterfaces;
using BLL.BusinessService;
using Microsoft.Extensions.DependencyInjection;
using Model.Models;
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
using static System.Windows.Forms.VisualStyles.VisualStyleElement.StartPanel;

namespace WPF
{
    /// <summary>
    /// Interaction logic for CreateAccount.xaml
    /// </summary>
    public partial class CreateAccount : Window
    {
        private readonly IAccountService _accountService;

        public CreateAccount()
        {
            InitializeComponent();
            _accountService = ((App)Application.Current).ServiceProvider.GetRequiredService<IAccountService>()
               ?? throw new ArgumentNullException(nameof(AccountService));
            LoadClub();
        }

        private void btnExit_Click(object sender, RoutedEventArgs e)
        {
            MessageBoxResult result = MessageBox.Show("Are you sure you want to exit?", "Exit", MessageBoxButton.YesNo, MessageBoxImage.Question);
            if (result == MessageBoxResult.Yes)
            {
                this.Close();
            }
        }
        private void LoadClub()
        {
            var club = _accountService.GetAllClubs();
            cboClub.ItemsSource = club;
            cboClub.DisplayMemberPath = "ClubName";
            cboClub.SelectedValuePath = "ClubId";
        }
        private void btnRegister_Click(object sender, RoutedEventArgs e)
        {
            string username = txtUsername.Text;
            string password = txtPassword.Text;
            string studentId = txtStudentID.Text;
            string fullName = txtFullName.Text;
            string email = txtEmail.Text;
            string role = "Member";

            
            if(string.IsNullOrEmpty(username))
            {
                MessageBox.Show("Username is required.", "Validation Error", MessageBoxButton.OK, MessageBoxImage.Error);
                return; 
            }
            if(username.Length <6 || username.Length >50 )
            {
                MessageBox.Show("Username must be between 3 and 50 characters long.", "Validation Error", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            if (string.IsNullOrWhiteSpace(password))
            {
                MessageBox.Show("Password is required.", "Validation Error", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            if (string.IsNullOrWhiteSpace(studentId))
            {
                MessageBox.Show("Student ID is required.", "Validation Error", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            if (string.IsNullOrEmpty(email))
            {
                MessageBox.Show("Username is required.", "Validation Error", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            if (string.IsNullOrWhiteSpace(fullName))
            {
                MessageBox.Show("Full Name is required.", "Validation Error", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            if (cboClub.SelectedValue == null)
            {
                MessageBox.Show("Please select a club.", "Validation Error", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }
            // Ép kiểu SelectedValue sang int (nếu SelectedValue là kiểu số)
            if (!int.TryParse(cboClub.SelectedValue.ToString(), out int clubId))
            {
                MessageBox.Show("Invalid club selection.", "Validation Error", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            // Validate: Kiểm tra định dạng email hợp lệ
            if (!IsValidEmail(email))
            {
                MessageBox.Show("Please enter a valid email address.", "Validation Error", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            // Validate: Kiểm tra mật khẩu có đủ độ dài (ví dụ: ít nhất 6 ký tự)
            if (password.Length < 6)
            {
                MessageBox.Show("Password must be at least 6 characters long.", "Validation Error", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            User user = new User { 
                UserName = username,
                Email = email,
                Password = password,
                StudentId = studentId,
                FullName = fullName,
                Role = role,
                ClubId = clubId,
                JoinDate = DateOnly.FromDateTime(DateTime.Now),
                Status = false
            };
            try
            {
                _accountService.AddAccount(user);
                MessageBox.Show("User registered successfully!", "Success", MessageBoxButton.OK, MessageBoxImage.Information);

                ClearForm();
            }
            catch (Exception ex) {
                MessageBox.Show($"An error occurred: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }


        }
        // Hàm kiểm tra định dạng email hợp lệ
        private bool IsValidEmail(string email)
        {
            try
            {
                var addr = new System.Net.Mail.MailAddress(email);
                return addr.Address == email;
            }
            catch
            {
                return false;
            }
        }
        private void ClearForm()
        {
            txtUsername.Clear();
            txtPassword.Clear();
            txtStudentID.Clear();
            txtFullName.Clear();
            cboClub.SelectedIndex = -1; 
        }
        private void btnlogin_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                LoginAccount loginAccount = new LoginAccount();
                loginAccount.Show();
                this.Close();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }
    }
}
