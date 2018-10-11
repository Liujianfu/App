using System;
using Xamarin.Forms;

namespace EvvMobile.Customizations.CustomControls.Calendar
{
    public class CalendarButton : Button
    {
		public static readonly BindableProperty DateProperty =
            BindableProperty.Create(nameof(Date), typeof(DateTime?), typeof(CalendarButton), null);

        public DateTime? Date
        {
            get { return (DateTime?)GetValue(DateProperty); }
            set { SetValue(DateProperty, value); }
        }

        public static readonly BindableProperty IsSelectedProperty =
            BindableProperty.Create(nameof(IsSelected), typeof(bool), typeof(CalendarButton), false);

        public bool IsSelected
        {
            get { return (bool)GetValue(IsSelectedProperty); }
            set { SetValue(IsSelectedProperty, value); }
        }

        public static readonly BindableProperty IsOutOfMonthProperty =
            BindableProperty.Create(nameof(IsOutOfMonth), typeof(bool), typeof(CalendarButton), false);

        public bool IsOutOfMonth
        {
            get { return (bool)GetValue(IsOutOfMonthProperty); }
            set { SetValue(IsOutOfMonthProperty, value); }
        }

        public static readonly BindableProperty TextWithoutMeasureProperty =
            BindableProperty.Create(nameof(TextWithoutMeasure), typeof(string), typeof(Button), null);

        public string TextWithoutMeasure
        {
            get
            {
                var text = (string)GetValue(TextWithoutMeasureProperty);
                return string.IsNullOrEmpty(text) ? Text : text;
            }
            set { SetValue(TextWithoutMeasureProperty, value); }
        }

		public static readonly BindableProperty BackgroundPatternProperty =
			BindableProperty.Create(nameof(BackgroundPattern), typeof(BackgroundPattern), typeof(Button), null);

		public BackgroundPattern BackgroundPattern
		{
			get { return (BackgroundPattern)GetValue(BackgroundPatternProperty); }
			set { SetValue(BackgroundPatternProperty, value); }
		}

		public static readonly BindableProperty BackgroundImageProperty =
			BindableProperty.Create(nameof(BackgroundImage), typeof(FileImageSource), typeof(Button), null);

		public FileImageSource BackgroundImage
		{
			get { return (FileImageSource)GetValue(BackgroundImageProperty); }
			set { SetValue(BackgroundImageProperty, value); }	
		}
        public static readonly BindableProperty AppointmentCountProperty =
            BindableProperty.Create(nameof(AppointmentCount), typeof(int), typeof(Button), 0);

        public int AppointmentCount
        {
            get { return (int)GetValue(AppointmentCountProperty); }
            set { SetValue(AppointmentCountProperty, value); }
        }

        public static readonly BindableProperty AppointmentFlagColorProperty =
            BindableProperty.Create(nameof(AppointmentFlagColor), typeof(Color), typeof(Calendar), Color.FromHex("#aaaaaa"));

        /// <summary>
        /// Gets or sets the text color of Appointment Flags.
        /// </summary>
        /// <value>The color of Appointment Flags.</value>
        public Color AppointmentFlagColor
        {
            get { return (Color)GetValue(AppointmentFlagColorProperty); }
            set { SetValue(AppointmentFlagColorProperty, value); }
        }


        public bool IsNotDateButton { get; set; }
    }
}

