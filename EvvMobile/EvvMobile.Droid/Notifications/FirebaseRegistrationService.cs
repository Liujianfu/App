using System.Threading.Tasks;
using Android.App;
using Android.Util;
using EvvMobile.Notifications;
using Firebase.Iid;

namespace EvvMobile.Droid.Notifications
{
    [Service]
    [IntentFilter(new[] { "com.google.firebase.INSTANCE_ID_EVENT" })]
    public class FirebaseRegistrationService : FirebaseInstanceIdService
    {
        const string TAG = "FirebaseRegistrationService";

        public override void OnTokenRefresh()
        {
            var refreshedToken = FirebaseInstanceId.Instance.Token;
            Log.Debug(TAG, "Refreshed token: " + refreshedToken);
            SendRegistrationTokenToAzureNotificationHub(refreshedToken);
        }

        void SendRegistrationTokenToAzureNotificationHub(string token)
        {
            // Update registration to notification hub with updated token
            Task.Run( () =>
            {
                 NotificationRegistrationService.Instance.RegisterDeviceAsync().Wait();
            });
        }
    }
}
