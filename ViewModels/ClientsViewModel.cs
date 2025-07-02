// Файл: ViewModels/ClientsViewModel.cs
// Описание: ViewModel для вкладки "Клиенты" с полной логикой CRUD.
using RepairServiceAppMVVM.Commands;
using RepairServiceAppMVVM.Models;
using RepairServiceAppMVVM.Services;
using System.Collections.ObjectModel;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;

namespace RepairServiceAppMVVM.ViewModels
{
    public class ClientsViewModel : NavigationViewModel
    {
        private readonly IClientService _clientService;
        private Client? _selectedClient;
        private bool _isFormVisible;

        public ObservableCollection<Client> Clients { get; } = new ObservableCollection<Client>();

        public Client? SelectedClient
        {
            get => _selectedClient;
            set
            {
                SetProperty(ref _selectedClient, value);
                // При выборе клиента в таблице, показываем форму для редактирования
                IsFormVisible = value != null;
            }
        }

        public bool IsFormVisible
        {
            get => _isFormVisible;
            set => SetProperty(ref _isFormVisible, value);
        }

        public ICommand LoadClientsCommand { get; }
        public ICommand NewClientCommand { get; }
        public ICommand SaveClientCommand { get; }
        public ICommand DeleteClientCommand { get; }
        public ICommand CancelCommand { get; }

        public ClientsViewModel(IClientService clientService)
        {
            _clientService = clientService;
            DisplayName = "Клиенты";

            LoadClientsCommand = new RelayCommand(async (_) => await LoadClientsAsync());

            // ИСПРАВЛЕНО: Обернуто в лямбда-выражение для соответствия сигнатуре Action<object?>
            NewClientCommand = new RelayCommand((_) => NewClient());
            CancelCommand = new RelayCommand((_) => Cancel());

            SaveClientCommand = new RelayCommand(async (_) => await SaveClientAsync(), (_) => SelectedClient != null);
            DeleteClientCommand = new RelayCommand(async (_) => await DeleteClientAsync(), (_) => SelectedClient != null);

            // Загружаем данные при создании ViewModel
            _ = LoadClientsAsync();
        }

        private async Task LoadClientsAsync()
        {
            Clients.Clear();
            var clients = await _clientService.GetClientsAsync();
            foreach (var client in clients)
            {
                Clients.Add(client);
            }
        }

        private void NewClient()
        {
            // Создаем новый объект и устанавливаем его как выбранный, чтобы форма заполнилась
            SelectedClient = new Client();
        }

        private async Task SaveClientAsync()
        {
            if (SelectedClient == null) return;

            if (string.IsNullOrWhiteSpace(SelectedClient.FirstName) || string.IsNullOrWhiteSpace(SelectedClient.LastName))
            {
                MessageBox.Show("Поля 'Имя' и 'Фамилия' обязательны для заполнения.", "Ошибка валидации");
                return;
            }

            if (SelectedClient.Id == 0) // ID = 0 означает, что это новый клиент
            {
                await _clientService.AddClientAsync(SelectedClient);
            }
            else // Иначе, обновляем существующего
            {
                await _clientService.UpdateClientAsync(SelectedClient);
            }

            // Перезагружаем список и скрываем форму
            await LoadClientsAsync();
            IsFormVisible = false;
            SelectedClient = null;
        }

        private async Task DeleteClientAsync()
        {
            if (SelectedClient == null) return;

            var result = MessageBox.Show($"Вы уверены, что хотите удалить клиента '{SelectedClient.FullName}'?", "Подтверждение удаления", MessageBoxButton.YesNo, MessageBoxImage.Warning);
            if (result == MessageBoxResult.Yes)
            {
                await _clientService.DeleteClientAsync(SelectedClient.Id);
                await LoadClientsAsync();
                SelectedClient = null;
            }
        }

        private void Cancel()
        {
            IsFormVisible = false;
            SelectedClient = null;
        }
    }
}