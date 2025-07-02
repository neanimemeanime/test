// Файл: Converters/PhoneNumberFormatConverter.cs
// Описание: Конвертер для форматирования телефонного номера в реальном времени.
using System;
using System.Globalization;
using System.Linq;
using System.Text.RegularExpressions;
using System.Windows.Data;

namespace RepairServiceAppMVVM.Converters
{
    public class PhoneNumberFormatConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is not string phoneNumber)
            {
                return string.Empty;
            }

            // Очищаем от всего, кроме цифр
            string digitsOnly = new string(phoneNumber.Where(char.IsDigit).ToArray());

            if (string.IsNullOrEmpty(digitsOnly))
            {
                return string.Empty;
            }

            // Если номер начинается с 7 или 8, убираем их для форматирования
            if (digitsOnly.Length > 10 && (digitsOnly.StartsWith("7") || digitsOnly.StartsWith("8")))
            {
                digitsOnly = digitsOnly.Substring(1);
            }

            // Ограничиваем до 10 цифр
            if (digitsOnly.Length > 10)
            {
                digitsOnly = digitsOnly.Substring(0, 10);
            }

            // Применяем маску
            switch (digitsOnly.Length)
            {
                case var n when n > 6:
                    return $"+7 ({digitsOnly.Substring(0, 3)}) {digitsOnly.Substring(3, 3)}-{digitsOnly.Substring(6)}";
                case var n when n > 3:
                    return $"+7 ({digitsOnly.Substring(0, 3)}) {digitsOnly.Substring(3)}";
                case var n when n > 0:
                    return $"+7 ({digitsOnly}";
                default:
                    return "+7 (";
            }
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is not string formattedNumber)
            {
                return string.Empty;
            }

            // При сохранении в модель возвращаем только цифры
            return new string(formattedNumber.Where(char.IsDigit).ToArray());
        }
    }
}
