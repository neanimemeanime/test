// Файл: Services/IDataSeedingService.cs
using System.Threading.Tasks;

namespace RepairServiceAppMVVM.Services
{
    public interface IDataSeedingService
    {
        Task SeedDataAsync();
    }
}
