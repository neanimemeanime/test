// Файл: ViewModels/LoginViewModel.cs
// Описание: "Мозг" для LoginView. Теперь он сообщает о успешном входе через событие.
using RepairServiceAppMVVM.Commands;
using RepairServiceAppMVVM.Models;
using RepairServiceAppMVVM.Services;
using System.Windows.Controls;
using System.Windows.Input;

namespace RepairServiceAppMVVM.ViewModels
{
    public class LoginViewModel : ViewModelBase
    {
        private readonly IAuthenticationService _authenticationService;

        // Событие, которое будет срабатывать при успешном входе
        public event Action<User>? LoginSuccess;

        private string _username = string.Empty;
        public string Username
        {
            get => _username;
            set => SetProperty(ref _username, value);
        }

        private bool _isLoading;
        public bool IsLoading
        {
            get => _isLoading;
            set => SetProperty(ref _isLoading, value);
        }

        private string _errorMessage = string.Empty;
        public string ErrorMessage
        {
            get => _errorMessage;
            set
            {
                SetProperty(ref _errorMessage, value);
                OnPropertyChanged(nameof(HasError));
            }
        }

        public bool HasError => !string.IsNullOrEmpty(ErrorMessage);

        public ICommand LoginCommand { get; }

        public LoginViewModel(IAuthenticationService authenticationService)
        {
            _authenticationService = authenticationService;
            LoginCommand = new RelayCommand(async (parameter) => await LoginAsync(parameter), (parameter) => CanLogin());
        }

        private bool CanLogin()
        {
            return !IsLoading;
        }

        private async Task LoginAsync(object? parameter)
        {
            if (parameter is not PasswordBox passwordBox) return;

            IsLoading = true;
            ErrorMessage = string.Empty;

            try
            {
                User? user = await _authenticationService.AuthenticateAsync(Username, passwordBox.Password);

                if (user != null)
                {
                    // Успешный вход! Вызываем событие и передаем пользователя.
                    LoginSuccess?.Invoke(user);
                }
                else
                {
                    ErrorMessage = "Неверный логин или пароль.";
                }
            }
            catch (Exception ex)
            {
                // Можно добавить логирование ошибки ex.Message
                ErrorMessage = "Произошла ошибка при подключении к сервису.";
            }
            finally
            {
                IsLoading = false;
            }
        }
    }
}
