// Файл: ViewModels/ServicesViewModel.cs
using RepairServiceAppMVVM.Commands;
using RepairServiceAppMVVM.Models;
using RepairServiceAppMVVM.Services;
using System;
using System.Collections.ObjectModel;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;

namespace RepairServiceAppMVVM.ViewModels
{
    public class ServicesViewModel : NavigationViewModel
    {
        private readonly IServiceTypeService _serviceTypeService;
        private ServiceType? _selectedServiceType;
        private bool _isFormVisible;

        public ObservableCollection<ServiceType> ServiceTypes { get; } = new ObservableCollection<ServiceType>();

        public ServiceType? SelectedServiceType
        {
            get => _selectedServiceType;
            set
            {
                SetProperty(ref _selectedServiceType, value);
                IsFormVisible = value != null;
            }
        }

        public bool IsFormVisible
        {
            get => _isFormVisible;
            set => SetProperty(ref _isFormVisible, value);
        }

        public ICommand NewServiceTypeCommand { get; }
        public ICommand SaveServiceTypeCommand { get; }
        public ICommand DeleteServiceTypeCommand { get; }
        public ICommand CancelCommand { get; }

        public ServicesViewModel(IServiceTypeService serviceTypeService)
        {
            _serviceTypeService = serviceTypeService;
            DisplayName = "Услуги";

            NewServiceTypeCommand = new RelayCommand((_) => NewServiceType());
            SaveServiceTypeCommand = new RelayCommand(async (_) => await SaveServiceTypeAsync(), (_) => SelectedServiceType != null);
            DeleteServiceTypeCommand = new RelayCommand(async (_) => await DeleteServiceTypeAsync(), (_) => SelectedServiceType != null);
            CancelCommand = new RelayCommand((_) => Cancel());

            _ = LoadServiceTypesAsync();
        }

        private async Task LoadServiceTypesAsync()
        {
            ServiceTypes.Clear();
            var services = await _serviceTypeService.GetServiceTypesAsync();
            foreach (var service in services)
            {
                ServiceTypes.Add(service);
            }
        }

        private void NewServiceType()
        {
            SelectedServiceType = new ServiceType();
        }

        private async Task SaveServiceTypeAsync()
        {
            if (SelectedServiceType == null) return;

            if (string.IsNullOrWhiteSpace(SelectedServiceType.Name))
            {
                MessageBox.Show("Поле 'Название' обязательно для заполнения.", "Ошибка валидации");
                return;
            }

            if (SelectedServiceType.Id == 0)
            {
                await _serviceTypeService.AddServiceTypeAsync(SelectedServiceType);
            }
            else
            {
                await _serviceTypeService.UpdateServiceTypeAsync(SelectedServiceType);
            }

            await LoadServiceTypesAsync();
            Cancel();
        }

        private async Task DeleteServiceTypeAsync()
        {
            if (SelectedServiceType == null) return;

            var result = MessageBox.Show($"Вы уверены, что хотите удалить услугу '{SelectedServiceType.Name}'?", "Подтверждение удаления", MessageBoxButton.YesNo, MessageBoxImage.Warning);
            if (result == MessageBoxResult.Yes)
            {
                try
                {
                    await _serviceTypeService.DeleteServiceTypeAsync(SelectedServiceType.Id);
                    await LoadServiceTypesAsync();
                    Cancel();
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message, "Ошибка удаления", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
        }

        private void Cancel()
        {
            IsFormVisible = false;
            SelectedServiceType = null;
        }
    }
}