using BLL.Interfaces;
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
        public User GetAccountByEmail(string email)
        {
            return context.Users.FirstOrDefault(u => u.Email == email);
        }
    }
}
