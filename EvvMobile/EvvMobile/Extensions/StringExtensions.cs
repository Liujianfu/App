
using Xamarin.Forms;

namespace EvvMobile.Extensions
{
    public static class StringExtensions
    {
        public static string CapitalizeForAndroid(this string str)
        {
            return Device.RuntimePlatform == Device.Android ? str.ToUpper() : str;
        }
    }
}

