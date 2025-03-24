using BLL.BusinessInterfaces;
using DAL.Interfaces;
using Model.Models;
using System;
using System.Collections.Generic;

namespace BLL.BusinessService
{
    public class TeamMemberService : ITeamMemberService
    {
        private readonly ITeamMemberRepository _teamMemberRepository;

        public TeamMemberService(ITeamMemberRepository teamMemberRepository)
        {
            _teamMemberRepository = teamMemberRepository;
        }

        public List<TeamMember> GetAllTeamMembers()
        {
            try
            {
                return _teamMemberRepository.GetAll();
            }
            catch (Exception ex)
            {
                throw new Exception("Error retrieving team members", ex);
            }
        }

        public TeamMember GetTeamMemberById(int id)
        {
            try
            {
                return _teamMemberRepository.GetById(id);
            }
            catch (Exception ex)
            {
                throw new Exception($"Error retrieving team member with ID {id}", ex);
            }
        }

        public List<TeamMember> GetTeamMembersByTeamId(int teamId)
        {
            try
            {
                return _teamMemberRepository.GetByTeamId(teamId);
            }
            catch (Exception ex)
            {
                throw new Exception($"Error retrieving team members for team ID {teamId}", ex);
            }
        }

        public List<TeamMember> GetTeamMembersByUserId(int userId)
        {
            try
            {
                return _teamMemberRepository.GetByUserId(userId);
            }
            catch (Exception ex)
            {
                throw new Exception($"Error retrieving team memberships for user ID {userId}", ex);
            }
        }

        public void AddTeamMember(TeamMember teamMember)
        {
            try
            {
                if (teamMember == null)
                    throw new ArgumentNullException(nameof(teamMember));

                if (_teamMemberRepository.ExistsByTeamAndUser(teamMember.TeamId, teamMember.UserId))
                    throw new InvalidOperationException("User is already a member of this team");

                _teamMemberRepository.Add(teamMember);
            }
            catch (Exception ex)
            {
                throw new Exception("Error adding team member", ex);
            }
        }

        public void UpdateTeamMember(TeamMember teamMember)
        {
            try
            {
                if (teamMember == null)
                    throw new ArgumentNullException(nameof(teamMember));

                if (!_teamMemberRepository.Exists(teamMember.TeamMemberId))
                    throw new InvalidOperationException($"Team member with ID {teamMember.TeamMemberId} not found");

                _teamMemberRepository.Update(teamMember);
            }
            catch (Exception ex)
            {
                throw new Exception("Error updating team member", ex);
            }
        }

        public void DeleteTeamMember(int id)
        {
            try
            {
                if (!_teamMemberRepository.Exists(id))
                    throw new InvalidOperationException($"Team member with ID {id} not found");

                _teamMemberRepository.Delete(id);
            }
            catch (Exception ex)
            {
                throw new Exception($"Error deleting team member with ID {id}", ex);
            }
        }

        public bool TeamMemberExists(int id)
        {
            try
            {
                return _teamMemberRepository.Exists(id);
            }
            catch (Exception ex)
            {
                throw new Exception($"Error checking existence of team member with ID {id}", ex);
            }
        }

        public bool TeamMemberExistsByTeamAndUser(int teamId, int userId)
        {
            try
            {
                return _teamMemberRepository.ExistsByTeamAndUser(teamId, userId);
            }
            catch (Exception ex)
            {
                throw new Exception($"Error checking existence of team member for team ID {teamId} and user ID {userId}", ex);
            }
        }
    }
}
