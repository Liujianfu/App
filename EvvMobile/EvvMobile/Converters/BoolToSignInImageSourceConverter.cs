using System;
using System.Globalization;
using Xamarin.Forms;

namespace EvvMobile.Converters
{
    public class BoolToSignInImageSourceConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            var boolValue = (bool)value;

            var imagePath = parameter as string;
            if (boolValue)
            {
                return ImageSource.FromResource("EvvMobile.Images.SignInOnLine.png");
            }
            else
            {
                return ImageSource.FromResource("EvvMobile.Images.SignInOffLine.png");
            }
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
