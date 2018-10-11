using System;

namespace EvvMobile.DataService.Extensions
{
    public static class DateTimeOffsetExtensions
    {
        public static DateTime? ToDateTime(this DateTimeOffset? src)
        {
            if (src == null)
                return null;
            return src.Value.DateTime;
        }
        public static DateTime ToDateTime(this DateTimeOffset src)
        {
            return src.DateTime;
        }
        public static DateTimeOffset? ToDateTimeOffset(this DateTime? src)
        {
            if (src == null)
                return null;
            return src.Value;
        }
        public static DateTimeOffset ToDateTimeOffset(this DateTime src)
        {
            return src;
        }
    }
}
