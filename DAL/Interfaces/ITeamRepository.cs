using Model.Models;
using Model.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DAL.Interfaces
{
    public interface ITeamRepository
    {
        public List<Team> GetAllTeams();
        public void AddTeam(Team team);
        public void AddTeamMember(TeamMember teamMember);
        public Team GetTeamById(int teamId);
        public void UpdateTeam(Team team);
        public void DeleteTeam(int teamId);
        public void DeleteTeamMembersByTeamId(int teamId);
    }
}
