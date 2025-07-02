// Файл: Services/DashboardService.cs
using Microsoft.EntityFrameworkCore;
using RepairServiceAppMVVM.Data;
using RepairServiceAppMVVM.Models;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace RepairServiceAppMVVM.Services
{
    public class DashboardService : IDashboardService
    {
        private readonly ApplicationDbContext _dbContext;

        public DashboardService(ApplicationDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task<(int TotalRepairs, int TotalClients, int TotalDevices, List<StatusSummary> RepairsByStatus)> GetDashboardStatsAsync()
        {
            var totalRepairs = await _dbContext.Repairs.CountAsync();
            var totalClients = await _dbContext.Clients.CountAsync();
            var totalDevices = await _dbContext.Devices.CountAsync();

            var repairsByStatus = await _dbContext.Repairs
                .GroupBy(r => r.Status)
                .Select(g => new StatusSummary
                {
                    StatusName = g.Key,
                    Count = g.Count(),
                    TotalCost = g.Sum(r => r.TotalCost)
                })
                .OrderBy(s => s.StatusName)
                .ToListAsync();

            return (totalRepairs, totalClients, totalDevices, repairsByStatus);
        }
    }
}
