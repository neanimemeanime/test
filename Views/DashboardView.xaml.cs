using RepairServiceAppMVVM.ViewModels;
using System.Windows;
using System.Windows.Controls;

namespace RepairServiceAppMVVM.Views
{
    /// <summary>
    /// Логика взаимодействия для DashboardView.xaml
    /// </summary>
    public partial class DashboardView : UserControl
    {
        public DashboardView()
        {
            InitializeComponent();
            // Подписываемся на событие загрузки UserControl
            this.Loaded += DashboardView_Loaded;
        }

        private async void DashboardView_Loaded(object sender, RoutedEventArgs e)
        {
            // Проверяем, что DataContext - это наша ViewModel
            if (DataContext is DashboardViewModel viewModel)
            {
                // Вызываем асинхронный метод загрузки данных
                await viewModel.LoadDashboardDataAsync();
            }
        }
    }
}
