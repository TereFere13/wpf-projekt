using System;
using System.Globalization;
using System.Windows;
using System.Windows.Data;

namespace wpf_projekt.Converters
{
    public class BoolToVisibilityNegConverter : IValueConverter
    {
        public static readonly BoolToVisibilityNegConverter Instance = new();

        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
            => value is true ? Visibility.Collapsed : Visibility.Visible;

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
            => value is Visibility v && v == Visibility.Collapsed;
    }
}