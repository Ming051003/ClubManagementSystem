using BLL.BusinessInterfaces;
using DAL.Interfaces;
using Model.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BLL.BusinessService
{
    public class ClubService : IClubService
    {
        private readonly IClubRepository clubRepository;
        public ClubService(IClubRepository clubRepository)
        {
            this.clubRepository = clubRepository;
        }
        public void AddClub(Club club)
        {
            clubRepository.AddClub(club);
        }

        public void DeleteClub(int clubId)
        {
           clubRepository.DeleteClub(clubId);
        }

        public Club GetClubByClubId(int clubId)
        {
            return clubRepository.GetClubByClubId(clubId);
        }

        public List<Club> GetClubs()
        {
            return clubRepository.GetClubs();
        }
        public void UpdateClub(Club club)
        {
            clubRepository.UpdateClub(club);
        }
    }
}
