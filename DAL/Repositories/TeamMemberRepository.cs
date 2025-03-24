using DAL.Interfaces;
using Model.Contexts;
using Model.Models;
using System.Collections.Generic;
using System.Linq;
using Microsoft.EntityFrameworkCore;

namespace DAL.Repositories
{
    public class TeamMemberRepository : ITeamMemberRepository
    {
        private readonly ClubManagementContext context;

        public TeamMemberRepository(ClubManagementContext context)
        {
            this.context = context;
        }

        public List<TeamMember> GetAll()
        {
            return context.TeamMembers
                .Include(tm => tm.Team)
                .Include(tm => tm.User)
                .ToList();
        }

        public TeamMember GetById(int id)
        {
            return context.TeamMembers
                .Include(tm => tm.Team)
                .Include(tm => tm.User)
                .FirstOrDefault(tm => tm.TeamMemberId == id);
        }

        public List<TeamMember> GetByTeamId(int teamId)
        {
            return context.TeamMembers
                .Include(tm => tm.Team)
                .Include(tm => tm.User)
                .Where(tm => tm.TeamId == teamId)
                .ToList();
        }

        public List<TeamMember> GetByUserId(int userId)
        {
            return context.TeamMembers
                .Include(tm => tm.Team)
                .Include(tm => tm.User)
                .Where(tm => tm.UserId == userId)
                .ToList();
        }

        public void Add(TeamMember teamMember)
        {
            context.TeamMembers.Add(teamMember);
            context.SaveChanges();
        }

        public void Update(TeamMember teamMember)
        {
            var existingTeamMember = context.TeamMembers.Find(teamMember.TeamMemberId);
            if (existingTeamMember != null)
            {
                existingTeamMember.TeamId = teamMember.TeamId;
                existingTeamMember.UserId = teamMember.UserId;
                existingTeamMember.JoinDate = teamMember.JoinDate;
                context.SaveChanges();
            }
        }

        public void Delete(int id)
        {
            var teamMember = context.TeamMembers.Find(id);
            if (teamMember != null)
            {
                context.TeamMembers.Remove(teamMember);
                context.SaveChanges();
            }
        }

        public bool Exists(int id)
        {
            return context.TeamMembers.Any(tm => tm.TeamMemberId == id);
        }

        public bool ExistsByTeamAndUser(int teamId, int userId)
        {
            return context.TeamMembers.Any(tm => tm.TeamId == teamId && tm.UserId == userId);
        }
    }
}
