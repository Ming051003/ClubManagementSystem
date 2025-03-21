using DAL.Interfaces;
using Microsoft.EntityFrameworkCore;
using Model.Contexts;
using Model.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BLL.Repositories
{
    public class AccountRepository : IAccountRepository
    {
        private readonly ClubManagementContext context;
        public AccountRepository(ClubManagementContext context)
        {
            this.context = context;
        }

        public void AddAccount(User account)
        {
             context.Users.Add(account);
             context.SaveChanges();

        }

        public void DeleteAccount(int id)
        {
            User user = GetAccountById(id);
            if (user != null) { 
                context.Users.Remove(user);
                context.SaveChanges();  
            }
        }   

        public User GetAccountById(int id)
        {
            return context.Users.FirstOrDefault(u => u.UserId == id);
        }

        public List<Club> GetAllClubs()
        {
            return context.Clubs.ToList();
        }

        public void UpdateAccount(User account)
        {
            var existingUser = context.Users.FirstOrDefault(u => u.UserId == account.UserId);
            if (existingUser != null)
            {
                existingUser.StudentId = account.StudentId;
                existingUser.UserName = account.UserName;
                existingUser.Password = account.Password;
                existingUser.FullName = account.FullName;
                existingUser.Email = account.Email;
                existingUser.Role = account.Role;
                existingUser.ClubId = account.ClubId;
                existingUser.Status = account.Status;
                context.SaveChanges();
            }           
        }

        public List<User> GetAll()
        {
            return context.Users.Include(u => u.Club).ToList();
        }

        public User Login(string username, string password)
        {
            return context.Users.FirstOrDefault(u => u.UserName == username && u.Password == password);
        }

        public List<User> GetAllUsersByRole(string role)
        {
            throw new NotImplementedException();
        }
    }
}
