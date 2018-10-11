using System;
using System.Threading.Tasks;
using Evv.Message.Portable.Schedulers.Dtos;
using EvvMobile.RestfulWebService.Infrastructure.Common;
using EvvMobile.RestfulWebService.Infrastructure.Extensions;
using Xamarin.Forms;

namespace EvvMobile.Notifications
{
    public class NotificationRegistrationService
    {
        private static NotificationRegistrationService instance;
        public static NotificationRegistrationService Instance => instance ?? (instance = new NotificationRegistrationService(Statics.NotificationConstants.ApplicationURL));

        private NotificationRegistrationService(string baseUrl)
        {
            _httpClientManager = new HttpClientManager();
            _baseUrl = baseUrl;
        }

        public async Task<bool> RegisterDeviceAsync(params string[] tags)
        {
            // Resolve dep with whatever IOC container
            var pushNotificationService = DependencyService.Get<IPushNotificationService>();

            // Get our registration information
            var deviceInstallation = pushNotificationService?.GetDeviceRegistration(tags);

            if (deviceInstallation == null)
                return false;

            // Put the device information to the server
            try
            {
                var client = _httpClientManager.GetOrAdd(_baseUrl);
                //back end receive this request, it should deregister the device before registering it 
                var result = await client.ExtensionPostAsJsonAsync("Register/Register",
                    deviceInstallation);

                return result.ResponseStatus == ResponseStatuses.Success;
            }
            catch (Exception e)
            {
                return false;
            }
        }
        /// <summary>
        /// if we want multiple users use same device, the app should deregister the device whenever the user log out 
        /// </summary>
        /// <returns></returns>
        public async Task<bool> DeregisterDeviceAsync()
        {
            var pushNotificationService = Xamarin.Forms.DependencyService.Get<IPushNotificationService>();

            var deviceInstallation = pushNotificationService?.GetDeviceRegistration(new string[] {});

            if (deviceInstallation == null)
                return false;

            // Delete that installation id from our NH
            try
            {
                var client = _httpClientManager.GetOrAdd(_baseUrl);
                var result = await client.ExtensionPostAsJsonAsync("Register/Deregister",
                    deviceInstallation);

                return result.ResponseStatus == ResponseStatuses.Success;
            }
            catch (Exception e)
            {
                return false;
            }
        }
        #region fields
        private IHttpClientManager _httpClientManager;
        private string _baseUrl;
        #endregion
    }
}
