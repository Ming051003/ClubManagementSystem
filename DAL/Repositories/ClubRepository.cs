using DAL.Interfaces;
using Model.Contexts;
using Model.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DAL.Repositories
{
    public class ClubRepository : IClubRepository
    {
        private readonly ClubManagementContext context;
        public ClubRepository(ClubManagementContext context)
        {
            this.context = context;
        }
        public void AddClub(Club club)
        {
            context.Clubs.Add(club);
            context.SaveChanges();
        }

        public void DeleteClub(int clubId)
        {
            var user = GetClubByClubId(clubId);
            if (user != null) { 
                context.Clubs.Remove(user);
                context.SaveChanges();
            }
        }

        public Club GetClubByClubId(int clubId)
        {
            return context.Clubs.FirstOrDefault(c => c.ClubId == clubId);
        }

        public List<Club> GetClubs()
        {
            return context.Clubs.ToList();
        }

        public void UpdateClub(Club club)
        {
           var existingClub =  context.Clubs.FirstOrDefault(c => c.ClubId ==  club.ClubId);
            if (existingClub != null) {
                existingClub.ClubName = club.ClubName;
                existingClub.Description = club.Description;
                existingClub.EstablishedDate = club.EstablishedDate;

                context.SaveChanges();
            }
            else
            {
                throw new Exception("Club not found");
            }
        }
    }
}
