using System;
using System.Globalization;
using Xamarin.Forms;

namespace EvvMobile.Converters
{
    public class StringToColorConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value == null)
            {
                return Color.Red;
            }
            if (value is string)
            {
                if (((string) value) == "Initiated")
                {
                    return Color.LawnGreen;
                }
                if (((string)value) == "Scheduled")
                {
                    return Color.DarkGreen;
                }
                if (((string)value) == "Completed")
                {
                    return Color.Blue;
                }
                if (((string)value) == "CompletedWithoutClockIn")
                {
                    return Color.Crimson;
                }
                if (((string)value) == "InProgress")
                {
                    return Color.DarkSeaGreen;
                }
                if (((string)value) == "ScheduleMissed")
                {
                    return Color.OrangeRed;
                }
            }
             return Color.Red;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
