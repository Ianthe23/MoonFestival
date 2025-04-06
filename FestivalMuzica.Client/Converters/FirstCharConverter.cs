using Avalonia.Data.Converters;
using System;
using System.Globalization;

namespace FestivalMuzica.Client.Converters
{
    public class FirstCharConverter : IValueConverter
    {
        // Convert method: Takes the full string and returns the first character
        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            if (value is string str && !string.IsNullOrEmpty(str))
            {
                return str[0].ToString(); // Return the first character
            }
            return string.Empty; // Return empty string if the value is null or empty
        }

        // ConvertBack method: Not needed for this one-way binding
        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}