// Файл: Services/ICsvExportService.cs
using RepairServiceAppMVVM.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace RepairServiceAppMVVM.Services
{
    public interface ICsvExportService
    {
        Task ExportRepairsToCsvAsync(IEnumerable<Repair> repairs, string filePath);
    }
}
