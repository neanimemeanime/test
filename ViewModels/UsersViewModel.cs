// Файл: ViewModels/UsersViewModel.cs
using RepairServiceAppMVVM.Commands;
using RepairServiceAppMVVM.Models;
using RepairServiceAppMVVM.Services;
using System;
using System.Collections.ObjectModel;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace RepairServiceAppMVVM.ViewModels
{
    public class UsersViewModel : NavigationViewModel
    {
        private readonly IUserService _userService;
        private User? _selectedUser;
        private bool _isFormVisible;

        public ObservableCollection<User> Users { get; } = new ObservableCollection<User>();
        public ObservableCollection<string> Roles { get; } = new ObservableCollection<string> { "Администратор", "Техник", "Менеджер" };

        public User? SelectedUser
        {
            get => _selectedUser;
            set
            {
                SetProperty(ref _selectedUser, value);
                IsFormVisible = value != null;
            }
        }

        public bool IsFormVisible
        {
            get => _isFormVisible;
            set => SetProperty(ref _isFormVisible, value);
        }

        public ICommand NewUserCommand { get; }
        public ICommand SaveUserCommand { get; }
        public ICommand DeleteUserCommand { get; }
        public ICommand CancelCommand { get; }

        public UsersViewModel(IUserService userService)
        {
            _userService = userService;
            DisplayName = "Пользователи";

            NewUserCommand = new RelayCommand((_) => NewUser());
            SaveUserCommand = new RelayCommand(async (p) => await SaveUserAsync(p), (_) => SelectedUser != null);
            DeleteUserCommand = new RelayCommand(async (_) => await DeleteUserAsync(), (_) => SelectedUser != null);
            CancelCommand = new RelayCommand((_) => Cancel());

            _ = LoadUsersAsync();
        }

        private async Task LoadUsersAsync()
        {
            Users.Clear();
            var users = await _userService.GetUsersAsync();
            foreach (var user in users)
            {
                Users.Add(user);
            }
        }

        private void NewUser()
        {
            SelectedUser = new User();
        }

        private async Task SaveUserAsync(object? parameter)
        {
            if (SelectedUser == null || parameter is not PasswordBox passwordBox) return;

            if (string.IsNullOrWhiteSpace(SelectedUser.Username) || string.IsNullOrWhiteSpace(SelectedUser.Role))
            {
                MessageBox.Show("Поля 'Логин' и 'Роль' обязательны.", "Ошибка валидации");
                return;
            }

            string? password = passwordBox.Password;
            if (SelectedUser.Id == 0 && string.IsNullOrWhiteSpace(password))
            {
                MessageBox.Show("Для нового пользователя необходимо задать пароль.", "Ошибка валидации");
                return;
            }

            try
            {
                if (SelectedUser.Id == 0)
                {
                    await _userService.AddUserAsync(SelectedUser, password!);
                }
                else
                {
                    await _userService.UpdateUserAsync(SelectedUser, string.IsNullOrWhiteSpace(password) ? null : password);
                }

                await LoadUsersAsync();
                Cancel();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка сохранения: {ex.Message}", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private async Task DeleteUserAsync()
        {
            if (SelectedUser == null) return;

            var result = MessageBox.Show($"Удалить пользователя '{SelectedUser.Username}'?", "Подтверждение", MessageBoxButton.YesNo, MessageBoxImage.Warning);
            if (result == MessageBoxResult.Yes)
            {
                try
                {
                    await _userService.DeleteUserAsync(SelectedUser.Id);
                    await LoadUsersAsync();
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
            SelectedUser = null;
        }
    }
}
