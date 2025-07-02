// Файл: ViewModels/DevicesViewModel.cs
using RepairServiceAppMVVM.Commands;
using RepairServiceAppMVVM.Models;
using RepairServiceAppMVVM.Services;
using System.Collections.ObjectModel;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;

namespace RepairServiceAppMVVM.ViewModels
{
    public class DevicesViewModel : NavigationViewModel
    {
        private readonly IDeviceService _deviceService;
        private readonly IClientService _clientService; // Нужен для загрузки списка клиентов
        private Device? _selectedDevice;
        private bool _isFormVisible;

        public ObservableCollection<Device> Devices { get; } = new ObservableCollection<Device>();
        public ObservableCollection<Client> ClientsForComboBox { get; } = new ObservableCollection<Client>();

        public Device? SelectedDevice
        {
            get => _selectedDevice;
            set
            {
                SetProperty(ref _selectedDevice, value);
                IsFormVisible = value != null;
            }
        }

        public bool IsFormVisible
        {
            get => _isFormVisible;
            set => SetProperty(ref _isFormVisible, value);
        }

        public ICommand NewDeviceCommand { get; }
        public ICommand SaveDeviceCommand { get; }
        public ICommand DeleteDeviceCommand { get; }
        public ICommand CancelCommand { get; }

        public DevicesViewModel(IDeviceService deviceService, IClientService clientService)
        {
            _deviceService = deviceService;
            _clientService = clientService;
            DisplayName = "Устройства";

            NewDeviceCommand = new RelayCommand((_) => NewDevice());
            SaveDeviceCommand = new RelayCommand(async (_) => await SaveDeviceAsync(), (_) => SelectedDevice != null);
            DeleteDeviceCommand = new RelayCommand(async (_) => await DeleteDeviceAsync(), (_) => SelectedDevice != null);
            CancelCommand = new RelayCommand((_) => Cancel());

            // Загружаем данные при создании
            _ = LoadInitialDataAsync();
        }

        private async Task LoadInitialDataAsync()
        {
            await LoadDevicesAsync();
            await LoadClientsForComboBoxAsync();
        }

        private async Task LoadDevicesAsync()
        {
            Devices.Clear();
            var devices = await _deviceService.GetDevicesAsync();
            foreach (var device in devices)
            {
                Devices.Add(device);
            }
        }

        private async Task LoadClientsForComboBoxAsync()
        {
            ClientsForComboBox.Clear();
            var clients = await _clientService.GetClientsAsync();
            foreach (var client in clients)
            {
                ClientsForComboBox.Add(client);
            }
        }

        private void NewDevice()
        {
            SelectedDevice = new Device();
        }

        private async Task SaveDeviceAsync()
        {
            if (SelectedDevice == null) return;

            if (SelectedDevice.ClientId == 0 || string.IsNullOrWhiteSpace(SelectedDevice.Type) || string.IsNullOrWhiteSpace(SelectedDevice.Model))
            {
                MessageBox.Show("Поля 'Клиент', 'Тип' и 'Модель' обязательны для заполнения.", "Ошибка валидации");
                return;
            }

            if (SelectedDevice.Id == 0)
            {
                await _deviceService.AddDeviceAsync(SelectedDevice);
            }
            else
            {
                await _deviceService.UpdateDeviceAsync(SelectedDevice);
            }

            await LoadDevicesAsync();
            Cancel();
        }

        private async Task DeleteDeviceAsync()
        {
            if (SelectedDevice == null) return;

            var result = MessageBox.Show($"Удалить устройство '{SelectedDevice.Model}'?", "Подтверждение", MessageBoxButton.YesNo, MessageBoxImage.Warning);
            if (result == MessageBoxResult.Yes)
            {
                await _deviceService.DeleteDeviceAsync(SelectedDevice.Id);
                await LoadDevicesAsync();
                Cancel();
            }
        }

        private void Cancel()
        {
            IsFormVisible = false;
            SelectedDevice = null;
        }
    }
}