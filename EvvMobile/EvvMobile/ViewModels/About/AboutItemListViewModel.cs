using System.Collections.Generic;
using EvvMobile.ViewModels.Base;

namespace EvvMobile.ViewModels.About
{
    public class AboutItemListViewModel : BaseViewModel
    {
        public List<AboutItemViewModel> Items { get; private set; }

        public string Overview { get; private set; }

        public string ListHeading { get; private set; }

        public AboutItemListViewModel()
        {
            Items = new List<AboutItemViewModel>()
            {
                new AboutItemViewModel()
                {
                    Title = "Help desk",
                    Uri = "http://www.feisystems.com/"
                },

                new AboutItemViewModel()
                {
                    Title = "Feed Back",
                    Uri = "http://www.feisystems.com/"
                }
            };

            Overview =
                "Our Electronic Visit Verification App allows you to record proof of service visit "+
                "and eliminate potential fraud charges." +
                "It can capture the date, time, location while submitting service visit record.\n"+
                "1. Capture patient signature at the point of care.\n"+
                "2. Record date, time and location of visit.\n"+
                "3. Verify employee compliance with scheduled shift."+
                "4. Easy to schedul an urgent shift with the app.";

            ListHeading =
                "The app supposts phones or tablets on both the Apple iOS or Google Android platform."+
                "Using advanced encryption, GPS, cellular and Wi-Fi technology to check employee's location, visit time.";
        }
    }
}
