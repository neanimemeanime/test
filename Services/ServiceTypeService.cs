// Файл: Services/ServiceTypeService.cs
using Microsoft.EntityFrameworkCore;
using RepairServiceAppMVVM.Data;
using RepairServiceAppMVVM.Models;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace RepairServiceAppMVVM.Services
{
    public class ServiceTypeService : IServiceTypeService
    {
        private readonly ApplicationDbContext _dbContext;

        public ServiceTypeService(ApplicationDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task<List<ServiceType>> GetServiceTypesAsync()
        {
            return await _dbContext.ServiceTypes.ToListAsync();
        }

        public async Task<ServiceType> AddServiceTypeAsync(ServiceType serviceType)
        {
            _dbContext.ServiceTypes.Add(serviceType);
            await _dbContext.SaveChangesAsync();
            return serviceType;
        }

        public async Task<ServiceType> UpdateServiceTypeAsync(ServiceType serviceType)
        {
            _dbContext.Entry(serviceType).State = EntityState.Modified;
            await _dbContext.SaveChangesAsync();
            return serviceType;
        }

        public async Task DeleteServiceTypeAsync(int serviceTypeId)
        {
            var serviceType = await _dbContext.ServiceTypes.FindAsync(serviceTypeId);
            if (serviceType != null)
            {
                bool isInUse = await _dbContext.Repairs.AnyAsync(r => r.ServiceTypeId == serviceTypeId);
                if (isInUse)
                {
                    throw new Exception("Невозможно удалить тип услуги, так как он используется в существующих ремонтах.");
                }

                _dbContext.ServiceTypes.Remove(serviceType);
                await _dbContext.SaveChangesAsync();
            }
        }
    }
}
