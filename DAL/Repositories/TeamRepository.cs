using BLL.Repositories;
using DAL.Interfaces;
using Microsoft.EntityFrameworkCore;
using Model.Contexts;
using Model.Models;
using Model.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DAL.Repositories
{
    public class TeamRepository : ITeamRepository
    {
        private readonly ClubManagementContext context;
        public TeamRepository(ClubManagementContext context)
        {
            this.context = context;
        }

        public List<Team> GetAllTeams()
        {
            return context.Teams
                .Include(t => t.Club)
                .Include(t => t.TeamMembers)
                .ThenInclude(tm => tm.User)
                .ToList();
        }

        public void AddTeam(Team team)
        {
            context.Teams.Add(team);
            context.SaveChanges();
        }

        public void AddTeamMember(TeamMember teamMember)
        {
            context.TeamMembers.Add(teamMember); 
            context.SaveChanges();
        }
        public void UpdateTeam(Team team)
        {
            var existingTeam = context.Teams.FirstOrDefault(t => t.TeamId == team.TeamId);
            if (existingTeam != null)
            {
                existingTeam.TeamName = team.TeamName;
                existingTeam.Description = team.Description;
                existingTeam.ClubId = team.ClubId;

                context.SaveChanges();
            }
            else
            {
                throw new Exception("Team not found.");
            }
        }

       
        public void DeleteTeam(int teamId)
        {
            var team = context.Teams.FirstOrDefault(t => t.TeamId == teamId);
            if (team != null)
            {
                DeleteTeamMembersByTeamId(teamId);

                context.Teams.Remove(team);
                context.SaveChanges();
            }
            else
            {
                throw new Exception("Team not found.");
            }
        }

        public void DeleteTeamMembersByTeamId(int teamId)
        {
            var teamMembers = context.TeamMembers.Where(tm => tm.TeamId == teamId).ToList();

            context.TeamMembers.RemoveRange(teamMembers);
            context.SaveChanges();
        }


        public Team GetTeamById(int teamId)
        {
           
            return context.Teams
                .Include(t => t.TeamMembers)  
                .Include(t => t.Club)         
                .FirstOrDefault(t => t.TeamId == teamId);  
        }

     
    }
}
