using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EvvMobile.Statics
{
    public class NotificationConstants
    {  
        // Azure app-specific connection string and hub path

        public const string NotificationHubName = "EvvMobile";
        public const string ApplicationURL = "http://jianfuliult6.fei.local/Evv.IISHost.PushNotification/api/";//"http://JIANFULIULT6.fei.local/Evv.IISHost.PushNotification/api/";//if register device from back end, this is 

        public const string ConnectionString = "<Azure connection string>";

    }
}
