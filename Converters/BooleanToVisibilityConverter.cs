// Файл: Converters/BooleanToVisibilityConverter.cs
// Описание: Стандартный конвертер, который преобразует логическое значение (true/false)
// в значение Visibility (Visible/Collapsed). Это позволяет нам привязывать
// видимость элементов в XAML к bool-свойствам в ViewModel.
using System.Globalization;
using System.Windows;
using System.Windows.Data;

namespace RepairServiceAppMVVM.Converters
{
    [ValueConversion(typeof(bool), typeof(Visibility))]
    public class BooleanToVisibilityConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            bool flag = false;
            if (value is bool b)
            {
                flag = b;
            }

            return flag ? Visibility.Visible : Visibility.Collapsed;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
