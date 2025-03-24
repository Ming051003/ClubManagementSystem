using BLL.BusinessInterfaces;
using BLL.Repositories;
using DAL.Interfaces;
using Microsoft.EntityFrameworkCore;
using Model.Models;
using Model.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BLL.BusinessService
{
    public class TeamService : ITeamService
    {
        private readonly ITeamRepository _teamRepository;
        private readonly IAccountRepository _accountRepository;
        public TeamService(ITeamRepository teamRepository, IAccountRepository accountRepository )
        {
            _teamRepository = teamRepository;
            _accountRepository = accountRepository;
        }

        public List<TeamView> GetTeamsByClubId(int clubId)
        {
            var teams = _teamRepository.GetAllTeams()
                .Where(t => t.ClubId == clubId)
                .ToList();

            var teamViews = teams.Select(t => new TeamView
            {
                TeamId = t.TeamId,
                TeamName = t.TeamName,
                Description = t.Description,
                ClubName = t.Club.ClubName,
                LeaderId = t.TeamMembers
                    .FirstOrDefault(tm => tm.User.Role == "TeamLeader")?.User.UserId ?? 0,
                Leader = t.TeamMembers
                    .Where(tm => tm.User.Role == "TeamLeader")  
                    .Select(tm => tm.User.FullName)
                    .FirstOrDefault()
            }).ToList();

            return teamViews;
        }

        public void AddTeamWithLeader(Team team, int leaderUserId)
        {
            var leader = _accountRepository.GetAccountById(leaderUserId);
            if (leader != null && leader.Role != "TeamLeader")
            {
                leader.Role = "TeamLeader"; 
                _accountRepository.UpdateAccount(leader);
            }
            _teamRepository.AddTeam(team);
            var teamMember = new TeamMember
            {
                TeamId = team.TeamId,
                UserId = leaderUserId,
                JoinDate = DateOnly.FromDateTime(DateTime.Now)  
            };

            _teamRepository.AddTeamMember(teamMember);
        }

        public void UpdateTeam(Team team, int leaderUserId)
        {
            if (string.IsNullOrEmpty(team.TeamName) || string.IsNullOrEmpty(team.Description))
            {
                throw new Exception("Team Name and Description cannot be empty.");
            }

            team.ClubId = User.Current?.ClubId ?? 0;  

            if (team.ClubId == 0)
            {
                throw new Exception("Invalid ClubId. The user is not associated with any club.");
            }

            _teamRepository.UpdateTeam(team);

            var leader = _accountRepository.GetAccountById(leaderUserId);
            if (leader != null && leader.Role != "TeamLeader")
            {
                leader.Role = "TeamLeader";
                _accountRepository.UpdateAccount(leader);
            }
        }



        public void DeleteTeam(int teamId)
        {
            var team = _teamRepository.GetTeamById(teamId);
            if (team == null)
            {
                throw new Exception("Team not found.");
            }

            _teamRepository.DeleteTeam(teamId);
        }

       
    }
}
