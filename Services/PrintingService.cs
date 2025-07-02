// Файл: Services/PrintingService.cs
using RepairServiceAppMVVM.Models;
using RepairServiceAppMVVM.Views;
using System.Globalization;
using System.Windows;
using System.Windows.Documents;
using System.Windows.Media;

namespace RepairServiceAppMVVM.Services
{
    public class PrintingService : IPrintingService
    {
        public void ShowPrintPreview(Repair repair)
        {
            // 1. Генерируем FlowDocument
            FlowDocument receiptDocument = GenerateReceiptDocument(repair);

            // 2. Создаем и показываем окно для предпросмотра
            ReceiptView receiptWindow = new ReceiptView(receiptDocument);
            receiptWindow.Owner = Application.Current.MainWindow;
            receiptWindow.ShowDialog();
        }

        private FlowDocument GenerateReceiptDocument(Repair repair)
        {
            var doc = new FlowDocument
            {
                PagePadding = new Thickness(40),
                FontFamily = new FontFamily("Segoe UI"),
                FontSize = 12,
                LineHeight = 16
            };

            doc.Blocks.Add(new Paragraph(new Run("Квитанция о ремонте")) { FontSize = 18, FontWeight = FontWeights.Bold, TextAlignment = TextAlignment.Center });
            doc.Blocks.Add(new Paragraph(new Run($"№ Ремонта: {repair.Id}")) { TextAlignment = TextAlignment.Center });
            doc.Blocks.Add(new Paragraph(new Run($"Дата приема: {repair.DateReceived:dd.MM.yyyy HH:mm}")) { TextAlignment = TextAlignment.Center });

            doc.Blocks.Add(new BlockUIContainer(new System.Windows.Shapes.Rectangle { Height = 1, Fill = Brushes.Black, Margin = new Thickness(0, 10, 0, 10) }));

            doc.Blocks.Add(new Paragraph(new Run("Информация о клиенте:")) { FontWeight = FontWeights.Bold });
            doc.Blocks.Add(new Paragraph(new Run($"ФИО: {repair.Client?.FullName ?? "N/A"}")));
            doc.Blocks.Add(new Paragraph(new Run($"Телефон: {repair.Client?.PhoneNumber ?? "N/A"}")));

            doc.Blocks.Add(new Paragraph(new Run("Информация об устройстве:")) { FontWeight = FontWeights.Bold, Margin = new Thickness(0, 10, 0, 0) });
            doc.Blocks.Add(new Paragraph(new Run($"Модель: {repair.Device?.Model ?? "N/A"}")));
            doc.Blocks.Add(new Paragraph(new Run($"Описание проблемы: {repair.Device?.ProblemDescription ?? "N/A"}")));

            doc.Blocks.Add(new Paragraph(new Run("Детали ремонта:")) { FontWeight = FontWeights.Bold, Margin = new Thickness(0, 10, 0, 0) });
            doc.Blocks.Add(new Paragraph(new Run($"Услуга: {repair.ServiceType?.Name ?? "N/A"}")));
            doc.Blocks.Add(new Paragraph(new Run($"Статус: {repair.Status ?? "N/A"}")));
            doc.Blocks.Add(new Paragraph(new Run($"Выполненные работы: {repair.WorkPerformed ?? "Нет"}")));

            doc.Blocks.Add(new BlockUIContainer(new System.Windows.Shapes.Rectangle { Height = 1, Fill = Brushes.Black, Margin = new Thickness(0, 20, 0, 10) }));

            doc.Blocks.Add(new Paragraph(new Run($"Итоговая стоимость: {repair.TotalCost.ToString("C", CultureInfo.GetCultureInfo("ru-RU"))}")) { FontSize = 14, FontWeight = FontWeights.Bold, TextAlignment = TextAlignment.Right });

            return doc;
        }
    }
}
