// Файл: Services/IDeviceService.cs
using RepairServiceAppMVVM.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace RepairServiceAppMVVM.Services
{
    public interface IDeviceService
    {
        Task<List<Device>> GetDevicesAsync();
        Task<Device> AddDeviceAsync(Device device);
        Task<Device> UpdateDeviceAsync(Device device);
        Task DeleteDeviceAsync(int deviceId);
    }
}