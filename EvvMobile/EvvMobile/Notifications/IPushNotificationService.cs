using Evv.Message.Portable.Schedulers.Dtos;

namespace EvvMobile.Notifications
{
    public interface IPushNotificationService
    {
        DeviceInstallation GetDeviceRegistration(params string[] tags);
        string GetDeviceId();
    }
}
