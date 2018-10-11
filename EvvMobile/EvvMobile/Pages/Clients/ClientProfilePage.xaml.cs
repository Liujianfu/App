using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using EvvMobile.Pages.Base;
using EvvMobile.ViewModels.Clients;
using ImageCircle.Forms.Plugin.Abstractions;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace EvvMobile.Pages.Clients
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class ClientProfilePage : ClientProfilePageXaml
    {
        public ClientProfilePage()
        {
            InitializeComponent();
            var collapseMapTapGestureRecognizer = new TapGestureRecognizer();
            collapseMapTapGestureRecognizer.Tapped += (s, e) =>
            {
                CollapseImageClicked(s, e);
            };
            CollapseImage.GestureRecognizers.Add(collapseMapTapGestureRecognizer);
            ContactCollapseImage.GestureRecognizers.Add(collapseMapTapGestureRecognizer);
            EmergencyCollapseImage.GestureRecognizers.Add(collapseMapTapGestureRecognizer);
        }
        protected override void OnAppearing()
        {
            base.OnAppearing();

            if (!string.IsNullOrWhiteSpace(ViewModel.ClientPictureBase64Img))
            {
                try
                {
                    byte[] bytes = Convert.FromBase64String(ViewModel.ClientPictureBase64Img);

                    var memImage = new MemoryStream(bytes);
                    ClientProfileImage.Source = ImageSource.FromStream(() => memImage);

                }
                catch (Exception e)
                {
                    ViewModel.ClientPictureBase64Img = "";//clear invalid signature
                }
            }
            else
            {
                ClientProfileImageLayout.Children.Clear();
                var initals = ExtractInitialsFromName(ViewModel.ClientFullName);
                var intitalLabel = new Label
                {
                    Text = initals,
                    HorizontalOptions =LayoutOptions.Center,
                    VerticalOptions = LayoutOptions.Center,
                    FontSize = 20
                };
                ClientProfileImageLayout.Children.Add(intitalLabel);
            }
        }

        public static string ExtractInitialsFromName(string name)
        {
            if (string.IsNullOrWhiteSpace(name))
                return "";
            // first remove all: punctuation, separator chars, control chars, and numbers (unicode style regexes)
            string initials = Regex.Replace(name, @"[\p{P}\p{S}\p{C}\p{N}]+", "");

            // Replacing all possible whitespace/separator characters (unicode style), with a single, regular ascii space.
            initials = Regex.Replace(initials, @"\p{Z}+", " ");

            // Remove all Sr, Jr, I, II, III, IV, V, VI, VII, VIII, IX at the end of names
            initials = Regex.Replace(initials.Trim(), @"\s+(?:[JS]R|I{1,3}|I[VX]|VI{0,3})$", "", RegexOptions.IgnoreCase);

            // Extract up to 2 initials from the remaining cleaned name.
            initials = Regex.Replace(initials, @"^(\p{L})[^\s]*(?:\s+(?:\p{L}+\s+(?=\p{L}))?(?:(\p{L})\p{L}*)?)?$", "$1$2").Trim();

            if (initials.Length > 2)
            {
                // Worst case scenario, everything failed, just grab the first two letters of what we have left.
                initials = initials.Substring(0, 2);
            }

            return initials.ToUpperInvariant();
        }
        private  void CollapseImageClicked(object sender, EventArgs e)
        {
            bool isVisible = false;
            if (sender.Equals(CollapseImage))
                isVisible = PersonalInfoGrid.IsVisible = !PersonalInfoGrid.IsVisible;
            else if (sender.Equals(ContactCollapseImage))
                isVisible = ContactInfoGrid.IsVisible = !ContactInfoGrid.IsVisible;
            else if (sender.Equals(EmergencyCollapseImage))
                isVisible = EmergencyContactInfoGrid.IsVisible = !EmergencyContactInfoGrid.IsVisible;
          /*  else if (sender.Equals(CollapseImage4))
                isVisible = ChartLayout4.IsVisible = !ChartLayout4.IsVisible;
            else if (sender.Equals(CollapseImage5))
                isVisible = ChartLayout5.IsVisible = !ChartLayout5.IsVisible;
            else if (sender.Equals(CollapseImage6))
                isVisible = ChartLayout6.IsVisible = !ChartLayout6.IsVisible;*/

            if (isVisible)
            {
                ((Image)sender).Source = ImageSource.FromResource("EvvMobile.Images.collapsearrow40.png");
            }
            else
            {
                ((Image)sender).Source = ImageSource.FromResource("EvvMobile.Images.expandarrow40.png");
            }

        }
    }
    public class ClientProfilePageXaml : ModelBoundContentPage<ClientProfileViewModel> { }
}