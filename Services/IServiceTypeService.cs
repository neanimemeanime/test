// Файл: Services/IServiceTypeService.cs
using RepairServiceAppMVVM.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace RepairServiceAppMVVM.Services
{
    public interface IServiceTypeService
    {
        Task<List<ServiceType>> GetServiceTypesAsync();
        Task<ServiceType> AddServiceTypeAsync(ServiceType serviceType);
        Task<ServiceType> UpdateServiceTypeAsync(ServiceType serviceType);
        Task DeleteServiceTypeAsync(int serviceTypeId);
    }
}
