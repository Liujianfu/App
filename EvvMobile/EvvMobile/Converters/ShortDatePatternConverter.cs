using System;
using Xamarin.Forms;

namespace EvvMobile.Converters
{
    public class ShortDatePatternConverter : IValueConverter
    {
        #region IValueConverter implementation
        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            if (value == null) // handle nullable DateTime
                return string.Empty;
            if (value is DateTime)
            {
                return ((DateTime)value).ToString("d");
            }
            return ((DateTimeOffset)value).ToString("d");
        }
        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            throw new NotImplementedException();
        }
        #endregion
        
    }
}

