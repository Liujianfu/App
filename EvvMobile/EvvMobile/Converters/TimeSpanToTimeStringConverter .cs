using System;
using System.Globalization;
using Xamarin.Forms;

namespace EvvMobile.Converters
{
    public class TimeSpanToTimeStringConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value == null)
            {
                return "00:00:00:00";
            }
            if (value is TimeSpan)
            {
                var timeSpan = (TimeSpan)value;
                var time = timeSpan.ToString(@"hh\:mm\:ss");
                return time;                
            }
            return "00:00:00:00";
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
