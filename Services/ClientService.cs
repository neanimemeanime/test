// Файл: Services/ClientService.cs
using Microsoft.EntityFrameworkCore;
using RepairServiceAppMVVM.Data;
using RepairServiceAppMVVM.Models;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace RepairServiceAppMVVM.Services
{
    public class ClientService : IClientService
    {
        private readonly ApplicationDbContext _dbContext;

        public ClientService(ApplicationDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task<List<Client>> GetClientsAsync()
        {
            return await _dbContext.Clients.ToListAsync();
        }

        public async Task<Client> AddClientAsync(Client client)
        {
            _dbContext.Clients.Add(client);
            await _dbContext.SaveChangesAsync();
            return client;
        }

        public async Task<Client> UpdateClientAsync(Client client)
        {
            _dbContext.Entry(client).State = EntityState.Modified;
            await _dbContext.SaveChangesAsync();
            return client;
        }

        public async Task DeleteClientAsync(int clientId)
        {
            var client = await _dbContext.Clients.FindAsync(clientId);
            if (client != null)
            {
                var devices = await _dbContext.Devices.Where(d => d.ClientId == clientId).ToListAsync();
                _dbContext.Devices.RemoveRange(devices);

                _dbContext.Clients.Remove(client);
                await _dbContext.SaveChangesAsync();
            }
        }
    }
}
