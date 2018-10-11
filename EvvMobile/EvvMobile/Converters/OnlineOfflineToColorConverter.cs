using System;
using System.Globalization;
using EvvMobile.Statics;
using Xamarin.Forms;

namespace EvvMobile.Converters
{
    public class OnlineOfflineToColorConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            var boolValue = (bool)value;

            if (boolValue)
            {
                return Palette.LevelTwoBackgroundColor;
            }
            else
            {
                return Palette.LevelOneBackgroundColor;
            }
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
