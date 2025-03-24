using Model.Models;
using System.Collections.Generic;

namespace DAL.Interfaces
{
    public interface ITeamMemberRepository
    {
        List<TeamMember> GetAll();
        TeamMember GetById(int id);
        List<TeamMember> GetByTeamId(int teamId);
        List<TeamMember> GetByUserId(int userId);
        void Add(TeamMember teamMember);
        void Update(TeamMember teamMember);
        void Delete(int id);
        bool Exists(int id);
        bool ExistsByTeamAndUser(int teamId, int userId);
    }
}
