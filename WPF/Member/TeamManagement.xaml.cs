using BLL.BusinessInterfaces;
using Microsoft.Extensions.DependencyInjection;
using Model.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;

namespace WPF.Member
{
    public partial class TeamManagement : UserControl
    {
        private readonly ITeamService _teamService;
        private readonly ITeamMemberService _teamMemberService;

        public TeamManagement()
        {
            InitializeComponent();
            _teamService = ((App)Application.Current).ServiceProvider.GetRequiredService<ITeamService>()
                ?? throw new ArgumentNullException(nameof(ITeamService));
            _teamMemberService = ((App)Application.Current).ServiceProvider.GetRequiredService<ITeamMemberService>()
                ?? throw new ArgumentNullException(nameof(ITeamMemberService));
            LoadTeamData();
        }

        private void LoadTeamData()
        {
            try
            {
                if (User.Current == null)
                {
                    MessageBox.Show("You are not logged in.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                    return;
                }

                var teamMember = _teamMemberService.GetTeamMembersByUserId(User.Current.UserId).FirstOrDefault();
                if (teamMember == null)
                {
                    txtTeamName.Text = "Not assigned to any team";
                    txtTeamLeader.Text = "N/A";
                    txtMemberCount.Text = "0";
                    dgTeamMembers.ItemsSource = null;
                    return;
                }

                var team = _teamService.GetTeamById(teamMember.TeamId);
                if (team == null)
                {
                    MessageBox.Show("Team information not found.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                    return;
                }

                txtTeamName.Text = team.TeamName;
                
                var teamLeader = _teamMemberService.GetTeamMembersByTeamId(team.TeamId)
                    .FirstOrDefault(tm => tm.User.Role == "Team Leader");
                
                txtTeamLeader.Text = teamLeader?.User?.FullName ?? "Not assigned";
                
                var teamMembers = _teamMemberService.GetTeamMembersByTeamId(team.TeamId);
                txtMemberCount.Text = teamMembers.Count.ToString();
                
                var teamMembersWithRoles = new List<TeamMemberViewModel>();
                foreach (var member in teamMembers)
                {
                    teamMembersWithRoles.Add(new TeamMemberViewModel
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
                MessageBox.Show($"Error loading team data: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }
    }

    public class TeamMemberViewModel
    {
        public int TeamMemberId { get; set; }
        public User User { get; set; }
        public DateOnly? JoinDate { get; set; }
        public string Role { get; set; }
    }
}
