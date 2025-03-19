using BLL.BusinessInterfaces;
using BLL.Interfaces;
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

        public List<Club> GetAllClubs()
        {
            return accountRepository.GetAllClubs();
        }

        public bool Login(string email, string password)
        {
            var user = accountRepository.GetAccountByEmail(email);
            if (user != null && user.Password == password)
            {
                return true;
            }
            return false;
        }

        public void UpdateAccount(User account)
        {
            accountRepository.UpdateAccount(account);
        }
    }
}
