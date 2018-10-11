using System;
using System.Globalization;
using Xamarin.Forms;

namespace EvvMobile.Converters
{
    public class BoolToYesNoIconPathConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            var boolValue = (bool)value;
            if (boolValue)
            {
                return ImageSource.FromResource("EvvMobile.Images.CheckYes.png");
            }
            else
            {
                return ImageSource.FromResource("EvvMobile.Images.CheckNo.png");
            }
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
