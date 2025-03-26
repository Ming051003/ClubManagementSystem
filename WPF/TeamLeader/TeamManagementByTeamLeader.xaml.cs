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

namespace WPF.TeamLeader
{
    /// <summary>
    /// Interaction logic for TeamManagementByTeamLeader.xaml
    /// </summary>
    public partial class TeamManagementByTeamLeader : UserControl
    {
        private readonly ITeamService _teamService;
        private readonly IAccountService _accountService;
        private readonly ITeamMemberService _teamMemberService;
        private int _selectedTeamId;
        private TeamMemberView _selectedTeamMember;
        private bool _isEditMode = false;

        public TeamManagementByTeamLeader()
        {
            InitializeComponent();
            _teamService = ((App)Application.Current).ServiceProvider.GetRequiredService<ITeamService>()
                ?? throw new ArgumentNullException(nameof(ITeamService));
            _accountService = ((App)Application.Current).ServiceProvider.GetRequiredService<IAccountService>()
                ?? throw new ArgumentNullException(nameof(IAccountService));
            _teamMemberService = ((App)Application.Current).ServiceProvider.GetRequiredService<ITeamMemberService>()
                ?? throw new ArgumentNullException(nameof(ITeamMemberService));

            LoadTeam();
        }

        private void LoadTeam()
        {
            try
            {
                if (User.Current == null)
                {
                    MessageBox.Show("You are not logged in.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                    return;
                }

                int clubId = User.Current.ClubId ?? 0;
                if (clubId == 0)
                {
                    MessageBox.Show("You are not associated with any club.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                    return;
                }

                // Find the team where the current user is a member
                var teamMember = _teamMemberService.GetTeamMembersByUserId(User.Current.UserId).FirstOrDefault();
                if (teamMember == null)
                {
                    MessageBox.Show("You are not assigned to any team.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                    return;
                }

                var team = _teamService.GetTeamById(teamMember.TeamId);
                if (team == null)
                {
                    MessageBox.Show("Team information not found.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                    return;
                }

                // Populate team details
                txtTeamID.Text = team.TeamId.ToString();
                txtTeamName.Text = team.TeamName;
                txtDescription.Text = team.Description;
                _selectedTeamId = team.TeamId;

                // Find the team leader (user with TeamLeader role in this team)
                var teamLeader = _teamMemberService.GetTeamMembersByTeamId(team.TeamId)
                    .FirstOrDefault(tm => tm.User.Role == "TeamLeader")?.User;

                if (teamLeader != null)
                {
                    cboLeader.ItemsSource = new List<User> { teamLeader };
                    cboLeader.SelectedIndex = 0;
                }

                // Load team members
                LoadTeamMembers(_selectedTeamId);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error loading team: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void LoadTeamMembers(int teamId)
        {
            try
            {
                if (teamId <= 0) return;

                var teamMembers = _teamMemberService.GetTeamMembersByTeamId(teamId);
                var teamMembersWithRoles = new List<TeamMemberView>();

                foreach (var member in teamMembers)
                {
                    teamMembersWithRoles.Add(new TeamMemberView
                    {
                        TeamMemberId = member.TeamMemberId,
                        User = member.User,
                        JoinDate = member.JoinDate,
                        Role = member.User.Role
                    });
                }

                dgTeamMembers.ItemsSource = teamMembersWithRoles;
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error loading team members: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void btnSaveTeam_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (_selectedTeamId == 0)
                {
                    MessageBox.Show("No team selected to update.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                    return;
                }

                string teamName = txtTeamName.Text.Trim();
                if (string.IsNullOrEmpty(teamName))
                {
                    MessageBox.Show("Team name is required.", "Validation Error", MessageBoxButton.OK, MessageBoxImage.Warning);
                    txtTeamName.Focus();
                    return;
                }

                string description = txtDescription.Text.Trim();
                int clubId = User.Current?.ClubId ?? 0;

                if (clubId == 0)
                {
                    MessageBox.Show("Club ID is not valid.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                    return;
                }

                var currentTeam = _teamService.GetTeamById(_selectedTeamId);
                if (currentTeam == null)
                {
                    MessageBox.Show("Team data not found.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                    return;
                }

                // Ensure the user is the team leader
                bool isTeamLeader = User.Current.Role == "TeamLeader";
                if (!isTeamLeader)
                {
                    MessageBox.Show("You don't have permission to edit this team.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                    return;
                }

                Team team = new Team
                {
                    TeamId = _selectedTeamId,
                    TeamName = teamName,
                    Description = description,
                    ClubId = clubId
                };

                _teamService.UpdateTeam(team, User.Current.UserId);
                MessageBox.Show("Team details updated successfully!", "Success", MessageBoxButton.OK, MessageBoxImage.Information);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error saving team: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void dgTeamMembers_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            try
            {
                // If in edit mode, ask user if they want to discard changes
                if (_isEditMode)
                {
                    var result = MessageBox.Show("You have unsaved changes. Do you want to discard them?",
                        "Unsaved Changes", MessageBoxButton.YesNo, MessageBoxImage.Warning);

                    if (result == MessageBoxResult.No)
                    {
                        // Revert selection change
                        dgTeamMembers.SelectedItem = _selectedTeamMember;
                        return;
                    }
                    else
                    {
                        // Exit edit mode
                        SetEditMode(false);
                    }
                }

                _selectedTeamMember = dgTeamMembers.SelectedItem as TeamMemberView;

                if (_selectedTeamMember != null)
                {
                    // Show member details
                    txtMemberId.Text = _selectedTeamMember.User.UserId.ToString();
                    txtMemberName.Text = _selectedTeamMember.User.FullName;
                    txtStudentId.Text = _selectedTeamMember.User.StudentId;
                    txtEmail.Text = _selectedTeamMember.User.Email;

                    // Set role in combo box
                    cboMemberRole.SelectedIndex = _selectedTeamMember.Role == "TeamLeader" ? 1 : 0;

                    // Show the action buttons
                    if (_selectedTeamMember.User.UserId != User.Current.UserId)
                    {
                        btnRemoveMember.Visibility = Visibility.Visible;
                        btnEditMember.Visibility = Visibility.Visible;
                    }
                    else
                    {
                        btnRemoveMember.Visibility = Visibility.Collapsed;
                        btnEditMember.Visibility = Visibility.Collapsed;
                    }

                    btnClearSelection.Visibility = Visibility.Visible;

                    // Hide the "no selection" message
                    txtNoSelection.Visibility = Visibility.Collapsed;
                }
                else
                {
                    ClearMemberDetails();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error selecting team member: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void ClearMemberDetails()
        {
            // Clear member details
            txtMemberId.Text = string.Empty;
            txtMemberName.Text = string.Empty;
            txtStudentId.Text = string.Empty;
            txtEmail.Text = string.Empty;
            cboMemberRole.SelectedIndex = -1;

            // Hide all action buttons
            btnRemoveMember.Visibility = Visibility.Collapsed;
            btnEditMember.Visibility = Visibility.Collapsed;
            btnSaveMember.Visibility = Visibility.Collapsed;
            btnCancelEdit.Visibility = Visibility.Collapsed;
            btnClearSelection.Visibility = Visibility.Collapsed;

            // Show the "no selection" message
            txtNoSelection.Visibility = Visibility.Visible;

            // Clear the selection
            _selectedTeamMember = null;

            // Exit edit mode if active
            if (_isEditMode)
            {
                SetEditMode(false);
            }
        }

        private void SetEditMode(bool isEdit)
        {
            _isEditMode = isEdit;

            // Toggle read-only state for editable fields
            txtMemberName.IsReadOnly = !isEdit;
            txtStudentId.IsReadOnly = !isEdit;
            txtEmail.IsReadOnly = !isEdit;
            cboMemberRole.IsEnabled = isEdit;

            // Toggle button visibility based on edit mode
            if (isEdit)
            {
                btnEditMember.Visibility = Visibility.Collapsed;
                btnRemoveMember.Visibility = Visibility.Collapsed;
                btnSaveMember.Visibility = Visibility.Visible;
                btnCancelEdit.Visibility = Visibility.Visible;
                btnClearSelection.Visibility = Visibility.Collapsed;
            }
            else
            {
                if (_selectedTeamMember != null && _selectedTeamMember.User.UserId != User.Current.UserId)
                {
                    btnEditMember.Visibility = Visibility.Visible;
                    btnRemoveMember.Visibility = Visibility.Visible;
                }
                btnSaveMember.Visibility = Visibility.Collapsed;
                btnCancelEdit.Visibility = Visibility.Collapsed;
                btnClearSelection.Visibility = Visibility.Visible;
            }
        }

        private void btnEditMember_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (_selectedTeamMember == null)
                {
                    MessageBox.Show("Please select a team member to edit.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                    return;
                }

                // Enter edit mode
                SetEditMode(true);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error entering edit mode: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void btnSaveMember_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (_selectedTeamMember == null)
                {
                    MessageBox.Show("No member selected to update.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                    return;
                }

                // Validate inputs
                string fullName = txtMemberName.Text.Trim();
                string studentId = txtStudentId.Text.Trim();
                string email = txtEmail.Text.Trim();
                string role = (cboMemberRole.SelectedIndex == 1) ? "TeamLeader" : "Member";

                if (string.IsNullOrEmpty(fullName))
                {
                    MessageBox.Show("Full name is required.", "Validation Error", MessageBoxButton.OK, MessageBoxImage.Warning);
                    txtMemberName.Focus();
                    return;
                }

                if (string.IsNullOrEmpty(studentId))
                {
                    MessageBox.Show("Student ID is required.", "Validation Error", MessageBoxButton.OK, MessageBoxImage.Warning);
                    txtStudentId.Focus();
                    return;
                }

                if (string.IsNullOrEmpty(email))
                {
                    MessageBox.Show("Email is required.", "Validation Error", MessageBoxButton.OK, MessageBoxImage.Warning);
                    txtEmail.Focus();
                    return;
                }

                // Check if role is being changed to TeamLeader
                bool isPromotingToLeader = _selectedTeamMember.Role != "TeamLeader" && role == "TeamLeader";

                if (isPromotingToLeader)
                {
                    // Check if there's already a team leader
                    var currentLeader = _teamMemberService.GetTeamMembersByTeamId(_selectedTeamId)
                        .FirstOrDefault(tm => tm.User.Role == "TeamLeader");

                    if (currentLeader != null)
                    {
                        var result = MessageBox.Show(
                            $"This team already has a leader ({currentLeader.User.FullName}). Promoting this member will demote the current leader. Continue?",
                            "Change Team Leader", MessageBoxButton.YesNo, MessageBoxImage.Warning);

                        if (result == MessageBoxResult.No)
                        {
                            return;
                        }
                    }
                }

                // Update user information
                var user = _selectedTeamMember.User;
                user.FullName = fullName;
                user.StudentId = studentId;
                user.Email = email;
                user.Role = role;

                _accountService.UpdateAccount(user);

                MessageBox.Show("Member details updated successfully!", "Success", MessageBoxButton.OK, MessageBoxImage.Information);

                // Exit edit mode
                SetEditMode(false);

                // Refresh the team members list
                LoadTeamMembers(_selectedTeamId);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error saving member details: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void btnCancelEdit_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                // Revert changes by reloading the current member data
                if (_selectedTeamMember != null)
                {
                    txtMemberName.Text = _selectedTeamMember.User.FullName;
                    txtStudentId.Text = _selectedTeamMember.User.StudentId;
                    txtEmail.Text = _selectedTeamMember.User.Email;
                    cboMemberRole.SelectedIndex = _selectedTeamMember.Role == "TeamLeader" ? 1 : 0;
                }

                // Exit edit mode
                SetEditMode(false);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error canceling edit: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void btnRemoveMember_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (_selectedTeamMember == null)
                {
                    MessageBox.Show("Please select a team member to remove.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                    return;
                }

                if (_selectedTeamMember.User.UserId == User.Current.UserId)
                {
                    MessageBox.Show("You cannot remove yourself from the team.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                    return;
                }

                var result = MessageBox.Show($"Are you sure you want to remove {_selectedTeamMember.User.FullName} from the team?",
                    "Remove Member", MessageBoxButton.YesNo, MessageBoxImage.Question);

                if (result == MessageBoxResult.No)
                {
                    return;
                }

                _teamMemberService.DeleteTeamMember(_selectedTeamMember.TeamMemberId);
                MessageBox.Show($"{_selectedTeamMember.User.FullName} has been removed from the team.", "Success", MessageBoxButton.OK, MessageBoxImage.Information);

                // Reload team members
                LoadTeamMembers(_selectedTeamId);

                // Clear member details
                ClearMemberDetails();
                dgTeamMembers.SelectedItem = null;
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error removing team member: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void btnClearSelection_Click(object sender, RoutedEventArgs e)
        {
            dgTeamMembers.SelectedItem = null;
            ClearMemberDetails();
        }
    }

    public class TeamMemberView
    {
        public int TeamMemberId { get; set; }
        public User User { get; set; }
        public DateOnly? JoinDate { get; set; }
        public string Role { get; set; }
    }
}