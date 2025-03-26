using BLL.BusinessInterfaces;
using Microsoft.Extensions.DependencyInjection;
using Model.Models;
using System;
using System.Windows;
using System.Windows.Controls;

namespace WPF.TeamLeader
{
    public partial class ProfileManagementByTeamLeader : UserControl
    {
        private readonly IAccountService _accountService;

        public ProfileManagementByTeamLeader()
        {
            InitializeComponent();
            _accountService = ((App)Application.Current).ServiceProvider.GetRequiredService<IAccountService>()
                ?? throw new ArgumentNullException(nameof(IAccountService));
            LoadUserProfile();
        }

        private void LoadUserProfile()
        {
            try
            {
                if (User.Current == null)
                {
                    MessageBox.Show("You are not logged in.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                    return;
                }

                var user = _accountService.GetAccountById(User.Current.UserId);
                if (user != null)
                {
                    txtFullName.Text = user.FullName;
                    txtStudentId.Text = user.StudentId;
                    txtUsername.Text = user.UserName;
                    txtEmail.Text = user.Email;
                    txtClub.Text = user.Club?.ClubName ?? "Not assigned";
                    txtJoinDate.Text = user.JoinDate?.ToString("dd/MM/yyyy") ?? "N/A";
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error loading profile: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void btnUpdatePassword_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (User.Current == null)
                {
                    MessageBox.Show("You are not logged in.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                    return;
                }

                string currentPassword = txtCurrentPassword.Password;
                string newPassword = txtNewPassword.Password;
                string confirmPassword = txtConfirmPassword.Password;

                if (string.IsNullOrWhiteSpace(currentPassword) || string.IsNullOrWhiteSpace(newPassword) || string.IsNullOrWhiteSpace(confirmPassword))
                {
                    MessageBox.Show("All password fields are required.", "Validation Error", MessageBoxButton.OK, MessageBoxImage.Warning);
                    return;
                }

                if (newPassword != confirmPassword)
                {
                    MessageBox.Show("New password and confirm password do not match.", "Validation Error", MessageBoxButton.OK, MessageBoxImage.Warning);
                    return;
                }

                var user = _accountService.GetAccountById(User.Current.UserId);
                if (user.Password != currentPassword)
                {
                    MessageBox.Show("Current password is incorrect.", "Validation Error", MessageBoxButton.OK, MessageBoxImage.Warning);
                    return;
                }

                user.Password = newPassword;
                _accountService.UpdateAccount(user);

                MessageBox.Show("Password updated successfully!", "Success", MessageBoxButton.OK, MessageBoxImage.Information);

                txtCurrentPassword.Password = "";
                txtNewPassword.Password = "";
                txtConfirmPassword.Password = "";
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error updating password: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void btnChangePassword_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (User.Current == null)
                {
                    MessageBox.Show("You are not logged in.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                    return;
                }

                if (string.IsNullOrEmpty(txtCurrentPassword.Password) ||
                    string.IsNullOrEmpty(txtNewPassword.Password) ||
                    string.IsNullOrEmpty(txtConfirmPassword.Password))
                {
                    MessageBox.Show("Please fill in all password fields.", "Error", MessageBoxButton.OK, MessageBoxImage.Warning);
                    return;
                }

                if (txtNewPassword.Password != txtConfirmPassword.Password)
                {
                    MessageBox.Show("New password and confirm password do not match.", "Error", MessageBoxButton.OK, MessageBoxImage.Warning);
                    return;
                }

                var success = _accountService.ChangePassword(User.Current.UserId, txtCurrentPassword.Password, txtNewPassword.Password);
                if (success)
                {
                    MessageBox.Show("Password changed successfully!", "Success", MessageBoxButton.OK, MessageBoxImage.Information);
                    txtCurrentPassword.Clear();
                    txtNewPassword.Clear();
                    txtConfirmPassword.Clear();
                }
                else
                {
                    MessageBox.Show("Failed to change password. Please check your current password.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error changing password: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }
    }
}
