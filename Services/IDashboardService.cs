// Файл: Services/IDashboardService.cs
using RepairServiceAppMVVM.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace RepairServiceAppMVVM.Services
{
    public interface IDashboardService
    {
        // Метод возвращает кортеж (tuple) со всей необходимой статистикой
        Task<(int TotalRepairs, int TotalClients, int TotalDevices, List<StatusSummary> RepairsByStatus)> GetDashboardStatsAsync();
    }
}
