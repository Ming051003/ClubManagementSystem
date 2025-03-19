using Model.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BLL.BusinessInterfaces
{
    public interface IAccountService
    {
        bool Login(string email, string password);
        public void AddAccount(User account);
        public void UpdateAccount(User account);
        public void DeleteAccount(int id);
        public User GetAccountById(int id);
        public List<Club> GetAllClubs();

    }
}
