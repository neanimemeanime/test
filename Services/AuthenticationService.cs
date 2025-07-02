// Файл: Services/AuthenticationService.cs
using Microsoft.EntityFrameworkCore;
using RepairServiceAppMVVM.Data;
using RepairServiceAppMVVM.Models;
using System.Threading.Tasks;

namespace RepairServiceAppMVVM.Services
{
    public class AuthenticationService : IAuthenticationService
    {
        private readonly ApplicationDbContext _dbContext;

        // ИСПРАВЛЕНО: Внедряем DbContext через конструктор
        public AuthenticationService(ApplicationDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task<User?> AuthenticateAsync(string username, string password)
        {
            // ИСПРАВЛЕНО: Используем внедренный _dbContext, а не создаем новый
            User? user = await _dbContext.Users.FirstOrDefaultAsync(u => u.Username == username);

            if (user != null && BCrypt.Net.BCrypt.Verify(password, user.PasswordHash))
            {
                return user;
            }

            return null;
        }
    }
}