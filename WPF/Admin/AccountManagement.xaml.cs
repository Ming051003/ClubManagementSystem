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

        private void LoadData()
        {
            dgAccount.ItemsSource = null;
            var data = _accountService.GetAll().Select(a => new UserView
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
                Status = (bool)a.Status
            }).ToList();
            dgAccount.ItemsSource = data;
        }

        private void LoadClub()
        {
            var club = _accountService.GetAllClubs();
            cboClub.ItemsSource = club;
            cboClub.DisplayMemberPath = "ClubName";
            cboClub.SelectedValuePath = "ClubId";
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
            }
        }
    }
}
