using RepairServiceAppMVVM.Models;
using RepairServiceAppMVVM.Services;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media; // Убедитесь, что эта директива есть

namespace RepairServiceAppMVVM.ViewModels
{
    public class DashboardViewModel : NavigationViewModel
    {
        private readonly IDashboardService _dashboardService;
        private int _totalRepairs;
        private int _repairsInProgress;
        private int _repairsCompleted;
        private int _totalClients;
        private int _totalDevices;
        private ObservableCollection<PieSliceViewModel> _statusSlices;

        public int TotalRepairs
        {
            get => _totalRepairs;
            set { _totalRepairs = value; OnPropertyChanged(); }
        }

        public int RepairsInProgress
        {
            get => _repairsInProgress;
            set { _repairsInProgress = value; OnPropertyChanged(); }
        }

        public int RepairsCompleted
        {
            get => _repairsCompleted;
            set { _repairsCompleted = value; OnPropertyChanged(); }
        }

        public int TotalClients
        {
            get => _totalClients;
            set { _totalClients = value; OnPropertyChanged(); }
        }

        public int TotalDevices
        {
            get => _totalDevices;
            set { _totalDevices = value; OnPropertyChanged(); }
        }

        public ObservableCollection<PieSliceViewModel> StatusSlices
        {
            get => _statusSlices;
            set { _statusSlices = value; OnPropertyChanged(); }
        }

        public DashboardViewModel(IDashboardService dashboardService)
        {
            _dashboardService = dashboardService;
            StatusSlices = new ObservableCollection<PieSliceViewModel>();
            // Дополнительный вызов LoadDashboardDataAsync можно добавить здесь,
            // если вы хотите, чтобы данные загружались сразу при создании ViewModel.
            // Если вы загружаете данные по команде или через событие, оставьте как есть.
        }

        public async Task LoadDashboardDataAsync()
        {
            try
            {
                var stats = await _dashboardService.GetDashboardStatsAsync();

                TotalRepairs = stats.TotalRepairs;
                TotalClients = stats.TotalClients;
                TotalDevices = stats.TotalDevices;

                // --- Расчет ремонтов в работе и завершенных ---
                var activeStatuses = new List<string> { "В работе", "Диагностика", "Ожидает запчасти", "Готов к выдаче" };
                RepairsInProgress = stats.RepairsByStatus
                                             .Where(s => activeStatuses.Contains(s.StatusName))
                                             .Sum(s => s.Count);

                RepairsCompleted = stats.RepairsByStatus.FirstOrDefault(s => s.StatusName == "Выдан")?.Count ?? 0;

                var newSlices = new ObservableCollection<PieSliceViewModel>();
                var totalForChart = (double)stats.RepairsByStatus.Sum(s => s.Count);

                if (totalForChart > 0)
                {
                    // --- НОВАЯ ЦВЕТОВАЯ ПАЛИТРА ---
                    var colors = new List<Brush>
                    {
                        // Пример палитры (на основе Material Design, но слегка приглушенной)
                        // Оттенки синего
                        new SolidColorBrush(Color.FromRgb(33, 150, 243)),  // Blue 500
                        new SolidColorBrush(Color.FromRgb(63, 81, 181)),   // Indigo 500
                        // Оттенки зеленого/акценты
                        new SolidColorBrush(Color.FromRgb(76, 175, 80)),   // Green 500
                        new SolidColorBrush(Color.FromRgb(0, 150, 136)),   // Teal 500
                        // Оттенки оранжевого/красного/теплого
                        new SolidColorBrush(Color.FromRgb(255, 152, 0)),   // Orange 500
                        new SolidColorBrush(Color.FromRgb(255, 87, 34)),   // Deep Orange 500
                        // Нейтральные/дополнительные
                        new SolidColorBrush(Color.FromRgb(158, 158, 158)), // Grey 500
                        new SolidColorBrush(Color.FromRgb(96, 125, 139))    // Blue Grey 500
                        // Вы можете искать "material design color palette" или "flat UI colors"
                        // для вдохновения.
                    };
                    int colorIndex = 0;

                    foreach (var summary in stats.RepairsByStatus)
                    {
                        newSlices.Add(new PieSliceViewModel
                        {
                            Title = summary.StatusName,
                            Count = summary.Count,
                            Percentage = (summary.Count / totalForChart),
                            Brush = colors[colorIndex % colors.Count] // Назначаем цвет из новой палитры
                        });
                        colorIndex++;
                    }
                }

                StatusSlices = newSlices; // Присваиваем новую коллекцию
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Произошла ошибка при загрузке данных для панели управления:\n\n{ex.Message}",
                                 "Ошибка загрузки", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }
    }
}