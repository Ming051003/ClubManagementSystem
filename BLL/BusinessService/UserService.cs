using BLL.BusinessInterfaces;
using DAL.Interfaces;
using Model.Models;
using System;
using System.Collections.Generic;
using System.Linq;

namespace BLL.BusinessService
{
    public class UserService : IUserService
    {
        private readonly IUserRepository _userRepository;

        public UserService(IUserRepository userRepository)
        {
            _userRepository = userRepository;
        }

        public IEnumerable<User> GetAllUsers()
        {
            return _userRepository.GetAll();
        }

        public User GetUserById(int id)
        {
            return _userRepository.GetById(id);
        }

        public User GetUserByEmail(string email)
        {
            return _userRepository.GetAll().FirstOrDefault(u => u.Email.Equals(email, StringComparison.OrdinalIgnoreCase));
        }

        public IEnumerable<User> GetUsersByClub(int clubId)
        {
            return _userRepository.GetAll().Where(u => u.ClubId == clubId);
        }

        public void AddUser(User user)
        {
            // Validate user data
            if (string.IsNullOrEmpty(user.FullName))
                throw new ArgumentException("Full name is required");

            if (string.IsNullOrEmpty(user.Email))
                throw new ArgumentException("Email is required");

            if (string.IsNullOrEmpty(user.Password))
                throw new ArgumentException("Password is required");

            if (string.IsNullOrEmpty(user.Role))
                throw new ArgumentException("Role is required");

            // Check if email is already in use
            if (GetUserByEmail(user.Email) != null)
                throw new ArgumentException("Email is already in use");

            // Set default values if not provided
            if (user.JoinDate == null || user.JoinDate == DateOnly.MinValue)
                user.JoinDate = DateOnly.FromDateTime(DateTime.Now);

            _userRepository.Add(user);
        }

        public void UpdateUser(User user)
        {
            // Validate user data
            if (string.IsNullOrEmpty(user.FullName))
                throw new ArgumentException("Full name is required");

            if (string.IsNullOrEmpty(user.Email))
                throw new ArgumentException("Email is required");

            if (string.IsNullOrEmpty(user.Role))
                throw new ArgumentException("Role is required");

            // Check if email is already in use by another user
            var existingUser = GetUserByEmail(user.Email);
            if (existingUser != null && existingUser.UserId != user.UserId)
                throw new ArgumentException("Email is already in use by another user");

            _userRepository.Update(user);
        }

        public void DeleteUser(int id)
        {
            _userRepository.Delete(id);
        }

        public bool ValidateUser(string email, string password)
        {
            var user = GetUserByEmail(email);
            if (user == null)
                return false;

            // In a real application, you would use proper password hashing
            return user.Password == password && user.Status == true;
        }
    }
}
