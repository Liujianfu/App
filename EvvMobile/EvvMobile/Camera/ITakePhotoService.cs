using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Plugin.Media.Abstractions;
namespace EvvMobile.Camera
{
    public interface ITakePhotoService
    {
         Task<MediaFile> TakePhotoAsync(Plugin.Media.Abstractions.StoreCameraMediaOptions options);
    }
}