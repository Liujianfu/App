using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Xamarin.Forms;

namespace EvvMobile.Customizations.CustomControls.Calendar
{
	public partial class Calendar : ContentView
	{

		#region AppointmentFlagColor

		public static readonly BindableProperty AppointmentFlagColorProperty =
			BindableProperty.Create(nameof(AppointmentFlagColor), typeof(Color), typeof(Calendar), Color.FromHex("#aaaaaa"),
                propertyChanged: (bindable, oldValue, newValue) => (bindable as Calendar).ChangeAppointmentFlagColor((Color)newValue, (Color)oldValue));
        protected void ChangeAppointmentFlagColor(Color newValue, Color oldValue)
        {
            if (newValue == oldValue) return;
            buttons.ForEach(b => b.AppointmentFlagColor = newValue);
        }
        /// <summary>
        /// Gets or sets the text color of Appointment Flags.
        /// </summary>
        /// <value>The color of Appointment Flags.</value>
        public Color AppointmentFlagColor
        {
			get { return (Color)GetValue(AppointmentFlagColorProperty); }
			set { SetValue(AppointmentFlagColorProperty, value); }
		}

		#endregion

	    #region AppointmentsSource

        public static readonly BindableProperty AppointmentsSourceProperty =
			BindableProperty.Create(nameof(AppointmentsSource), typeof(IEnumerable), typeof(Calendar), null,
				propertyChanged: (bindable, oldValue, newValue) => (bindable as Calendar).ChangeAppointmentsSource((IEnumerable)newValue, (IEnumerable)oldValue));

	    protected void ChangeAppointmentsSource(IEnumerable newValue, IEnumerable oldValue)
	    {
	        var appointments = newValue.Cast<int>().ToArray();
	        for (int i = 0; i < buttons.Count; i++)
	        {
	            if (CalendarViewType == DateTypeEnum.Normal )
	            {
	                if (i < appointments.Length)
	                {
	                    buttons[i].AppointmentCount = 0;
                        buttons[i].AppointmentCount = appointments[i];
	                }
	                else
	                {
	                    buttons[i].AppointmentCount = 0;
	                }	                
	            }
	            else if( CalendarViewType == DateTypeEnum.Month)
	            {
	                if (buttons[i].Date.HasValue &&
	                    buttons[i].Date.Value.Month  <= appointments.Length)
	                {
	                    buttons[i].AppointmentCount = 0;
                        buttons[i].AppointmentCount = appointments[buttons[i].Date.Value.Month - 1];
	                }
	                else
	                {
	                    buttons[i].AppointmentCount = 0;

	                }
                }
	            else if (CalendarViewType == DateTypeEnum.Week)
	            {
	                if (i < appointments.Length)
	                {
	                    buttons[i].AppointmentCount = 0;
                        buttons[i].AppointmentCount = appointments[i];
	                }
	                else
	                {
	                    buttons[i].AppointmentCount = 0;

	                }

                }

            }
	        if (Device.RuntimePlatform == Device.iOS)
	        {
	            ChangeCalendar(CalandarChanges.All);
	        }
        }

	    /// <summary>
        /// Gets or sets Appointments Source.
        /// </summary>
        /// <value>The Appointments Source .</value>
        public IEnumerable AppointmentsSource
        {
			    get { return (IEnumerable)GetValue(AppointmentsSourceProperty); }
			    set { SetValue(AppointmentsSourceProperty, value); }
		}

		#endregion

	}
}
