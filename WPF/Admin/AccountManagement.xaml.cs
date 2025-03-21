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
using static System.Windows.Forms.VisualStyles.VisualStyleElement.StartPanel;

namespace WPF.Admin
{
    /// <summary>
    /// Interaction logic for AccountManagement.xaml
    /// </summary>
    public partial class AccountManagement : UserControl
    {
        private IAccountService _accountService;
        public AccountManagement()
        {
            InitializeComponent();
            _accountService = ((App)Application.Current).ServiceProvider.GetRequiredService<IAccountService>()
               ?? throw new ArgumentNullException(nameof(AccountService));
            LoadData();
            LoadClub();
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
            int? selectedClubId = cboClub.SelectedValue as int?;  

            LoadData(searchText, selectedRole, selectedClubId);
            ClearFilter();
        }

        private void ClearFilter()
        {
            cboClub1.SelectedValue = -1;
            cboRole1.SelectedValue = -1;

        }
        private void LoadClub()
        {
            var club = _accountService.GetAllClubs();
            cboClub.ItemsSource = club;
            cboClub.DisplayMemberPath = "ClubName";
            cboClub.SelectedValuePath = "ClubId";

            cboClub1.ItemsSource = club;
            cboClub1.DisplayMemberPath = "ClubName";
            cboClub1.SelectedValuePath = "ClubId";
        }

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
            if (selectAccount != null) {
                txtRollNumber.Text = selectAccount.StudentId;
                txtUsername.Text = selectAccount.UserName;
                txtPassword.Text = selectAccount.Password;
                txtEmail.Text = selectAccount.Email;
                txtFullName.Text = selectAccount.FullName;
                cboRole.SelectedValue = selectAccount.Role;
                cboClub.SelectedValue = selectAccount.ClubId;
                
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
            int? selectedClubId = cboClub1.SelectedValue as int?;

            LoadData("", selectedRole, selectedClubId);
        }

       

        private void ButtonAdd_Click(object sender, RoutedEventArgs e)
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
            int clubId = (int)cboClub.SelectedValue;
            bool isActive = rbActive.IsChecked == true;

            if (_accountService.GetAll().Any(u => u.UserName == username))
            {
                MessageBox.Show("Username already exists. Please choose another one.", "Validation Error", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }
            if (_accountService.GetAll().Any(u => u.StudentId == studentId && u.ClubId == clubId))
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
                ClubId = clubId,
                Status = isActive,
                JoinDate = DateOnly.FromDateTime(DateTime.Now),
            };
            _accountService.AddAccount(user);

            LoadData();
        }

        private void ButtonEdit_Click(object sender, RoutedEventArgs e)
        {
            var selectedAccount = dgAccount.SelectedValue as User;
            if (selectedAccount == null) return;
            if (!ValidateForm()) return;

            string studenId = txtRollNumber.Text.Trim();
            string userName = txtUsername.Text.Trim();
            string password = txtPassword.Text.Trim();
            string fullName = txtFullName.Text.Trim();
            string email = txtEmail.Text.Trim();
            string role = cboRole.SelectedValue as string;
            int clubId = (int)cboClub.SelectedValue;
            bool isActive = rbActive.IsChecked == true;

            // Cập nhật thông tin người dùng
            selectedAccount.UserName = userName;
            selectedAccount.Password = password;
            selectedAccount.FullName = fullName;
            selectedAccount.Email = email;
            selectedAccount.Role = role;
            selectedAccount.ClubId = clubId;
            selectedAccount.Status = isActive;

            _accountService.UpdateAccount(selectedAccount);
            LoadData();

        }


        private void ButtonDel_Click(object sender, RoutedEventArgs e)
        {

        }

        private bool ValidateForm()
        {
            string username = txtUsername.Text.Trim();
            string studentId = txtRollNumber.Text.Trim();
            int clubId = (int)cboClub.SelectedValue;
            if (string.IsNullOrWhiteSpace(username))
            {
                MessageBox.Show("Username cannot be empty.", "Validation Error", MessageBoxButton.OK, MessageBoxImage.Error);
                return false;
            }
           

            if (string.IsNullOrWhiteSpace(studentId)) {
                MessageBox.Show("Roll number cannot be empty.", "Validation Error", MessageBoxButton.OK, MessageBoxImage.Error);
                return false;
            }
            //if (_accountService.GetAll().Any(u => u.StudentId == studentId))
            //{
            //    MessageBox.Show("Roll number already exists. Please choose another one.", "Validation Error", MessageBoxButton.OK, MessageBoxImage.Error);
            //    return false;
            //}
            


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

            if (cboClub.SelectedValue == null)
            {
                MessageBox.Show("Please select a Club.", "Validation Error", MessageBoxButton.OK, MessageBoxImage.Error);
                return false;
            }

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

    }
}
