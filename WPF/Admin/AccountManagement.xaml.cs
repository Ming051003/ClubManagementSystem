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
using System.Windows.Navigation;
using System.Windows.Shapes;

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
    }
}
