using System;
using System.Globalization;
using System.Windows;
using System.Windows.Data;

namespace RepairServiceAppMVVM.Converters
{
    // Атрибут ValueConversion можно убрать.
    // [ValueConversion(typeof(bool), typeof(Visibility))]
    public class InverseBooleanToVisibilityConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            bool boolRepresentation = false; // По умолчанию считаем "ложь"

            if (value is bool b)
            {
                boolRepresentation = b;
            }
            else if (value is int i)
            {
                boolRepresentation = i > 0; // Число больше 0 -> true, иначе false
            }
            // Можно добавить обработку других типов, если потребуется

            // Инвертируем результат: true -> Collapsed, false -> Visible
            return boolRepresentation ? Visibility.Collapsed : Visibility.Visible;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}