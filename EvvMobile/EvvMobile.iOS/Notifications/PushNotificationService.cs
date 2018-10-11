using System;
using System.Collections.Generic;
using System.Text;
using Evv.Message.Portable.Schedulers.Dtos;
using EvvMobile.Notifications;
using EvvMobile.ViewModels.Systems;
using UIKit;

[assembly: Xamarin.Forms.Dependency(typeof(EvvMobile.iOS.Notifications.PushNotificationService))]
namespace EvvMobile.iOS.Notifications
{
    public class PushNotificationService : IPushNotificationService
    {
        public DeviceInstallation GetDeviceRegistration(params string[] tags)
        {
            if (AppDelegate.PushDeviceToken == null)
            {
                return null;
            }

            // Format our app install information for NH
            var registrationId = AppDelegate.PushDeviceToken.Description
                .Trim('<', '>').Replace(" ", string.Empty).ToUpperInvariant();

            var installation = new DeviceInstallation
            {
                InstallationId = UIDevice.CurrentDevice.IdentifierForVendor.ToString().Replace("<", string.Empty)
                .Replace( ">", string.Empty).Replace(" ", string.Empty),
                Platform = "apns",
                PushChannel = registrationId,
                StaffId = SystemViewModel.Instance.CurrentStaffId
            };
            // Set up tags to request
            installation.Tags.AddRange(tags);
            installation.Tags.Add(installation.InstallationId);
            // Set up templates to request
            PushTemplate genericTemplate = new PushTemplate
            {
                Body = "{\"aps\":{\"alert\":\"$(messageParam)\"}}"
            };

            installation.Templates.Add("genericTemplate", genericTemplate);

            return installation;
        }

        public string GetDeviceId()
        {
            return UIDevice.CurrentDevice.IdentifierForVendor.ToString();
        }
    }
}
