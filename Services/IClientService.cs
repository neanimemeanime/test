// Файл: Services/IClientService.cs
// Описание: "Контракт", описывающий, что должен уметь делать сервис для клиентов.
using RepairServiceAppMVVM.Models;

namespace RepairServiceAppMVVM.Services
{
    public interface IClientService
    {
        Task<List<Client>> GetClientsAsync();
        Task<Client> AddClientAsync(Client client);
        Task<Client> UpdateClientAsync(Client client);
        Task DeleteClientAsync(int clientId);
    }
}