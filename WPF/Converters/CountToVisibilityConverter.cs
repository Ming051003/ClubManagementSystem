using System;
using System.Globalization;
using System.Windows;
using System.Windows.Data;

namespace WPF.Converters
{
    public class CountToVisibilityConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value == null)
                return Visibility.Collapsed;

            bool showWhenZero = parameter != null && parameter.ToString() == "ShowWhenZero";

            if (value is int count)
            {
                if (showWhenZero)
                    return Visibility.Visible; // Always show when parameter is set to ShowWhenZero
                else
                    return count > 0 ? Visibility.Visible : Visibility.Collapsed;
            }

            return Visibility.Collapsed;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
