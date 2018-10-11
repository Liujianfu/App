using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.Networking.PushNotifications;
using Windows.System.Profile;
using Evv.Message.Portable.Schedulers.Dtos;
using EvvMobile.Notifications;

namespace EvvMobile.UWP.Notifications
{
/// <summary>
/// Not sure if this works, need more testing.
/// 
/// </summary>
    public class PushNotificationService : IPushNotificationService
    {
        public DeviceInstallation GetDeviceRegistration(params string[] tags)
        {
            
            var installationId = GetDeviceId();
            var channel = GetChannel().Result;

            if (channel == null)
                return null;

            var installation = new DeviceInstallation
            {
                InstallationId = installationId,
                Platform = "wns",
                PushChannel = channel.Uri,
                StaffId = ""//TODO:add staff id
            };
            // Set up tags to request
            installation.Tags.AddRange(tags);
            // Set up templates to request
            PushTemplate genericTemplate = new PushTemplate
            {
                Body = @"{""data"":{""message"":""$(messageParam)""}}"
            };
            PushTemplate silentTemplate = new PushTemplate
            {
                Body = @"{""data"":{""message"":""$(silentMessageParam)"", ""action"":""$(actionParam)"", ""silent"":""true""}}"
            };
            installation.Templates.Add("genericTemplate", genericTemplate);
            installation.Templates.Add("silentTemplate", silentTemplate);

            return installation;
        }

        public string GetDeviceId()
        {
            var token = HardwareIdentification.GetPackageSpecificToken(null);
            var hardwareId = token.Id;
            var dataReader = Windows.Storage.Streams.DataReader.FromBuffer(hardwareId);

            byte[] bytes = new byte[hardwareId.Length];
            dataReader.ReadBytes(bytes);

            return BitConverter.ToString(bytes).Replace("-", "");
        }

        private async Task<PushNotificationChannel> GetChannel()
        {
            var channel = await PushNotificationChannelManager
                .CreatePushNotificationChannelForApplicationAsync();
            return channel;
        }
    }
}
