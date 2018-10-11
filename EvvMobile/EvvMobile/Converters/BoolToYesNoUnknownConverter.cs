using System;
using System.Globalization;
using Xamarin.Forms;

namespace EvvMobile.Converters
{
    public class BoolToYesNoUnknownConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value == null)
            {
                return "Unknown";
            }
            var boolValue = (bool)value;
            if (boolValue)
            {
                return "Yes";
            }
            else
            {
                return "No";
            }
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
