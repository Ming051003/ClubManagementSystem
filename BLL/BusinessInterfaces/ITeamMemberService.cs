using Model.Models;
using System.Collections.Generic;

namespace BLL.BusinessInterfaces
{
    public interface ITeamMemberService
    {
        List<TeamMember> GetAllTeamMembers();
        TeamMember GetTeamMemberById(int id);
        List<TeamMember> GetTeamMembersByTeamId(int teamId);
        List<TeamMember> GetTeamMembersByUserId(int userId);
        void AddTeamMember(TeamMember teamMember);
        void UpdateTeamMember(TeamMember teamMember);
        void DeleteTeamMember(int id);
        bool TeamMemberExists(int id);
        bool TeamMemberExistsByTeamAndUser(int teamId, int userId);
    }
}
