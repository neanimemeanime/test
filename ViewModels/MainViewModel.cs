// Файл: ViewModels/MainViewModel.cs
using RepairServiceAppMVVM.Models;
using RepairServiceAppMVVM.Services;
using System.Collections.ObjectModel;
using System.Linq;

namespace RepairServiceAppMVVM.ViewModels
{
    public class MainViewModel : ViewModelBase
    {
        private User? _currentUser;
        private NavigationViewModel? _selectedViewModel;

        public ObservableCollection<NavigationViewModel> TabViewModels { get; } = new ObservableCollection<NavigationViewModel>();

        public NavigationViewModel? SelectedViewModel
        {
            get => _selectedViewModel;
            set
            {
                SetProperty(ref _selectedViewModel, value);
                if (value is ReportsViewModel rvm)
                {
                    _ = rvm.LoadInitialDataAsync();
                }
            }
        }

        private string _userInfo = string.Empty;
        public string UserInfo
        {
            get => _userInfo;
            set => SetProperty(ref _userInfo, value);
        }

        // Сервисы
        private readonly IClientService _clientService;
        private readonly IServiceTypeService _serviceTypeService;
        private readonly IUserService _userService;
        private readonly IDeviceService _deviceService;
        private readonly IRepairService _repairService;
        private readonly IDashboardService _dashboardService;
        private readonly IReportService _reportService;
        private readonly ICsvExportService _csvExportService;
        private readonly IPrintingService _printingService;

        // Конструктор для Dependency Injection
        public MainViewModel(
            IClientService clientService, IServiceTypeService serviceTypeService, IUserService userService,
            IDeviceService deviceService, IRepairService repairService, IDashboardService dashboardService,
            IReportService reportService, ICsvExportService csvExportService, IPrintingService printingService)
        {
            _clientService = clientService;
            _serviceTypeService = serviceTypeService;
            _userService = userService;
            _deviceService = deviceService;
            _repairService = repairService;
            _dashboardService = dashboardService;
            _reportService = reportService;
            _csvExportService = csvExportService;
            _printingService = printingService;
        }

        // Метод для инициализации после создания ViewModel
        public void Initialize(User currentUser)
        {
            _currentUser = currentUser;
            UserInfo = $"Текущий пользователь: {_currentUser.Username} ({_currentUser.Role})";

            TabViewModels.Clear();
            TabViewModels.Add(new DashboardViewModel(_dashboardService) { DisplayName = "Панель управления" });
            TabViewModels.Add(new RepairsViewModel(_repairService, _clientService, _deviceService, _serviceTypeService, _userService) { DisplayName = "Ремонты" });
            TabViewModels.Add(new ClientsViewModel(_clientService) { DisplayName = "Клиенты" });
            TabViewModels.Add(new DevicesViewModel(_deviceService, _clientService) { DisplayName = "Устройства" });
            TabViewModels.Add(new ServicesViewModel(_serviceTypeService) { DisplayName = "Услуги" });
            TabViewModels.Add(new UsersViewModel(_userService) { DisplayName = "Пользователи" });
            TabViewModels.Add(new ReportsViewModel(_reportService, _userService, _csvExportService, _printingService) { DisplayName = "Отчеты" });

            SelectedViewModel = TabViewModels.FirstOrDefault();
        }
    }
}
