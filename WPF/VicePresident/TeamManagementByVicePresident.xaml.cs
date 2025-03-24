using BLL.BusinessInterfaces;
using BLL.BusinessService;
using Microsoft.Extensions.DependencyInjection;
using Model.Models;
using Model.ViewModels;
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

namespace WPF.VicePresident
{
    /// <summary>
    /// Interaction logic for TeamManagementByVicePresident.xaml
    /// </summary>
    public partial class TeamManagementByVicePresident : UserControl
    {
        private ITeamService _teamService;
        private IAccountService _accountService;
        public TeamManagementByVicePresident()
        {
            InitializeComponent();
            _teamService = ((App)Application.Current).ServiceProvider.GetRequiredService<ITeamService>()
              ?? throw new ArgumentNullException(nameof(TeamService));
            _accountService = ((App)Application.Current).ServiceProvider.GetRequiredService<IAccountService>()
             ?? throw new ArgumentNullException(nameof(AccountService));
            LoadData();
            LoadLeaders();
        }
        private void LoadData(string searchText = "")
        {
            int clubId = User.Current.ClubId ?? 0;
            var teams = _teamService.GetTeamsByClubId(clubId).Where(a =>
                     (string.IsNullOrEmpty(searchText) || a.TeamName.Contains(searchText)))
                     .ToList();
            dgTeam.ItemsSource = teams;
        }
        private void LoadLeaders()
        {
            var leaders = _accountService.GetLeadersByClubId();

            // Gán dữ liệu vào ComboBox
            cboLeader.ItemsSource = leaders;
            cboLeader.DisplayMemberPath = "FullName"; // Hiển thị tên đầy đủ
            cboLeader.SelectedValuePath = "UserId"; // Lưu giá trị UserId khi chọn
        }

        private void ApplyFilter_Click(object sender, RoutedEventArgs e)
        {
            string searchText = tbSearch.Text.Trim();
            LoadData(searchText);
        }
        private void dgTeam_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            var selectTeam = dgTeam.SelectedItem as TeamView;
            if (selectTeam != null)
            {
                txtTeamID.Text = selectTeam.TeamId.ToString();
                txtTeamName.Text = selectTeam.TeamName;
                txtDescription.Text = selectTeam.Description;
                cboLeader.SelectedValue = selectTeam.LeaderId;

            }
        }

        private void ButtonEdit_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                var selectedTeam = dgTeam.SelectedItem as TeamView;
                if (selectedTeam == null)
                {
                    MessageBox.Show("Please select a team to edit.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                    return;
                }

                string teamName = txtTeamName.Text.Trim();
                string description = txtDescription.Text.Trim();
                int leaderUserId = (int)cboLeader.SelectedValue;
                int clubId = User.Current?.ClubId ?? 0;

                if (clubId == 0)
                {
                    MessageBox.Show("Club ID is not valid or the user is not associated with any club.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                    return;
                }

                var oldLeader = _accountService.GetAccountById(selectedTeam.LeaderId);
                if (oldLeader != null && oldLeader.Role == "TeamLeader")
                {
                    oldLeader.Role = "Member";
                    _accountService.UpdateAccountRoleOnly(oldLeader);
                }

                var newLeader = _accountService.GetAccountById(leaderUserId);
                if (newLeader == null)
                {
                    MessageBox.Show("Leader not found in the database.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                    return;
                }

                if (newLeader.Role != "TeamLeader")
                {
                    newLeader.Role = "TeamLeader";
                    _accountService.UpdateAccount(newLeader);
                }

                Team team = new Team
                {
                    TeamId = selectedTeam.TeamId,
                    TeamName = teamName,
                    Description = description,
                    ClubId = clubId
                };

                _teamService.UpdateTeam(team, leaderUserId);

                MessageBox.Show("Team updated successfully and leader role updated!", "Success", MessageBoxButton.OK, MessageBoxImage.Information);
                LoadData();
                ClearForm();
            }
            catch (Exception ex)
            {
                MessageBox.Show("An error occurred: " + ex.Message, "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }


        private void ClearForm()
        {
            txtTeamID.Text = string.Empty;
            txtTeamName.Text = string.Empty;
            txtDescription.Text = string.Empty;
            cboLeader.SelectedValue = -1;
        }
    }
}
