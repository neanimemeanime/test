// Файл: ViewModels/NavigationViewModel.cs
// Описание: Базовый класс для всех ViewModel, которые будут использоваться как вкладки.
// Добавляет свойство DisplayName для заголовка вкладки.

namespace RepairServiceAppMVVM.ViewModels
{
    public abstract class NavigationViewModel : ViewModelBase
    {
        public string DisplayName { get; set; } = string.Empty;
    }
}