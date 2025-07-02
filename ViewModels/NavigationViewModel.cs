namespace RepairServiceAppMVVM.ViewModels
{
    // Этот класс будет базовым для всех ViewModel'ей, которые являются вкладками.
    // Он наследуется от ViewModelBase, чтобы поддерживать уведомления об изменениях.
    public abstract class NavigationViewModel : ViewModelBase
    {
        // Свойство для хранения имени вкладки, которое будет отображаться в меню.
        public string DisplayName { get; set; } = string.Empty;
    }
}
