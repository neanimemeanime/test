// Файл: Services/UserService.cs
using Microsoft.EntityFrameworkCore;
using RepairServiceAppMVVM.Data;
using RepairServiceAppMVVM.Models;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace RepairServiceAppMVVM.Services
{
    public class UserService : IUserService
    {
        private readonly ApplicationDbContext _dbContext;

        public UserService(ApplicationDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task<List<User>> GetUsersAsync()
        {
            return await _dbContext.Users.ToListAsync();
        }

        public async Task<User> AddUserAsync(User user, string password)
        {
            user.PasswordHash = BCrypt.Net.BCrypt.HashPassword(password);
            _dbContext.Users.Add(user);
            await _dbContext.SaveChangesAsync();
            return user;
        }

        public async Task<User> UpdateUserAsync(User user, string? newPassword)
        {
            var userToUpdate = await _dbContext.Users.FindAsync(user.Id);
            if (userToUpdate != null)
            {
                userToUpdate.Username = user.Username;
                userToUpdate.Role = user.Role;

                if (!string.IsNullOrWhiteSpace(newPassword))
                {
                    userToUpdate.PasswordHash = BCrypt.Net.BCrypt.HashPassword(newPassword);
                }
                _dbContext.Entry(userToUpdate).State = EntityState.Modified;
                await _dbContext.SaveChangesAsync();
            }
            return userToUpdate!;
        }

        public async Task DeleteUserAsync(int userId)
        {
            var user = await _dbContext.Users.FindAsync(userId);
            if (user != null)
            {
                bool isAssigned = await _dbContext.Repairs.AnyAsync(r => r.AssignedToUserId == userId);
                if (isAssigned)
                {
                    throw new Exception("Невозможно удалить пользователя, так как он назначен на существующие ремонты.");
                }

                if (user.Role == "Администратор")
                {
                    var adminCount = await _dbContext.Users.CountAsync(u => u.Role == "Администратор");
                    if (adminCount <= 1)
                    {
                        throw new Exception("Невозможно удалить последнего администратора в системе.");
                    }
                }

                _dbContext.Users.Remove(user);
                await _dbContext.SaveChangesAsync();
            }
        }
    }
}
