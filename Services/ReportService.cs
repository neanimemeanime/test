// Файл: Services/ReportService.cs
using Microsoft.EntityFrameworkCore;
using RepairServiceAppMVVM.Data;
using RepairServiceAppMVVM.Models;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace RepairServiceAppMVVM.Services
{
    public class ReportService : IReportService
    {
        private readonly ApplicationDbContext _dbContext;

        public ReportService(ApplicationDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task<List<Repair>> GetFilteredRepairsAsync(ReportFilterOptions options)
        {
            var query = _dbContext.Repairs
                .Include(r => r.Client)
                .Include(r => r.Device)
                .Include(r => r.ServiceType)
                .Include(r => r.AssignedToUser)
                .AsQueryable();

            if (options.StartDate.HasValue)
            {
                query = query.Where(r => r.DateReceived >= options.StartDate.Value);
            }

            if (options.EndDate.HasValue)
            {
                var endDate = options.EndDate.Value.AddDays(1);
                query = query.Where(r => r.DateReceived < endDate);
            }

            if (!string.IsNullOrEmpty(options.Status) && options.Status != "Все")
            {
                query = query.Where(r => r.Status == options.Status);
            }

            if (options.AssignedToUserId.HasValue && options.AssignedToUserId.Value != -1)
            {
                query = query.Where(r => r.AssignedToUserId == options.AssignedToUserId.Value);
            }

            return await query.ToListAsync();
        }
    }
}
