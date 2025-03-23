using Model.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BLL.BusinessInterfaces
{
    public interface IClubService
    {
        public List<Club> GetClubs();
        public void AddClub(Club club);
        public void UpdateClub(Club club);
        public void DeleteClub(int clubId);
        public Club GetClubByClubId(int clubId);
    }
}
