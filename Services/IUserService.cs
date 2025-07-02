// Файл: Services/IUserService.cs
using RepairServiceAppMVVM.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace RepairServiceAppMVVM.Services
{
    public interface IUserService
    {
        Task<List<User>> GetUsersAsync();
        Task<User> AddUserAsync(User user, string password);
        Task<User> UpdateUserAsync(User user, string? password);
        Task DeleteUserAsync(int userId);
    }
}
