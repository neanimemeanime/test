// Файл: Services/RepairService.cs
using Microsoft.EntityFrameworkCore;
using RepairServiceAppMVVM.Data;
using RepairServiceAppMVVM.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace RepairServiceAppMVVM.Services
{
    public class RepairService : IRepairService
    {
        private readonly ApplicationDbContext _dbContext;

        public RepairService(ApplicationDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task<List<Repair>> GetRepairsAsync()
        {
            return await _dbContext.Repairs
                .Include(r => r.Client)
                .Include(r => r.Device)
                .Include(r => r.ServiceType)
                .Include(r => r.AssignedToUser)
                .ToListAsync();
        }

        public async Task<Repair> AddRepairAsync(Repair repair)
        {
            _dbContext.Repairs.Add(repair);
            await _dbContext.SaveChangesAsync();
            return repair;
        }

        public async Task<Repair> UpdateRepairAsync(Repair repair)
        {
            _dbContext.Entry(repair).State = EntityState.Modified;
            await _dbContext.SaveChangesAsync();
            return repair;
        }

        public async Task DeleteRepairAsync(int repairId)
        {
            var repair = await _dbContext.Repairs.FindAsync(repairId);
            if (repair != null)
            {
                _dbContext.Repairs.Remove(repair);
                await _dbContext.SaveChangesAsync();
            }
        }
    }
}
