using RepairServiceAppMVVM.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace RepairServiceAppMVVM.Services
{
    public interface IDashboardService
    {
        // Этот метод возвращает кортеж со всей необходимой статистикой
        Task<(int TotalRepairs, int TotalClients, int TotalDevices, List<StatusSummary> RepairsByStatus)> GetDashboardStatsAsync();
    }
}
