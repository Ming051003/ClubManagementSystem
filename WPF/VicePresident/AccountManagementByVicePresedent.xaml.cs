using BLL.BusinessInterfaces;
using BLL.BusinessService;
using Microsoft.Extensions.DependencyInjection;
using Model.Models;
using System;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace WPF.VicePresident
{
    /// <summary>
    /// Interaction logic for AccountManagementByVicePresedent.xaml
    /// </summary>
    public partial class AccountManagementByVicePresedent : UserControl
    {
        private IAccountService _accountService;
        public AccountManagementByVicePresedent()
        {
            InitializeComponent();
            _accountService = ((App)Application.Current).ServiceProvider.GetRequiredService<IAccountService>()
               ?? throw new ArgumentNullException(nameof(AccountService));
            LoadData();
            LoadRole();
        }

        private void LoadData(string searchByName = "", string searchByRoll = "", string selectedRole = "")
        {
            string username = User.Current?.UserName;

            if (string.IsNullOrEmpty(username))
            {
                MessageBox.Show("User is not logged in.", "Login Required", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            int? clubIdFromCurrentUser = _accountService.GetClubIdByUsername(username);

            if (clubIdFromCurrentUser == null)
            {
                MessageBox.Show("No club found for the current user.", "Club Not Found", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            var data = _accountService.GetAll()
                     .Where(a =>
                         (string.IsNullOrEmpty(searchByName) || a.FullName.ToLower().Contains(searchByName.ToLower())) &&
                         (string.IsNullOrEmpty(searchByRoll) || a.StudentId.ToLower().Contains(searchByRoll.ToLower())) &&
                         (string.IsNullOrEmpty(selectedRole) || a.Role == selectedRole) &&
                         (a.ClubId == clubIdFromCurrentUser)
                     )
                     .Select(a => new UserView
                     {
                         UserId = a.UserId,
                         StudentId = a.StudentId,
                         UserName = a.UserName,
                         FullName = a.FullName,
                         Email = a.Email,
                         Password = a.Password,
                         Role = a.Role,
                         ClubId = a.ClubId,
                         ClubName = a.Club != null ? a.Club.ClubName : null,
                         JoinDate = (DateOnly)a.JoinDate,
                         Status = a.Status.HasValue && a.Status.Value ? "Active" : "Inactive"
                     }).ToList();

            dgAccount.ItemsSource = data;
        }

        private void ApplyFilter_Click(object sender, RoutedEventArgs e)
        {
            string searchByName = tbSearchName.Text.Trim();
            string searchByRoll = tbSearchRoll.Text.Trim();
            string selectedRole = cboRole1.SelectedValue as string;

            LoadData(searchByName, searchByRoll, selectedRole);
            ClearFilter();
        }

        private void ClearFilter()
        {
            cboRole1.SelectedValue = -1;
            tbSearchName.Text = string.Empty;
            tbSearchRoll.Text = string.Empty;
        }

        private void LoadRole()
        {
            var roles = new List<object>
            {
                new { Role = "All Members" }
            };

            var roleData = _accountService.GetAll()
                .Where(a => a.Role == "Member" || a.Role == "TeamLeader" || a.Role == "VicePresident")
                .Select(a => new { a.Role }).Distinct().ToList();

            foreach (var role in roleData)
            {
                roles.Add(role);
            }

            cboRole1.ItemsSource = roles;
            cboRole1.SelectedIndex = 0; // Set "All Members" as default
            
            cboRole.ItemsSource = roles.Where(r => (r as dynamic).Role != "All Members").ToList();
        }

        private void dgAccount_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (dgAccount.SelectedItem != null)
            {
                UserView selectedUser = (UserView)dgAccount.SelectedItem;
                txtRollNumber.Text = selectedUser.StudentId;
                txtUsername.Text = selectedUser.UserName;
                txtPassword.Password = selectedUser.Password;
                txtEmail.Text = selectedUser.Email;
                txtFullName.Text = selectedUser.FullName;
                cboRole.SelectedValue = selectedUser.Role;
            }
        }

        private void ComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            string searchByName = tbSearchName.Text.Trim();
            string searchByRoll = tbSearchRoll.Text.Trim();
            string selectedRole = cboRole1.SelectedValue as string;

            // If "All Members" is selected, pass empty string to LoadData
            if (selectedRole == "All Members")
            {
                selectedRole = "";
            }

            LoadData(searchByName, searchByRoll, selectedRole);
        }

        private void ButtonAdd_Click(object sender, RoutedEventArgs e)
        {
            if (ValidateForm())
            {
                string username = txtUsername.Text.Trim();
                string password = txtPassword.Password.Trim();
                string email = txtEmail.Text.Trim();
                string fullName = txtFullName.Text.Trim();
                string studentId = txtRollNumber.Text.Trim();
                string role = cboRole.SelectedValue as string;

                // Get the current user's club ID
                string currentUsername = User.Current?.UserName;
                int? clubId = _accountService.GetClubIdByUsername(currentUsername);

                if (clubId == null)
                {
                    MessageBox.Show("No club found for the current user.", "Club Not Found", MessageBoxButton.OK, MessageBoxImage.Warning);
                    return;
                }

                try
                {
                    // Create a new User object
                    var account = new User
                    {
                        UserName = username,
                        Password = password,
                        Email = email,
                        FullName = fullName,
                        StudentId = studentId,
                        Role = role,
                        ClubId = clubId,
                        Status = true,
                        JoinDate = DateOnly.FromDateTime(DateTime.Now)
                    };

                    _accountService.AddAccount(account);
                    MessageBox.Show("Account added successfully!", "Success", MessageBoxButton.OK, MessageBoxImage.Information);
                    ClearForm();
                    LoadData();
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Error: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
        }

        private void ButtonEdit_Click(object sender, RoutedEventArgs e)
        {
            if (dgAccount.SelectedItem == null)
            {
                MessageBox.Show("Please select an account to edit.", "No Selection", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            if (ValidateForm())
            {
                UserView selectedUser = (UserView)dgAccount.SelectedItem;
                int userId = selectedUser.UserId;
                string username = txtUsername.Text.Trim();
                string password = txtPassword.Password.Trim();
                string email = txtEmail.Text.Trim();
                string fullName = txtFullName.Text.Trim();
                string studentId = txtRollNumber.Text.Trim();
                string role = cboRole.SelectedValue as string;

                // Get the current user's club ID
                string currentUsername = User.Current?.UserName;
                int? clubId = _accountService.GetClubIdByUsername(currentUsername);

                if (clubId == null)
                {
                    MessageBox.Show("No club found for the current user.", "Club Not Found", MessageBoxButton.OK, MessageBoxImage.Warning);
                    return;
                }

                try
                {
                    // Create a User object for update
                    var account = new User
                    {
                        UserId = userId,
                        UserName = username,
                        Password = password,
                        Email = email,
                        FullName = fullName,
                        StudentId = studentId,
                        Role = role,
                        ClubId = clubId,
                        Status = true,
                        JoinDate = DateOnly.FromDateTime(DateTime.Now)
                    };

                    _accountService.UpdateAccount(account);
                    MessageBox.Show("Account updated successfully!", "Success", MessageBoxButton.OK, MessageBoxImage.Information);
                    ClearForm();
                    LoadData();
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Error: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
        }

        private void ButtonClear_Click(object sender, RoutedEventArgs e)
        {
            ClearForm();
        }

        private bool ValidateForm()
        {
            string username = txtUsername.Text.Trim();
            string password = txtPassword.Password.Trim();
            string email = txtEmail.Text.Trim();
            string fullName = txtFullName.Text.Trim();
            string studentId = txtRollNumber.Text.Trim();
            string role = cboRole.SelectedValue as string;

            if (string.IsNullOrEmpty(username))
            {
                MessageBox.Show("Username is required.", "Validation Error", MessageBoxButton.OK, MessageBoxImage.Warning);
                return false;
            }

            if (string.IsNullOrEmpty(password))
            {
                MessageBox.Show("Password is required.", "Validation Error", MessageBoxButton.OK, MessageBoxImage.Warning);
                return false;
            }

            if (string.IsNullOrEmpty(email))
            {
                MessageBox.Show("Email is required.", "Validation Error", MessageBoxButton.OK, MessageBoxImage.Warning);
                return false;
            }

            if (!email.Contains("@") || !email.Contains("."))
            {
                MessageBox.Show("Please enter a valid email address.", "Validation Error", MessageBoxButton.OK, MessageBoxImage.Warning);
                return false;
            }

            if (string.IsNullOrEmpty(fullName))
            {
                MessageBox.Show("Full Name is required.", "Validation Error", MessageBoxButton.OK, MessageBoxImage.Warning);
                return false;
            }

            if (string.IsNullOrEmpty(studentId))
            {
                MessageBox.Show("Roll Number is required.", "Validation Error", MessageBoxButton.OK, MessageBoxImage.Warning);
                return false;
            }

            if (string.IsNullOrEmpty(role))
            {
                MessageBox.Show("Role is required.", "Validation Error", MessageBoxButton.OK, MessageBoxImage.Warning);
                return false;
            }

            return true;
        }

        private void ClearForm()
        {
            txtRollNumber.Text = string.Empty;
            txtUsername.Text = string.Empty;
            txtPassword.Password = string.Empty;
            txtFullName.Text = string.Empty;
            txtEmail.Text = string.Empty;
            cboRole.SelectedValue = -1;
        }

        private void SearchBox_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                ApplyFilter_Click(sender, e);
            }
        }
    }
}
