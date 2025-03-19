using BLL.BusinessInterfaces;
using BLL.Interfaces;
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
        public bool Login(string email, string password)
        {
            var user = accountRepository.GetAccountByEmail(email);
            if (user != null && user.Password == password)
            {
                return true;
            }
            return false;
        }
    }
}
