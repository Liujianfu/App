using System.Globalization;
using EvvMobile.iOS.Localization;
using EvvMobile.Localization;
using Foundation;
using Xamarin.Forms;

[assembly:Dependency(typeof(Localize))]

namespace EvvMobile.iOS.Localization
{
    public class Localize : ILocalize
    {
        public CultureInfo GetCurrentCultureInfo ()
        {
            try
            {
                return CultureInfo.GetCultureInfo(
                    NSLocale.CurrentLocale.LocaleIdentifier.Replace('_', '-'));
            }
            catch (CultureNotFoundException)
            {
                return CultureInfo.GetCultureInfo(NSLocale.CurrentLocale.LanguageCode);
            }
        }
    }
}

