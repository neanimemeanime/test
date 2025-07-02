// Файл: ViewModels/DashboardViewModel.cs
using RepairServiceAppMVVM.Commands;
using RepairServiceAppMVVM.Models;
using RepairServiceAppMVVM.Services;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Input;
using System.Windows.Media;

namespace RepairServiceAppMVVM.ViewModels
{
    public class DashboardViewModel : NavigationViewModel
    {
        private readonly IDashboardService _dashboardService;
        private readonly Color[] _pieChartColors = { Colors.LightBlue, Colors.Orange, Colors.LightGreen, Colors.Red, Colors.Purple, Colors.Gold, Colors.Teal };

        private int _totalRepairs;
        public int TotalRepairs { get => _totalRepairs; set => SetProperty(ref _totalRepairs, value); }

        private int _totalClients;
        public int TotalClients { get => _totalClients; set => SetProperty(ref _totalClients, value); }

        private int _totalDevices;
        public int TotalDevices { get => _totalDevices; set => SetProperty(ref _totalDevices, value); }

        public ObservableCollection<StatusSummary> RepairsByStatus { get; } = new ObservableCollection<StatusSummary>();
        public ObservableCollection<PieSliceViewModel> PieSlices { get; } = new ObservableCollection<PieSliceViewModel>();

        public ICommand LoadStatsCommand { get; }

        public DashboardViewModel(IDashboardService dashboardService)
        {
            _dashboardService = dashboardService;
            DisplayName = "Панель управления";
            LoadStatsCommand = new RelayCommand(async (_) => await LoadStatsAsync());
            _ = LoadStatsAsync();
        }

        private async Task LoadStatsAsync()
        {
            var stats = await _dashboardService.GetDashboardStatsAsync();
            TotalRepairs = stats.TotalRepairs;
            TotalClients = stats.TotalClients;
            TotalDevices = stats.TotalDevices;

            RepairsByStatus.Clear();
            foreach (var item in stats.RepairsByStatus)
            {
                RepairsByStatus.Add(item);
            }

            UpdatePieChart();
        }

        private void UpdatePieChart()
        {
            PieSlices.Clear();
            if (RepairsByStatus.Count == 0) return;

            double totalCount = RepairsByStatus.Sum(s => s.Count);
            if (totalCount == 0) return;

            int colorIndex = 0;
            foreach (var status in RepairsByStatus)
            {
                var slice = new PieSliceViewModel
                {
                    Title = $"{status.StatusName} ({status.Count} шт.)",
                    Percentage = status.Count / totalCount,
                    Fill = new SolidColorBrush(_pieChartColors[colorIndex % _pieChartColors.Length])
                };
                PieSlices.Add(slice);
                colorIndex++;
            }
        }
    }
}
