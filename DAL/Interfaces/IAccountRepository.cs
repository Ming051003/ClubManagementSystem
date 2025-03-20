using Model.Models;
using System.Collections.Generic;

namespace DAL.Interfaces
{
    public interface IAccountRepository
    {
        List<User> GetAll();
        User GetById(int id);
        void Add(User entity);
        void Update(User entity);
        void Delete(int id);
        User GetByEmail(string email);
        List<Club> GetAllClubs();
    }
}
