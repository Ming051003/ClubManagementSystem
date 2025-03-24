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
                existingUser.UserName = account.UserName;
                existingUser.StudentId = account.StudentId;
                existingUser.FullName = account.FullName;
                existingUser.Email = account.Email;
                existingUser.Password = account.Password;  
                existingUser.Role = account.Role;
                existingUser.ClubId = account.ClubId;
                existingUser.Status = account.Status;
                existingUser.JoinDate = account.JoinDate; 

                context.SaveChanges();
            }
            else
            {
                throw new Exception("User not found");
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
    
        public int? GetClubIdByUsername(string username)
        {
            var user = context.Users.FirstOrDefault(u => u.UserName == username);
            return user?.ClubId;
        }

        public List<User> GetLeadersByClubId(int currentClubId)
        {
            return context.Users
                .Where(u => (u.Role == "TeamLeader" || u.Role == "Member") && u.ClubId == currentClubId)  
                .ToList();
        }

        public void UpdateAccountRoleOnly(User user)
        {
            var existingUser = context.Users.FirstOrDefault(u => u.UserId == user.UserId);
            if (existingUser != null)
            {
                existingUser.Role = user.Role;
                context.SaveChanges();
            }
            else
            {
                throw new Exception("User not found.");
            }
        }

        public bool ChangePassword(int userId, string currentPassword, string newPassword)
        {
            try
            {
                using var context = new ClubManagementContext();
                var user = context.Users.FirstOrDefault(u => u.UserId == userId);
                
                if (user == null) return false;
                
                // Verify current password
                if (user.Password != currentPassword) return false;
                
                // Update password
                user.Password = newPassword;
                context.SaveChanges();
                
                return true;
            }
            catch
            {
                return false;
            }
        }

        public List<User> GetMembersByClubId(int clubId)
        {
            return context.Users
                .Where(u => u.ClubId == clubId && u.Role == "Member")
                .ToList();
        }
    }
}
