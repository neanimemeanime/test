// Файл: Services/CsvExportService.cs
using RepairServiceAppMVVM.Models;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Text;
using System.Threading.Tasks;

namespace RepairServiceAppMVVM.Services
{
    public class CsvExportService : ICsvExportService
    {
        public async Task ExportRepairsToCsvAsync(IEnumerable<Repair> repairs, string filePath)
        {
            var builder = new StringBuilder();
            // Заголовок CSV
            builder.AppendLine("ID;Клиент;Устройство;Тип услуги;Дата приема;Статус;Итоговая стоимость;Назначен;Описание проблемы;Выполненные работы;Использованные детали;Примечания");

            foreach (var repair in repairs)
            {
                builder.AppendLine($"{repair.Id};" +
                                 $"{Sanitize(repair.Client?.FullName)};" +
                                 $"{Sanitize(repair.Device?.Model)};" +
                                 $"{Sanitize(repair.ServiceType?.Name)};" +
                                 $"{repair.DateReceived:dd.MM.yyyy};" +
                                 $"{Sanitize(repair.Status)};" +
                                 $"{repair.TotalCost.ToString(CultureInfo.InvariantCulture)};" +
                                 $"{Sanitize(repair.AssignedToUser?.Username)};" +
                                 $"{Sanitize(repair.Device?.ProblemDescription)};" +
                                 $"{Sanitize(repair.WorkPerformed)};" +
                                 $"{Sanitize(repair.PartsUsed)};" +
                                 $"{Sanitize(repair.Notes)}");
            }

            // Используем UTF8 с BOM, чтобы Excel корректно распознавал кириллицу
            await File.WriteAllTextAsync(filePath, builder.ToString(), new UTF8Encoding(true));
        }

        // Вспомогательный метод для очистки строк для CSV
        private string Sanitize(string? text)
        {
            if (string.IsNullOrEmpty(text))
            {
                return string.Empty;
            }
            // Если текст содержит точку с запятой или кавычки, обрамляем его в кавычки
            if (text.Contains(';') || text.Contains('"'))
            {
                return $"\"{text.Replace("\"", "\"\"")}\"";
            }
            return text;
        }
    }
}