using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using EvvMobile.Statics;
using Xamarin.Forms;

namespace EvvMobile.Converters
{
    public class StripedBackgroundIndexConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            Color rowcolor = Palette.LevelThreeBackgroundColor;
            if (value == null || parameter == null) return Palette.LevelThreeBackgroundColor;
            var index = ((ListView)parameter).ItemsSource.Cast<object>().ToList().IndexOf(value);
            if (index >= 0)
            {
                if (index % 2 == 0)
                {
                    rowcolor = Palette.LevelFourBackgroundColor;
                }                
            }

            return rowcolor;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
