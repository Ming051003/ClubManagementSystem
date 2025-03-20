using Model.Models;
using System.Collections.Generic;

namespace BLL.BusinessInterfaces
{
    public interface IUserService
    {
        IEnumerable<User> GetAllUsers();
        User GetUserById(int id);
        User GetUserByEmail(string email);
        IEnumerable<User> GetUsersByClub(int clubId);
        void AddUser(User user);
        void UpdateUser(User user);
        void DeleteUser(int id);
        bool ValidateUser(string email, string password);
    }
}
