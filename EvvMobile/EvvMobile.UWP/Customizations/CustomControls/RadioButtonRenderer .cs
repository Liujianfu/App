using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.UI.Xaml.Media;
using EvvMobile.Customizations.CustomControls;
using EvvMobile.Helper;
using EvvMobile.UWP.Customizations.CustomControls;
using EvvMobile.UWP.Extensions;
using Xamarin.Forms;
using Xamarin.Forms.Platform.UWP;

using NativeRadioButton = Windows.UI.Xaml.Controls.RadioButton;

[assembly: ExportRenderer(typeof(CustomRadioButton), typeof(RadioButtonRenderer))]
namespace EvvMobile.UWP.Customizations.CustomControls
{
    public class RadioButtonRenderer : ViewRenderer<CustomRadioButton, NativeRadioButton>
    {
        protected override void OnElementChanged(ElementChangedEventArgs<CustomRadioButton> e)
        {
            base.OnElementChanged(e);

            if (e.OldElement != null)
            {
                e.OldElement.CheckedChanged -= CheckedChanged;
            }

            if (e.NewElement == null)
            {
                return;
            }

            if (Control == null)
            {
                var radioButton = new NativeRadioButton();
                radioButton.Checked += (s, args) => Element.Checked = true;
                radioButton.Unchecked += (s, args) => Element.Checked = false;

                SetNativeControl(radioButton);
            }

            Control.Content = e.NewElement.Text;
            Control.IsChecked = e.NewElement.Checked;

            UpdateFont();

            Element.CheckedChanged += CheckedChanged;
            Element.PropertyChanged += ElementOnPropertyChanged;
        }

        private void ElementOnPropertyChanged(object sender, PropertyChangedEventArgs propertyChangedEventArgs)
        {
            NativeRadioButton control = Control;

            if (control == null)
            {
                return;
            }

            switch (propertyChangedEventArgs.PropertyName)
            {
                case "Checked":
                    control.IsChecked = Element.Checked;
                    break;
                case "TextColor":
                    control.Foreground = Element.TextColor.ToBrush();
                    break;
                case "FontName":
                case "FontSize":
                    UpdateFont();
                    break;
                case "Text":
                    control.Content = Element.Text;
                    break;
                default:
                    System.Diagnostics.Debug.WriteLine("Property change for {0} has not been implemented.", propertyChangedEventArgs.PropertyName);
                    break;
            }
        }

        private void CheckedChanged(object sender, EventArgs<bool> eventArgs)
        {
            Device.BeginInvokeOnMainThread(() =>
            {
                Control.Content = Element.Text;
                Control.IsChecked = eventArgs.Value;
            });
        }

        /// <summary>
        /// Updates the font.
        /// </summary>
        private void UpdateFont()
        {
            if (!string.IsNullOrEmpty(Element.FontName))
            {
                Control.FontFamily = new FontFamily(Element.FontName);
            }

            Control.FontSize = (Element.FontSize > 0) ? (float)Element.FontSize : 12.0f;
        }
    }
}
