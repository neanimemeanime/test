// Файл: Services/IReportService.cs
using RepairServiceAppMVVM.Models;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace RepairServiceAppMVVM.Services
{
    // Класс для передачи параметров фильтрации
    public class ReportFilterOptions
    {
        public DateTime? StartDate { get; set; }
        public DateTime? EndDate { get; set; }
        public string? Status { get; set; }
        public int? AssignedToUserId { get; set; }
    }

    public interface IReportService
    {
        Task<List<Repair>> GetFilteredRepairsAsync(ReportFilterOptions options);
    }
}
