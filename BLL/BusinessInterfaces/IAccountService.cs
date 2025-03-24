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
        public List<User> GetAll();
        public User Login(string username, string password);
        public void AddAccount(User account);
        public void UpdateAccount(User account);
        public void DeleteAccount(int id);
        public User GetAccountById(int id);
        public List<Club> GetAllClubs();
        public int? GetClubIdByUsername(string username);
    }
}
