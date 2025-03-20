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
    public class AccountService : IAccountService
    {
        private readonly IAccountRepository _accountRepository;
        
        public AccountService(IAccountRepository accountRepository)
        {
            _accountRepository = accountRepository;
        }

        public void AddAccount(User account)
        {
            _accountRepository.Add(account);
        }

        public void DeleteAccount(int id)
        {
            _accountRepository.Delete(id);
        }

        public User GetAccountByEmail(string email)
        {
            return _accountRepository.GetByEmail(email);
        }

        public User GetAccountById(int id)
        {
            return _accountRepository.GetById(id);
        }

        public List<User> GetAll()
        {
            return accountRepository.GetAll();
        }

        public List<Club> GetAllClubs()
        {
            return _accountRepository.GetAllClubs().ToList();
        }

        public User Login(string username, string password)
        {
            return accountRepository.Login(username, password);
        }

        public void UpdateAccount(User account)
        {
            _accountRepository.Update(account);
        }
    }
}
