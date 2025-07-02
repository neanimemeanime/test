// Файл: Services/DeviceService.cs
using Microsoft.EntityFrameworkCore;
using RepairServiceAppMVVM.Data;
using RepairServiceAppMVVM.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace RepairServiceAppMVVM.Services
{
    public class DeviceService : IDeviceService
    {
        private readonly ApplicationDbContext _dbContext;

        public DeviceService(ApplicationDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task<List<Device>> GetDevicesAsync()
        {
            return await _dbContext.Devices.Include(d => d.Client).ToListAsync();
        }

        public async Task<Device> AddDeviceAsync(Device device)
        {
            _dbContext.Devices.Add(device);
            await _dbContext.SaveChangesAsync();
            return device;
        }

        public async Task<Device> UpdateDeviceAsync(Device device)
        {
            _dbContext.Entry(device).State = EntityState.Modified;
            await _dbContext.SaveChangesAsync();
            return device;
        }

        public async Task DeleteDeviceAsync(int deviceId)
        {
            var device = await _dbContext.Devices.FindAsync(deviceId);
            if (device != null)
            {
                _dbContext.Devices.Remove(device);
                await _dbContext.SaveChangesAsync();
            }
        }
    }
}
