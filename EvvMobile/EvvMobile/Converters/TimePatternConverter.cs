using System;
using Xamarin.Forms;

namespace EvvMobile.Converters
{
    public class TimePatternConverter : IValueConverter
    {
        #region IValueConverter implementation
        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            if(value == null)
                return "";
            if (value is DateTime)
            {
                return ((DateTime)value).ToString("hh:mm tt");
            }
            if (value is DateTimeOffset)
            {
                return ((DateTimeOffset)value).ToString("hh:mm tt");
            }
            return "";
        }
        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            throw new NotImplementedException();
        }
        #endregion
        
    }
}

