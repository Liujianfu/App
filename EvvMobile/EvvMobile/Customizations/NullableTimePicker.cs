using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Forms;

namespace EvvMobile.Customizations
{
    
    class NullableTimePicker : TimePicker
    {
        private string _format = null;
        public static readonly BindableProperty NullableTimeProperty = BindableProperty.Create(nameof(NullableTime), typeof(TimeSpan?), typeof(NullableDatePicker), null, BindingMode.TwoWay);

        public TimeSpan? NullableTime
        {
            get { return (TimeSpan?)GetValue(NullableTimeProperty); }
            set { SetValue(NullableTimeProperty, value); UpdateTime(); }
        }

        private void UpdateTime()
        {
            if (NullableTime.HasValue) { if (null != _format) Format = _format;Time = NullableTime.Value;  }
            else { _format = Format; Format = " -     - :-     -"; }
        }
        protected override void OnBindingContextChanged()
        {
            base.OnBindingContextChanged();
            UpdateTime();
        }

        protected override void OnPropertyChanged(string propertyName = null)
        {
            base.OnPropertyChanged(propertyName);
            if (propertyName == "Time" || propertyName == "IsFocused") NullableTime = Time;
        }
    }
}
