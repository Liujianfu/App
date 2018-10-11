using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using EvvMobile.Camera;
using EvvMobile.Droid.Camera;
using Plugin.Media.Abstractions;
using Plugin.Media;
using Xamarin.Forms;
[assembly: Dependency(typeof(TakePhotoService))]
namespace EvvMobile.Droid.Camera
{
    public class TakePhotoService : ITakePhotoService
    {
        public async Task<MediaFile> TakePhotoAsync(StoreCameraMediaOptions options)
        {
            var file = await CrossMedia.Current.TakePhotoAsync(options);
            return file;
        }
    }
}