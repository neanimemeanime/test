// Файл: Views/ReceiptView.xaml.cs
using System.Printing;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;

namespace RepairServiceAppMVVM.Views
{
    public partial class ReceiptView : Window
    {
        public ReceiptView(FlowDocument document)
        {
            InitializeComponent();
            ReceiptDocumentReader.Document = document;
        }

        private void PrintButton_Click(object sender, RoutedEventArgs e)
        {
            var printDialog = new PrintDialog();
            if (printDialog.ShowDialog() == true)
            {
                // Отключаем постраничный просмотр для корректной печати
                ReceiptDocumentReader.ViewingMode = FlowDocumentReaderViewingMode.Scroll;
                IDocumentPaginatorSource document = ReceiptDocumentReader.Document;
                printDialog.PrintDocument(document.DocumentPaginator, "Квитанция о ремонте");
                ReceiptDocumentReader.ViewingMode = FlowDocumentReaderViewingMode.Page;
            }
        }

        private void CloseButton_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }
    }
}