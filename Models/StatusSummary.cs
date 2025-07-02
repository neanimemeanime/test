// Файл: Models/StatusSummary.cs
// Описание: Простая модель для хранения сгруппированной статистики по статусам.
namespace RepairServiceAppMVVM.Models
{
    public class StatusSummary
    {
        public string StatusName { get; set; } = string.Empty;
        public int Count { get; set; }
        public decimal TotalCost { get; set; }
    }
}