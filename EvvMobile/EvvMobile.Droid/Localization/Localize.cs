using System.Globalization;
using EvvMobile.Droid.Localization;
using EvvMobile.Localization;
using Xamarin.Forms;

[assembly:Dependency(typeof(Localize))]

namespace EvvMobile.Droid.Localization
{
    public class Localize : ILocalize
    {
        public CultureInfo GetCurrentCultureInfo ()
        {
            var androidLocale = Java.Util.Locale.Default;
            var netLanguage = androidLocale.ToString().Replace ("_", "-"); // turns pt_BR into pt-BR
            return new CultureInfo(netLanguage);
        }
    }
}

