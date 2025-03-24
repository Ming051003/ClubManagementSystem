using Model.Models;
using Model.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BLL.BusinessInterfaces
{
    public interface ITeamService
    {
        public List<TeamView> GetTeamsByClubId(int clubId);
        public Team GetTeamById(int teamId);
        public void AddTeamWithLeader(Team team, int leaderUserId);
        public void UpdateTeam(Team team, int leaderUserId);
        public void DeleteTeam(int teamId);
    }
}
