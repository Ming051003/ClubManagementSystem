using Model.Models;
using System.Collections.Generic;

namespace DAL.Interfaces
{
    public interface IAccountRepository
    {
        public List<User> GetAll();
        public User Login(string username, string password);
        public void AddAccount(User account);
        public void UpdateAccount(User account);
        public void DeleteAccount(int id);
        public User GetAccountById(int id);
        public List<Club> GetAllClubs();
        public int? GetClubIdByUsername(string username);
        public List<User> GetLeadersByClubId(int currentClubId);
        public void UpdateAccountRoleOnly(User user);
        public bool ChangePassword(int userId, string currentPassword, string newPassword);
        public List<User> GetMembersByClubId(int clubId);
    }
}
