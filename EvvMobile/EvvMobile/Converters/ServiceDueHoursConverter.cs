using System;
using Xamarin.Forms;

namespace EvvMobile.Converters
{
    public class ServiceDueHoursConverter : IValueConverter
    {
        #region IValueConverter implementation
        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            if (value == null) // handle nullable DateTime
                return string.Empty;
            DateTime inputDateTime;
            if (value is DateTime)
            {
                inputDateTime=((DateTime)value);
            }
            else
            {
                inputDateTime = ((DateTimeOffset) value).DateTime;
            }

            if (inputDateTime >= DateTime.Now)
            {
                var timeSpan = inputDateTime - DateTime.Now;
                if (timeSpan.Hours < 1)
                {
                    return string.Format("In {0} minutes", timeSpan.Minutes);
                }
                else
                if (timeSpan.Hours == 1)
                {
                    return "In 1 hour";
                }
                else
                {
                    return string.Format("In {0} hours", timeSpan.Hours);
                }
            }
            else
            {
                var timeSpan = DateTime.Now - inputDateTime  ;
                if (timeSpan.Hours < 1)
                {
                    return string.Format("{0} minutes ago", timeSpan.Minutes);
                }
                else
                if (timeSpan.Hours == 1)
                {
                    return "1 hour ago";
                }
                else
                {
                    return string.Format("{0} hours ago", timeSpan.Hours);
                }
            }
        }
        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            throw new NotImplementedException();
        }
        #endregion
        
    }
}

