using System;
using System.Globalization;
using Xamarin.Forms;

namespace EvvMobile.Converters
{
    public class StringToImageConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is string)
            {
                return ImageSource.FromResource("EvvMobile.Images." + value + ".png");
            }
             return "EvvMobile.Images.HomeService.png";
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
