using System;
using System.Threading.Tasks;
using EvvMobile.Camera;
using Plugin.Media.Abstractions;
using Plugin.Media;

using EvvMobile.iOS.Camera;
using UIKit;
using Xamarin.Forms;
[assembly: Dependency(typeof(TakePhotoService))]
namespace EvvMobile.iOS.Camera
{
    public class TakePhotoService : ITakePhotoService
    {
        public async Task<MediaFile> TakePhotoAsync(StoreCameraMediaOptions options)
        {
            Func<object> func = () =>
            {
                var imageView = new UIImageView(UIImage.FromBundle("face-template.png"));
                imageView.ContentMode = UIViewContentMode.ScaleAspectFit;

                var screen = UIScreen.MainScreen.Bounds;
                imageView.Frame = screen;

                return imageView;
            };
            options.OverlayViewProvider = func;
            var file = await CrossMedia.Current.TakePhotoAsync(options);
            return file;
        }
    }
}