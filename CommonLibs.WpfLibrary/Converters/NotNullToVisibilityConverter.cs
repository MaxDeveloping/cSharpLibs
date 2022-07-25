using System;
using System.Collections.Generic;
using System.Globalization;
using System.Text;
using System.Windows;
using System.Windows.Data;

namespace CommonLibs.WpfLibrary.Converters
{
    public class NotNullToVisibilityConverter : IValueConverter
    {
        public bool InvertVisibility { get; set; }

        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            var isVisible = value != null;

            if (InvertVisibility)
                isVisible = !isVisible;

            return isVisible ? Visibility.Visible : Visibility.Collapsed;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new InvalidOperationException("This converter can not convert back.");
        }
    }
}
