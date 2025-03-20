using DAL.Interfaces;
using Microsoft.EntityFrameworkCore;
using Model.Contexts;
using Model.Models;
using System.Collections.Generic;
using System.Linq;

namespace DAL.Repositories
{
    public class UserRepository : Repository<User>, IUserRepository
    {
        public UserRepository(ClubManagementContext context) : base(context)
        {
        }

        public override IEnumerable<User> GetAll()
        {
            return _context.Users
                .Include(u => u.Club)
                .ToList();
        }

        public override User GetById(int id)
        {
            return _context.Users
                .Include(u => u.Club)
                .FirstOrDefault(u => u.UserId == id);
        }
    }
}
