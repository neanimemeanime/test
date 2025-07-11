﻿using System;
using System.Globalization;
using System.Windows;
using System.Windows.Data;

namespace RepairServiceAppMVVM.Converters
{
    // Атрибут ValueConversion можно убрать, так как теперь работаем с разными типами.
    // Если оставите, это просто подсказка для дизайнера, на функционал не влияет.
    // [ValueConversion(typeof(bool), typeof(Visibility))]
    public class BooleanToVisibilityConverter : IValueConverter
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

            return boolRepresentation ? Visibility.Visible : Visibility.Collapsed;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}   