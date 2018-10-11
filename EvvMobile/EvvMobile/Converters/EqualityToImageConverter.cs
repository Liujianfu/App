using System;
using System.Globalization;
using Xamarin.Forms;

namespace EvvMobile.Converters
{
    public class EqualityToImageConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is string && value !=null)
            {
                var appendStr = value as string;
                return ImageSource.FromResource("EvvMobile.Images." + parameter + appendStr + ".png");
            }
            else
             return ImageSource.FromResource("EvvMobile.Images."+ parameter + ".png");
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
