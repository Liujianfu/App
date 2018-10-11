using System;

namespace EvvMobile.Customizations.CustomControls.Calendar
{
	public class DateTimeEventArgs : EventArgs
	{
		public DateTime DateTime { get; set; }
	    public bool    IsWeekView { get; set; }
        public int SelectedIndex { get; set; }
    }
}

