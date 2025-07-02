// Файл: Views/ClientsView.xaml.cs
using System.Linq;
using System.Windows.Controls;
using System.Windows.Input;

namespace RepairServiceAppMVVM.Views
{
    public partial class ClientsView : UserControl
    {
        public ClientsView()
        {
            InitializeComponent();
        }

        // Этот метод срабатывает ПЕРЕД тем, как символ появится в поле
        private void PhoneNumberTextBox_PreviewTextInput(object sender, TextCompositionEventArgs e)
        {
            // Разрешаем вводить только цифры
            if (!char.IsDigit(e.Text, e.Text.Length - 1))
            {
                e.Handled = true; // Отменяем ввод, если это не цифра
            }
        }

        // Этот метод срабатывает ПОСЛЕ того, как текст в поле изменился
        private void PhoneNumberTextBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (sender is not TextBox textBox) return;

            // Сохраняем позицию курсора, чтобы он не прыгал в начало
            int selectionStart = textBox.SelectionStart;
            string originalText = textBox.Text;

            // Удаляем все, кроме цифр
            string digitsOnly = new string(textBox.Text.Where(char.IsDigit).ToArray());

            // Если номер начинается с 7 или 8, убираем их для единообразия
            if (digitsOnly.StartsWith("7") || digitsOnly.StartsWith("8"))
            {
                digitsOnly = digitsOnly.Length > 1 ? digitsOnly.Substring(1) : "";
            }

            // Ограничиваем до 10 цифр
            if (digitsOnly.Length > 10)
            {
                digitsOnly = digitsOnly.Substring(0, 10);
            }

            string formatted;
            switch (digitsOnly.Length)
            {
                case var n when n <= 3:
                    formatted = $"+7 ({digitsOnly}";
                    break;
                case var n when n <= 6:
                    formatted = $"+7 ({digitsOnly.Substring(0, 3)}) {digitsOnly.Substring(3)}";
                    break;
                case var n when n <= 8:
                    formatted = $"+7 ({digitsOnly.Substring(0, 3)}) {digitsOnly.Substring(3, 3)}-{digitsOnly.Substring(6)}";
                    break;
                case var n when n > 8:
                    formatted = $"+7 ({digitsOnly.Substring(0, 3)}) {digitsOnly.Substring(3, 3)}-{digitsOnly.Substring(6, 2)}-{digitsOnly.Substring(8)}";
                    break;
                default:
                    formatted = ""; // Если все стерли, поле будет пустым
                    break;
            }

            // Устанавливаем отформатированный текст, только если он действительно изменился
            if (textBox.Text != formatted)
            {
                textBox.Text = formatted;
                // Восстанавливаем позицию курсора
                textBox.SelectionStart = textBox.Text.Length;
            }
        }
    }
}
