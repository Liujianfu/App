using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using EvvMobile.Pages.Base;
using EvvMobile.ViewModels.Schedules;
using SignaturePad.Forms;
using Xamarin.Forms;

namespace EvvMobile.Pages.Schedules
{
    public partial class SignaturePage : SignaturePageXaml
    {
        public SignaturePage()
        {
            InitializeComponent();
        }

        private async void OnSaveSignature(object sender, EventArgs e)
        {
            var settings = new ImageConstructionSettings
            {
                BackgroundColor = Color.White,
                StrokeColor = Color.Black,
            };
            // write the signature stream to the memory stream
            using (var stream = await Signatureview.GetImageStreamAsync(SignatureImageFormat.Jpeg, settings))
            using (var ms = new MemoryStream())
            {
                if (stream != null)
                {
                    stream.Seek(0, SeekOrigin.Begin);
                    await stream.CopyToAsync(ms);
                    var bytes = ms.ToArray();
                    ViewModel.ClientSignatureBase64Img = Convert.ToBase64String(bytes);                    
                }

            }
            await Navigation.PopAsync();
        }
    }
    public abstract class SignaturePageXaml : ModelBoundContentPage<ScheduleViewModel> { }
}