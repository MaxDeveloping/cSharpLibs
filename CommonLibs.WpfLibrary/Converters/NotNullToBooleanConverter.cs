using System;
using System.Collections.Generic;
using System.Globalization;
using System.Text;
using System.Windows.Data;

namespace CommonLibs.WpfLibrary.Converters
{
    public class NotNullToBooleanConverter : IValueConverter
    {
        public bool InvertResult { get; set; }

        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            var isNotNull = value != null;

            if (InvertResult)
                return !isNotNull;

            return isNotNull;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new InvalidOperationException("This converter can't convert back.");
        }
    }
}
