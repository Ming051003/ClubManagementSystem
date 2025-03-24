using BLL.BusinessInterfaces;
using BLL.Repositories;
using DAL.Interfaces;
using DAL.Repositories;
using Model.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BLL.BusinessService
{
    public class AccountService : IAccountService
    {
        private readonly IAccountRepository accountRepository;
        public AccountService(IAccountRepository accountRepository)
        {
            this.accountRepository = accountRepository;
        }

        public void AddAccount(User account)
        {
            accountRepository.AddAccount(account);
        }

        public void DeleteAccount(int id)
        {
            accountRepository.DeleteAccount(id);
        }

        public User GetAccountById(int id)
        {
            return accountRepository.GetAccountById(id);
        }

        public List<User> GetAll()
        {
            return accountRepository.GetAll();
        }

        public List<Club> GetAllClubs()
        {
            return accountRepository.GetAllClubs();
        }

        public int? GetClubIdByUsername(string username)
        {
           return accountRepository.GetClubIdByUsername(username);
        }

        public User Login(string username, string password)
        {
            User user = accountRepository.Login(username, password);

            if (user != null)
            {
                User.Current = user;

                return User.Current; 
            }

            return null;
        }
        public void UpdateAccount(User account)
        {
            accountRepository.UpdateAccount(account);
        }
        public List<User> GetLeadersByClubId()
        {
            int clubId = User.Current?.ClubId ?? 0;  
            return accountRepository.GetLeadersByClubId(clubId);
        }

        public void UpdateAccountRoleOnly(User user)
        {
            accountRepository.UpdateAccountRoleOnly(user);
        }
    }
}
