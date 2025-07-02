// Файл: Services/IRepairService.cs
using RepairServiceAppMVVM.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace RepairServiceAppMVVM.Services
{
    public interface IRepairService
    {
        Task<List<Repair>> GetRepairsAsync();
        Task<Repair> AddRepairAsync(Repair repair);
        Task<Repair> UpdateRepairAsync(Repair repair);
        Task DeleteRepairAsync(int repairId);
    }
}
