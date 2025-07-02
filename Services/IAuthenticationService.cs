// Файл: Services/IAuthenticationService.cs
// Описание: Это "контракт" или интерфейс. Он говорит, что любой сервис
// аутентификации должен уметь делать (в нашем случае - проверять пользователя).
// Это нужно для гибкости и тестирования.
using RepairServiceAppMVVM.Models;

namespace RepairServiceAppMVVM.Services
{
    public interface IAuthenticationService
    {
        Task<User?> AuthenticateAsync(string username, string password);
    }
}