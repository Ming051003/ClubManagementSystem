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

namespace WPF.President
{
    /// <summary>
    /// Interaction logic for AccountManagementByPresident.xaml
    /// </summary>
    public partial class AccountManagementByPresident : UserControl
    {
        private IAccountService _accountService;
        public AccountManagementByPresident()
        {
            InitializeComponent();
            _accountService = ((App)Application.Current).ServiceProvider.GetRequiredService<IAccountService>()
               ?? throw new ArgumentNullException(nameof(AccountService));
            LoadData();
            //LoadClub();
            LoadRole();
        }

        private void LoadData(string searchText = "", string selectedRole = "", int? selectedClubId = null)
        {
            var data = _accountService.GetAll()
                     .Where(a =>
                         (string.IsNullOrEmpty(searchText) || a.UserName.Contains(searchText) || a.FullName.Contains(searchText)) &&
                         (string.IsNullOrEmpty(selectedRole) || a.Role == selectedRole) &&
                         (!selectedClubId.HasValue || a.ClubId == selectedClubId)
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
            string searchText = tbSearch.Text.Trim();
            string selectedRole = cboRole.SelectedValue as string;

            LoadData(searchText, selectedRole);
            ClearFilter();
        }

        private void ClearFilter()
        {
            cboRole1.SelectedValue = -1;

        }
        //private void LoadClub()
        //{
        //    var club = _accountService.GetAllClubs();
        //    cboClub.ItemsSource = club;
        //    cboClub.DisplayMemberPath = "ClubName";
        //    cboClub.SelectedValuePath = "ClubId";

        //    cboClub1.ItemsSource = club;
        //    cboClub1.DisplayMemberPath = "ClubName";
        //    cboClub1.SelectedValuePath = "ClubId";
        //}

        private void LoadRole()
        {
            var roles = _accountService.GetAll()
                .Where(a => a.Role == "Member" || a.Role == "TeamLeader" || a.Role == "VicePresident" || a.Role == "President" || a.Role == "Admin")
                .Select(a => new
                {
                    a.Role
                })
                .Distinct()
                .ToList();

            cboRole.ItemsSource = roles;
            cboRole.DisplayMemberPath = "Role";
            cboRole.SelectedValuePath = "Role";
            cboRole1.ItemsSource = roles;
            cboRole1.DisplayMemberPath = "Role";
            cboRole1.SelectedValuePath = "Role";
        }

        private void dgAccount_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            var selectAccount = dgAccount.SelectedItem as UserView;
            if (selectAccount != null)
            {
                txtRollNumber.Text = selectAccount.StudentId;
                txtUsername.Text = selectAccount.UserName;
                txtPassword.Text = selectAccount.Password;
                txtEmail.Text = selectAccount.Email;
                txtFullName.Text = selectAccount.FullName;
                cboRole.SelectedValue = selectAccount.Role;
                //cboClub.SelectedValue = selectAccount.ClubId;

                if (selectAccount.Status == "Active")
                {
                    rbActive.IsChecked = true;
                    rbInactive.IsChecked = false;
                }
                else
                {
                    rbActive.IsChecked = false;
                    rbInactive.IsChecked = true;
                }
            }
        }



        private void ComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            //string searchText = tbSearch.Text.Trim();
            string selectedRole = cboRole1.SelectedValue as string;
            //int? selectedClubId = cboClub1.SelectedValue as int?;

            LoadData("", selectedRole);
        }



        private void ButtonAdd_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (!ValidateForm())
                {
                    return;
                }

                // Lấy dữ liệu từ form
                string studentId = txtRollNumber.Text.Trim();
                string username = txtUsername.Text.Trim();
                string password = txtPassword.Text.Trim();
                string fullName = txtFullName.Text.Trim();
                string email = txtEmail.Text.Trim();
                string role = cboRole.SelectedValue as string;
                //int clubId = (int)cboClub.SelectedValue;
                bool isActive = rbActive.IsChecked == true;

                if (_accountService.GetAll().Any(u => u.UserName == username))
                {
                    MessageBox.Show("Username already exists. Please choose another one.", "Validation Error", MessageBoxButton.OK, MessageBoxImage.Error);
                    return;
                }
                if (_accountService.GetAll().Any(u => u.StudentId == studentId))
                {
                    MessageBox.Show("A user with the same Roll number and Club already exists in this organization.", "Validation Error", MessageBoxButton.OK, MessageBoxImage.Error);
                    return;
                }

                User user = new User
                {
                    StudentId = studentId,
                    UserName = username,
                    Password = password,
                    FullName = fullName,
                    Email = email,
                    Role = role,
                    //ClubId = clubId,
                    Status = isActive,
                    JoinDate = DateOnly.FromDateTime(DateTime.Now),
                };
                _accountService.AddAccount(user);
                MessageBox.Show("Thêm món thành công!", "Information Message", MessageBoxButton.OK, MessageBoxImage.Information);
                LoadData();

            }
            catch (Exception ex)
            {
                MessageBox.Show("Có lỗi xảy ra: " + ex.Message, "Error Message", MessageBoxButton.OK, MessageBoxImage.Error);
            }

        }

        private void ButtonEdit_Click(object sender, RoutedEventArgs e)
        {
            var selectedAccount = dgAccount.SelectedItem as UserView;
            if (selectedAccount == null) return;
            if (!ValidateForm()) return;

            // Chuyển dữ liệu từ UserView sang User
            var updatedAccount = new User
            {
                UserId = selectedAccount.UserId,
                StudentId = txtRollNumber.Text.Trim(),
                UserName = txtUsername.Text.Trim(),
                FullName = txtFullName.Text.Trim(),
                Email = txtEmail.Text.Trim(),
                Password = txtPassword.Text.Trim(),
                Role = cboRole.SelectedValue as string,
                //ClubId = (int)cboClub.SelectedValue,
                Status = rbActive.IsChecked == true ? true : false,
                JoinDate = DateOnly.FromDateTime(DateTime.Now),
            };

            try
            {
                _accountService.UpdateAccount(updatedAccount);
                MessageBox.Show("User updated successfully.", "Success", MessageBoxButton.OK, MessageBoxImage.Information);
                LoadData();
                ClearForm();

            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error: {ex.Message}", "Update Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }

        }


        private void ButtonDel_Click(object sender, RoutedEventArgs e)
        {
            var selectedAccount = dgAccount.SelectedItem as UserView;
            if (selectedAccount == null)
            {
                MessageBox.Show("Please select a user to delete!", "Delete Error", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            var result = MessageBox.Show("Are you sure you want to delete this user?", "Confirm Delete", MessageBoxButton.YesNo, MessageBoxImage.Warning);
            if (result == MessageBoxResult.Yes)
            {
                try
                {
                    _accountService.DeleteAccount(selectedAccount.UserId);
                    MessageBox.Show("User deleted successfully!", "Success", MessageBoxButton.OK, MessageBoxImage.Information);
                    LoadData();
                    ClearForm();

                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Error: {ex.Message}", "Delete Error", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
        }

        private bool ValidateForm()
        {
            string username = txtUsername.Text.Trim();
            string studentId = txtRollNumber.Text.Trim();
            //int clubId = (int)cboClub.SelectedValue;
            if (string.IsNullOrWhiteSpace(username))
            {
                MessageBox.Show("Username cannot be empty.", "Validation Error", MessageBoxButton.OK, MessageBoxImage.Error);
                return false;
            }

            if (string.IsNullOrWhiteSpace(studentId))
            {
                MessageBox.Show("Roll number cannot be empty.", "Validation Error", MessageBoxButton.OK, MessageBoxImage.Error);
                return false;
            }

            if (string.IsNullOrWhiteSpace(txtFullName.Text))
            {
                MessageBox.Show("Full Name cannot be empty.", "Validation Error", MessageBoxButton.OK, MessageBoxImage.Error);
                return false;
            }

            if (string.IsNullOrWhiteSpace(txtEmail.Text) || !IsValidEmail(txtEmail.Text))
            {
                MessageBox.Show("Please enter a valid email address.", "Validation Error", MessageBoxButton.OK, MessageBoxImage.Error);
                return false;
            }

            if (string.IsNullOrWhiteSpace(txtPassword.Text) || txtPassword.Text.Length < 6)
            {
                MessageBox.Show("Password must be at least 6 characters long.", "Validation Error", MessageBoxButton.OK, MessageBoxImage.Error);
                return false;
            }

            if (cboRole.SelectedValue == null)
            {
                MessageBox.Show("Please select a Role.", "Validation Error", MessageBoxButton.OK, MessageBoxImage.Error);
                return false;
            }

            //if (cboClub.SelectedValue == null)
            //{
            //    MessageBox.Show("Please select a Club.", "Validation Error", MessageBoxButton.OK, MessageBoxImage.Error);
            //    return false;
            //}

            if (!rbActive.IsChecked.HasValue && !rbInactive.IsChecked.HasValue)
            {
                MessageBox.Show("Please select the Status.", "Validation Error", MessageBoxButton.OK, MessageBoxImage.Error);
                return false;
            }

            return true;
        }

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
            txtRollNumber.Text = string.Empty;
            txtUsername.Text = string.Empty;
            txtPassword.Text = string.Empty;
            txtFullName.Text = string.Empty;
            txtEmail.Text = string.Empty;
            cboRole.SelectedValue = -1;
            //cboClub.SelectedValue = -1;
            rbActive.IsChecked = true;
        }
    }
}
