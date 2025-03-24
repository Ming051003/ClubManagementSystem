using BLL.BusinessInterfaces;
using BLL.BusinessService;
using Microsoft.Extensions.DependencyInjection;
using Model.Models;
using Model.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;

namespace WPF.President
{
    /// <summary>
    /// Interaction logic for TeamManagementByPresident.xaml
    /// </summary>
    public partial class TeamManagementByPresident : UserControl
    {
        private readonly ITeamService _teamService;
        private readonly IAccountService _accountService;
        private readonly ITeamMemberService _teamMemberService;
        private int _selectedTeamId;

        public TeamManagementByPresident()
        {
            InitializeComponent();
            _teamService = ((App)Application.Current).ServiceProvider.GetRequiredService<ITeamService>()
              ?? throw new ArgumentNullException(nameof(TeamService));
            _accountService = ((App)Application.Current).ServiceProvider.GetRequiredService<IAccountService>()
             ?? throw new ArgumentNullException(nameof(AccountService));
            _teamMemberService = ((App)Application.Current).ServiceProvider.GetRequiredService<ITeamMemberService>()
             ?? throw new ArgumentNullException(nameof(ITeamMemberService));
            
            LoadTeams();
            LoadLeaders();
            ClearTeamForm();
        }

        private void LoadTeams(string searchText = "")
        {
            try
            {
                int clubId = User.Current?.ClubId ?? 0;
                if (clubId == 0)
                {
                    MessageBox.Show("You are not associated with any club.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                    return;
                }

                var teams = _teamService.GetTeamsByClubId(clubId)
                    .Where(a => string.IsNullOrEmpty(searchText) || a.TeamName.Contains(searchText))
                    .ToList();
                
                dgTeam.ItemsSource = teams;
                
                // Clear team members if no team is selected
                if (_selectedTeamId == 0)
                {
                    dgTeamMembers.ItemsSource = null;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error loading teams: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void LoadLeaders()
        {
            try
            {
                var leaders = _accountService.GetLeadersByClubId();
                cboLeader.ItemsSource = leaders;
                cboLeader.DisplayMemberPath = "FullName";
                cboLeader.SelectedValuePath = "UserId";
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error loading leaders: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void btnApplyFilter_Click(object sender, RoutedEventArgs e)
        {
            string searchText = tbSearch.Text.Trim();
            LoadTeams(searchText);
        }

        private void btnAddTeam_Click(object sender, RoutedEventArgs e)
        {
            ClearTeamForm();
            txtTeamName.Focus();
        }

        private void btnSaveTeam_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                // Validate inputs
                string teamName = txtTeamName.Text.Trim();
                if (string.IsNullOrEmpty(teamName))
                {
                    MessageBox.Show("Team name is required.", "Validation Error", MessageBoxButton.OK, MessageBoxImage.Warning);
                    txtTeamName.Focus();
                    return;
                }

                string description = txtDescription.Text.Trim();
                
                if (cboLeader.SelectedItem == null)
                {
                    MessageBox.Show("Please select a team leader.", "Validation Error", MessageBoxButton.OK, MessageBoxImage.Warning);
                    cboLeader.Focus();
                    return;
                }
                
                int leaderUserId = (int)cboLeader.SelectedValue;
                int clubId = User.Current?.ClubId ?? 0;

                if (clubId == 0)
                {
                    MessageBox.Show("Club ID is not valid or you are not associated with any club.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                    return;
                }

                // Check if it's an add or update operation
                bool isNewTeam = string.IsNullOrEmpty(txtTeamID.Text);
                
                if (isNewTeam)
                {
                    // Create new team
                    Team team = new Team
                    {
                        TeamName = teamName,
                        Description = description,
                        ClubId = clubId
                    };

                    _teamService.AddTeamWithLeader(team, leaderUserId);
                    MessageBox.Show("Team added successfully and leader role updated!", "Success", MessageBoxButton.OK, MessageBoxImage.Information);
                }
                else
                {
                    // Update existing team
                    int teamId = int.Parse(txtTeamID.Text);
                    
                    var selectedTeam = dgTeam.SelectedItem as TeamView;
                    if (selectedTeam == null)
                    {
                        MessageBox.Show("Team data not found.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                        return;
                    }
                    
                    // Update leader role if changed
                    if (selectedTeam.LeaderId != leaderUserId)
                    {
                        // Demote old leader
                        var oldLeader = _accountService.GetAccountById(selectedTeam.LeaderId);
                        if (oldLeader != null && oldLeader.Role == "TeamLeader")
                        {
                            oldLeader.Role = "Member";
                            _accountService.UpdateAccountRoleOnly(oldLeader);
                        }

                        // Promote new leader
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
                    }

                    // Update team
                    Team team = new Team
                    {
                        TeamId = teamId,
                        TeamName = teamName,
                        Description = description,
                        ClubId = clubId
                    };

                    _teamService.UpdateTeam(team, leaderUserId);
                    MessageBox.Show("Team updated successfully!", "Success", MessageBoxButton.OK, MessageBoxImage.Information);
                }
                
                // Refresh data
                LoadTeams();
                ClearTeamForm();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error saving team: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void btnDeleteTeam_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                var selectedTeam = dgTeam.SelectedItem as TeamView;
                if (selectedTeam == null)
                {
                    MessageBox.Show("Please select a team to delete.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                    return;
                }

                var result = MessageBox.Show($"Are you sure you want to delete the team '{selectedTeam.TeamName}'?\nThis will also remove all team members.", 
                    "Delete Team", MessageBoxButton.YesNo, MessageBoxImage.Warning);
                
                if (result == MessageBoxResult.No)
                {
                    return;
                }

                _teamService.DeleteTeam(selectedTeam.TeamId);

                MessageBox.Show("Team deleted successfully!", "Success", MessageBoxButton.OK, MessageBoxImage.Information);
                LoadTeams();
                ClearTeamForm();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error deleting team: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void dgTeam_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            var selectedTeam = dgTeam.SelectedItem as TeamView;
            if (selectedTeam != null)
            {
                txtTeamID.Text = selectedTeam.TeamId.ToString();
                txtTeamName.Text = selectedTeam.TeamName;
                txtDescription.Text = selectedTeam.Description;
                cboLeader.SelectedValue = selectedTeam.LeaderId;
                
                // Save selected team ID
                _selectedTeamId = selectedTeam.TeamId;
                
                // Load team members
                LoadTeamMembers(_selectedTeamId);
            }
            else
            {
                ClearTeamForm();
            }
        }

        private void ClearTeamForm()
        {
            txtTeamID.Text = string.Empty;
            txtTeamName.Text = string.Empty;
            txtDescription.Text = string.Empty;
            cboLeader.SelectedIndex = -1;
            _selectedTeamId = 0;
            dgTeamMembers.ItemsSource = null;
        }

        private void LoadTeamMembers(int teamId)
        {
            try
            {
                if (teamId <= 0) return;
                
                // Get all team members
                var teamMembers = _teamMemberService.GetTeamMembersByTeamId(teamId);
                
                // Create a list of team members with role information
                var teamMembersWithRoles = new List<TeamMemberView>();
                foreach (var member in teamMembers)
                {
                    teamMembersWithRoles.Add(new TeamMemberView
                    {
                        TeamMemberId = member.TeamMemberId,
                        User = member.User,
                        JoinDate = member.JoinDate,
                        Role = member.User.Role,
                        // Can promote if user is a regular member (not already a team leader)
                        CanPromote = member.User.Role == "Member"
                    });
                }
                
                // Display team members in the DataGrid
                dgTeamMembers.ItemsSource = teamMembersWithRoles;
                
                // Load available members for the dropdown
                LoadAvailableMembers();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error loading team members: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }
        
        private void LoadAvailableMembers()
        {
            try
            {
                if (_selectedTeamId <= 0) return;
                
                // Get current team members
                var currentTeamMembers = _teamMemberService.GetTeamMembersByTeamId(_selectedTeamId)
                    .Select(tm => tm.UserId)
                    .ToList();
                
                // Get all members in the club who are not already in the team
                var clubId = User.Current?.ClubId ?? 0;
                var availableMembers = _accountService.GetMembersByClubId(clubId)
                    .Where(m => !currentTeamMembers.Contains(m.UserId) && m.Role == "Member")
                    .ToList();
                
                // Set the available members to the ComboBox
                cboAvailableMembers.ItemsSource = availableMembers;
                cboAvailableMembers.DisplayMemberPath = "FullName";
                cboAvailableMembers.SelectedValuePath = "UserId";
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error loading available members: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void dgTeamMembers_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            // This can be used if you need to perform actions when a team member is selected
        }

        private void btnAddMember_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (_selectedTeamId <= 0)
                {
                    MessageBox.Show("Please select a team first.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                    return;
                }
                
                if (cboAvailableMembers.SelectedItem == null)
                {
                    MessageBox.Show("Please select a member to add.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                    return;
                }
                
                // Get the selected member
                var selectedMember = cboAvailableMembers.SelectedItem as User;
                if (selectedMember == null)
                {
                    MessageBox.Show("Error getting selected member information.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                    return;
                }
                
                // Create new team member
                var teamMember = new TeamMember
                {
                    TeamId = _selectedTeamId,
                    UserId = selectedMember.UserId,
                    JoinDate = DateOnly.FromDateTime(DateTime.Now)
                };
                
                // Add the member to the team
                _teamMemberService.AddTeamMember(teamMember);
                
                MessageBox.Show($"{selectedMember.FullName} has been added to the team.", "Success", MessageBoxButton.OK, MessageBoxImage.Information);
                
                // Refresh team members list
                LoadTeamMembers(_selectedTeamId);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error adding team member: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void btnPromoteMember_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                // Get the team member from the button's DataContext
                var button = sender as Button;
                var teamMember = button.DataContext as TeamMemberView;
                
                if (teamMember == null)
                {
                    MessageBox.Show("Unable to get team member information.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                    return;
                }
                
                var result = MessageBox.Show($"Are you sure you want to promote {teamMember.User.FullName} to Team Leader?\nThis will demote the current team leader.", 
                    "Promote Member", MessageBoxButton.YesNo, MessageBoxImage.Question);
                
                if (result == MessageBoxResult.No)
                {
                    return;
                }
                
                // Get the current team
                var selectedTeam = dgTeam.SelectedItem as TeamView;
                if (selectedTeam == null)
                {
                    MessageBox.Show("Team information not found.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                    return;
                }
                
                // Demote current leader
                var currentLeader = _accountService.GetAccountById(selectedTeam.LeaderId);
                if (currentLeader != null && currentLeader.Role == "TeamLeader")
                {
                    currentLeader.Role = "Member";
                    _accountService.UpdateAccountRoleOnly(currentLeader);
                }
                
                // Promote new leader
                var newLeader = _accountService.GetAccountById(teamMember.User.UserId);
                if (newLeader == null)
                {
                    MessageBox.Show("User information not found.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                    return;
                }
                
                newLeader.Role = "TeamLeader";
                _accountService.UpdateAccountRoleOnly(newLeader);
                
                // Update team with new leader
                Team team = new Team
                {
                    TeamId = selectedTeam.TeamId,
                    TeamName = selectedTeam.TeamName,
                    Description = selectedTeam.Description,
                    ClubId = selectedTeam.ClubId
                };
                
                _teamService.UpdateTeam(team, newLeader.UserId);
                
                MessageBox.Show($"{teamMember.User.FullName} has been promoted to Team Leader.", "Success", MessageBoxButton.OK, MessageBoxImage.Information);
                
                // Refresh data
                LoadTeams();
                LoadTeamMembers(_selectedTeamId);
                
                // Update leader dropdown
                LoadLeaders();
                cboLeader.SelectedValue = newLeader.UserId;
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error promoting team member: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void btnRemoveMember_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                // Get the team member from the button's DataContext
                var button = sender as Button;
                var teamMember = button.DataContext as TeamMemberView;
                
                if (teamMember == null)
                {
                    MessageBox.Show("Unable to get team member information.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                    return;
                }
                
                // Check if the member is the team leader
                var selectedTeam = dgTeam.SelectedItem as TeamView;
                if (selectedTeam != null && teamMember.User.UserId == selectedTeam.LeaderId)
                {
                    MessageBox.Show("Cannot remove the team leader. Please assign a new team leader first.", 
                        "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                    return;
                }
                
                var result = MessageBox.Show($"Are you sure you want to remove {teamMember.User.FullName} from the team?", 
                    "Remove Member", MessageBoxButton.YesNo, MessageBoxImage.Question);
                
                if (result == MessageBoxResult.No)
                {
                    return;
                }
                
                // Remove the team member
                _teamMemberService.DeleteTeamMember(teamMember.TeamMemberId);
                
                MessageBox.Show($"{teamMember.User.FullName} has been removed from the team.", "Success", MessageBoxButton.OK, MessageBoxImage.Information);
                
                // Refresh team members list
                LoadTeamMembers(_selectedTeamId);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error removing team member: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

    }

    public class TeamMemberView
    {
        public int TeamMemberId { get; set; }
        public User User { get; set; }
        public DateOnly? JoinDate { get; set; }
        public string Role { get; set; }
        public bool CanPromote { get; set; }
    }
}
