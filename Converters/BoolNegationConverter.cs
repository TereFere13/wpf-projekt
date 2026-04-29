using System;
using System.Globalization;
using System.Windows.Data;

namespace wpf_projekt.Converters
{
    public class BoolNegationConverter : IValueConverter
    {
        public static readonly BoolNegationConverter Instance = new();

        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
            => value is bool b && !b;

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
            => value is bool b && !b;
    }
}