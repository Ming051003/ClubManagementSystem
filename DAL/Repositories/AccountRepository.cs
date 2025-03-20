using BLL.Interfaces;
using Microsoft.EntityFrameworkCore;
using Model.Contexts;
using Model.Models;
using System.Collections.Generic;
using System.Linq;

namespace DAL.Repositories
{
    public class AccountRepository : IAccountRepository
    {
        private readonly ClubManagementContext _context;
        
        public AccountRepository(ClubManagementContext context)
        {
            _context = context;
        }

        public List<User> GetAll()
        {
            return _context.Users
                .Include(u => u.Club)
                .ToList();
        }

        public User GetById(int id)
        {
            return _context.Users
                .Include(u => u.Club)
                .FirstOrDefault(u => u.UserId == id);
        }

        public User GetByEmail(string email)
        {
            return _context.Users
                .Include(u => u.Club)
                .FirstOrDefault(u => u.Email == email);
        }

        public User Login(string username, string password)
        {
            return _context.Users.FirstOrDefault(u => u.UserName == username && u.Password == password);
        }

        public void Update(User entity)
        {
            _context.Users.Attach(entity);
            _context.Entry(entity).State = EntityState.Modified;
            _context.SaveChanges();
        }

        public void Delete(int id)
        {
            var entity = _context.Users.Find(id);
            if (entity != null)
            {
                _context.Users.Remove(entity);
                _context.SaveChanges();
            }
        }

        public List<Club> GetAllClubs()
        {
            return _context.Clubs.ToList();
        }

        

        public void AddAccount(User account)
        {
            throw new NotImplementedException();
        }

        public void UpdateAccount(User account)
        {
            throw new NotImplementedException();
        }

        public void DeleteAccount(int id)
        {
            throw new NotImplementedException();
        }

        public User GetAccountById(int id)
        {
            throw new NotImplementedException();
        }
    }
}
