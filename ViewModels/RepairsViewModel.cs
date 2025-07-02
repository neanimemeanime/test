// Файл: ViewModels/RepairsViewModel.cs
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
    public class RepairsViewModel : NavigationViewModel
    {
        private readonly IRepairService _repairService;
        private readonly IClientService _clientService;
        private readonly IDeviceService _deviceService;
        private readonly IServiceTypeService _serviceTypeService;
        private readonly IUserService _userService;

        private Repair? _selectedRepair;
        private bool _isFormVisible;
        private int _selectedClientIdInForm;
        private int _selectedServiceTypeIdInForm;

        public ObservableCollection<Repair> Repairs { get; } = new ObservableCollection<Repair>();
        public ObservableCollection<Client> ClientsForComboBox { get; } = new ObservableCollection<Client>();
        public ObservableCollection<Device> DevicesForComboBox { get; } = new ObservableCollection<Device>();
        public ObservableCollection<ServiceType> ServiceTypesForComboBox { get; } = new ObservableCollection<ServiceType>();
        public ObservableCollection<User> UsersForComboBox { get; } = new ObservableCollection<User>();
        public ObservableCollection<string> Statuses { get; } = new ObservableCollection<string>
        {
            "В ожидании", "В работе", "Диагностика", "Ожидает запчасти", "Готов к выдаче", "Выдан", "Отменен"
        };

        public Repair? SelectedRepair
        {
            get => _selectedRepair;
            set
            {
                SetProperty(ref _selectedRepair, value);
                IsFormVisible = value != null;
                if (value != null)
                {
                    SelectedClientIdInForm = value.ClientId;
                    SelectedServiceTypeIdInForm = value.ServiceTypeId;
                }
            }
        }

        public int SelectedClientIdInForm
        {
            get => _selectedClientIdInForm;
            set
            {
                if (SetProperty(ref _selectedClientIdInForm, value))
                {
                    if (SelectedRepair != null) SelectedRepair.ClientId = value;
                    _ = LoadDevicesForClientAsync(value);
                }
            }
        }

        public int SelectedServiceTypeIdInForm
        {
            get => _selectedServiceTypeIdInForm;
            set
            {
                if (SetProperty(ref _selectedServiceTypeIdInForm, value))
                {
                    if (SelectedRepair != null) SelectedRepair.ServiceTypeId = value;
                    UpdatePriceFromSelectedService(value);
                }
            }
        }

        public bool IsFormVisible { get => _isFormVisible; set => SetProperty(ref _isFormVisible, value); }

        public ICommand NewRepairCommand { get; }
        public ICommand SaveRepairCommand { get; }
        public ICommand DeleteRepairCommand { get; }
        public ICommand CancelCommand { get; }

        public RepairsViewModel(IRepairService repairService, IClientService clientService, IDeviceService deviceService, IServiceTypeService serviceTypeService, IUserService userService)
        {
            _repairService = repairService;
            _clientService = clientService;
            _deviceService = deviceService;
            _serviceTypeService = serviceTypeService;
            _userService = userService;
            DisplayName = "Ремонты";

            NewRepairCommand = new RelayCommand((_) => NewRepair());
            SaveRepairCommand = new RelayCommand(async (_) => await SaveRepairAsync(), (_) => SelectedRepair != null);
            DeleteRepairCommand = new RelayCommand(async (_) => await DeleteRepairAsync(), (_) => SelectedRepair != null && SelectedRepair.Id != 0);
            CancelCommand = new RelayCommand((_) => Cancel());

            _ = LoadInitialDataAsync();
        }

        private async Task LoadInitialDataAsync()
        {
            await LoadRepairsAsync();
            await LoadComboBoxDataAsync();
        }

        private async Task LoadRepairsAsync()
        {
            Repairs.Clear();
            var repairs = await _repairService.GetRepairsAsync();
            foreach (var repair in repairs) Repairs.Add(repair);
        }

        private async Task LoadComboBoxDataAsync()
        {
            ClientsForComboBox.Clear();
            var clients = await _clientService.GetClientsAsync();
            foreach (var client in clients) ClientsForComboBox.Add(client);

            ServiceTypesForComboBox.Clear();
            var serviceTypes = await _serviceTypeService.GetServiceTypesAsync();
            foreach (var service in serviceTypes) ServiceTypesForComboBox.Add(service);

            UsersForComboBox.Clear();
            var users = await _userService.GetUsersAsync();
            foreach (var user in users) UsersForComboBox.Add(user);
        }

        private async Task LoadDevicesForClientAsync(int clientId)
        {
            DevicesForComboBox.Clear();
            if (clientId > 0)
            {
                var allDevices = await _deviceService.GetDevicesAsync();
                var clientDevices = allDevices.Where(d => d.ClientId == clientId).ToList();
                if (clientDevices.Any())
                {
                    foreach (var device in clientDevices) DevicesForComboBox.Add(device);
                }
                else
                {
                    DevicesForComboBox.Add(new Device { Id = 0, Model = "У клиента нет устройств" });
                }
            }
            else
            {
                DevicesForComboBox.Add(new Device { Id = 0, Model = "Сначала выберите клиента" });
            }

            if (SelectedRepair != null)
            {
                if (!DevicesForComboBox.Any(d => d.Id == SelectedRepair.DeviceId))
                {
                    SelectedRepair.DeviceId = DevicesForComboBox.First().Id;
                }
            }
        }

        private void UpdatePriceFromSelectedService(int serviceTypeId)
        {
            if (SelectedRepair == null) return;
            var selectedService = ServiceTypesForComboBox.FirstOrDefault(s => s.Id == serviceTypeId);
            if (selectedService != null)
            {
                SelectedRepair.TotalCost = selectedService.DefaultPrice;
            }
        }

        private void NewRepair()
        {
            // ИСПРАВЛЕНО: Устанавливаем SelectedRepair в новый объект, это покажет форму
            SelectedRepair = new Repair { DateReceived = DateTime.Now, Status = "В ожидании" };
        }

        private async Task SaveRepairAsync()
        {
            if (SelectedRepair == null) return;
            if (SelectedRepair.ClientId == 0 || SelectedRepair.DeviceId == 0 || SelectedRepair.ServiceTypeId == 0)
            {
                MessageBox.Show("Поля 'Клиент', 'Устройство' и 'Тип услуги' обязательны.", "Ошибка валидации");
                return;
            }

            if (SelectedRepair.Id == 0) await _repairService.AddRepairAsync(SelectedRepair);
            else
            {
                // Находим оригинальный объект в коллекции и обновляем его
                var originalRepair = Repairs.FirstOrDefault(r => r.Id == SelectedRepair.Id);
                if (originalRepair != null)
                {
                    // Копируем свойства из отредактированного объекта
                    originalRepair.ClientId = SelectedRepair.ClientId;
                    originalRepair.DeviceId = SelectedRepair.DeviceId;
                    originalRepair.ServiceTypeId = SelectedRepair.ServiceTypeId;
                    originalRepair.DateReceived = SelectedRepair.DateReceived;
                    originalRepair.DateDue = SelectedRepair.DateDue;
                    originalRepair.DateCompleted = SelectedRepair.DateCompleted;
                    originalRepair.Status = SelectedRepair.Status;
                    originalRepair.TotalCost = SelectedRepair.TotalCost;
                    originalRepair.AssignedToUserId = SelectedRepair.AssignedToUserId;
                    originalRepair.ProblemDescription = SelectedRepair.ProblemDescription;
                    originalRepair.WorkPerformed = SelectedRepair.WorkPerformed;
                    originalRepair.PartsUsed = SelectedRepair.PartsUsed;
                    originalRepair.Notes = SelectedRepair.Notes;

                    await _repairService.UpdateRepairAsync(originalRepair);
                }
            }

            await LoadRepairsAsync();
            Cancel();
        }

        private async Task DeleteRepairAsync()
        {
            if (SelectedRepair == null || SelectedRepair.Id == 0) return;
            var result = MessageBox.Show($"Удалить ремонт №{SelectedRepair.Id}?", "Подтверждение", MessageBoxButton.YesNo, MessageBoxImage.Warning);
            if (result == MessageBoxResult.Yes)
            {
                await _repairService.DeleteRepairAsync(SelectedRepair.Id);
                await LoadRepairsAsync();
                Cancel();
            }
        }

        private void Cancel()
        {
            IsFormVisible = false;
            SelectedRepair = null;
        }
    }
}
