// Файл: ViewModels/ReportsViewModel.cs
using Microsoft.Win32;
using RepairServiceAppMVVM.Commands;
using RepairServiceAppMVVM.Models;
using RepairServiceAppMVVM.Services;
using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;

namespace RepairServiceAppMVVM.ViewModels
{
    public class ReportsViewModel : NavigationViewModel
    {
        private readonly IReportService _reportService;
        private readonly IUserService _userService;
        private readonly ICsvExportService _csvExportService;
        private readonly IPrintingService _printingService; // Добавляем сервис печати

        private DateTime? _startDate;
        public DateTime? StartDate { get => _startDate; set => SetProperty(ref _startDate, value); }

        private DateTime? _endDate;
        public DateTime? EndDate { get => _endDate; set => SetProperty(ref _endDate, value); }

        private string _selectedStatus = "Все";
        public string SelectedStatus { get => _selectedStatus; set => SetProperty(ref _selectedStatus, value); }

        private User? _selectedUser;
        public User? SelectedUser { get => _selectedUser; set => SetProperty(ref _selectedUser, value); }

        private Repair? _selectedRepair; // Для выбора в таблице
        public Repair? SelectedRepair { get => _selectedRepair; set => SetProperty(ref _selectedRepair, value); }

        public ObservableCollection<string> Statuses { get; }
        public ObservableCollection<User> UsersForFilter { get; }
        public ObservableCollection<Repair> FilteredRepairs { get; } = new ObservableCollection<Repair>();

        private int _totalRepairsCount;
        public int TotalRepairsCount { get => _totalRepairsCount; set => SetProperty(ref _totalRepairsCount, value); }

        private decimal _totalRepairsCost;
        public decimal TotalRepairsCost { get => _totalRepairsCost; set => SetProperty(ref _totalRepairsCost, value); }

        public ICommand ApplyFilterCommand { get; }
        public ICommand ResetFilterCommand { get; }
        public ICommand ExportToCsvCommand { get; }
        public ICommand PrintReceiptCommand { get; } // Новая команда

        public ReportsViewModel(IReportService reportService, IUserService userService, ICsvExportService csvExportService, IPrintingService printingService)
        {
            _reportService = reportService;
            _userService = userService;
            _csvExportService = csvExportService;
            _printingService = printingService; // Сохраняем сервис
            DisplayName = "Отчеты";

            Statuses = new ObservableCollection<string> { "Все", "В ожидании", "В работе", "Диагностика", "Ожидает запчасти", "Готов к выдаче", "Выдан", "Отменен" };
            UsersForFilter = new ObservableCollection<User>();

            ApplyFilterCommand = new RelayCommand(async (_) => await ApplyFilterAsync());
            ResetFilterCommand = new RelayCommand(async (_) => await ResetFilterAsync());
            ExportToCsvCommand = new RelayCommand(async (_) => await ExportToCsvAsync(), (_) => FilteredRepairs.Any());
            PrintReceiptCommand = new RelayCommand((_) => PrintReceipt(), (_) => SelectedRepair != null); // Новая команда
        }

        public async Task LoadInitialDataAsync()
        {
            await LoadUsersForFilterAsync();
            await ApplyFilterAsync();
        }

        private async Task LoadUsersForFilterAsync()
        {
            UsersForFilter.Clear();
            UsersForFilter.Add(new User { Id = -1, Username = "Все" });
            var users = await _userService.GetUsersAsync();
            foreach (var user in users) UsersForFilter.Add(user);
            SelectedUser = UsersForFilter.First();
        }

        private async Task ApplyFilterAsync()
        {
            var options = new ReportFilterOptions { StartDate = this.StartDate, EndDate = this.EndDate, Status = this.SelectedStatus, AssignedToUserId = this.SelectedUser?.Id };
            var repairs = await _reportService.GetFilteredRepairsAsync(options);
            FilteredRepairs.Clear();
            foreach (var repair in repairs) FilteredRepairs.Add(repair);
            UpdateStatistics();
        }

        private async Task ResetFilterAsync()
        {
            StartDate = null;
            EndDate = null;
            SelectedStatus = "Все";
            SelectedUser = UsersForFilter.FirstOrDefault();
            await ApplyFilterAsync();
        }

        private void UpdateStatistics()
        {
            TotalRepairsCount = FilteredRepairs.Count;
            TotalRepairsCost = FilteredRepairs.Sum(r => r.TotalCost);
        }

        private async Task ExportToCsvAsync()
        {
            var saveFileDialog = new SaveFileDialog { Filter = "CSV файл (*.csv)|*.csv", FileName = $"Отчет_Ремонты_{DateTime.Now:yyyyMMdd_HHmmss}.csv" };
            if (saveFileDialog.ShowDialog() == true)
            {
                try
                {
                    await _csvExportService.ExportRepairsToCsvAsync(FilteredRepairs, saveFileDialog.FileName);
                    MessageBox.Show("Отчет успешно экспортирован.", "Экспорт", MessageBoxButton.OK, MessageBoxImage.Information);
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Ошибка при экспорте: {ex.Message}", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
        }

        private void PrintReceipt()
        {
            if (SelectedRepair != null)
            {
                _printingService.ShowPrintPreview(SelectedRepair);
            }
        }
    }
}
