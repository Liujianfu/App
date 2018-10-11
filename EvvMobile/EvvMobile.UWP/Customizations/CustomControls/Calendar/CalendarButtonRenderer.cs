using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Shapes;
using EvvMobile.Customizations.CustomControls.Calendar;
using EvvMobile.Statics;
using EvvMobile.UWP.Customizations.CustomControls.Calendar;
using EvvMobile.UWP.Extensions;
using ImageOrientation = EvvMobile.Customizations.CustomControls.ImageOrientation;
#if WINDOWS_UWP
using Xamarin.Forms.Platform.UWP;
#else
using Xamarin.Forms.Platform.WinRT;
#endif

[assembly: ExportRenderer(typeof(CalendarButton), typeof(CalendarButtonRenderer))]
namespace EvvMobile.UWP.Customizations.CustomControls.Calendar
{
    public class CalendarButtonRenderer : ButtonRenderer
    {
        protected override void OnElementChanged(ElementChangedEventArgs<Xamarin.Forms.Button> e)
        {
            base.OnElementChanged(e);
            if (Control == null) return;
        }

        protected override void OnElementPropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            base.OnElementPropertyChanged(sender, e);
            var element = Element as CalendarButton;

            if (e.PropertyName == nameof(element.TextWithoutMeasure) || 
                e.PropertyName == nameof(element.AppointmentCount)|| e.PropertyName == "Renderer")
            {
                var stackPanel = new StackPanel
                {
                    Margin = new Thickness(0,-3,0,-3),
                    Orientation = Orientation.Vertical,
                  //  Background = new SolidColorBrush(Palette.AppointmentIndicatorColor.ToMediaColor())
                };
                var label = new TextBlock
                {
                    TextAlignment = TextAlignment.Center,
                    Text = element.TextWithoutMeasure,
                    FontSize = element.FontSize-6
                };
                stackPanel.Children.Add(label);
                if (!element.IsNotDateButton)
                {
                   if (element.AppointmentCount > 0)
                    {
                        var appointmentPanel = new StackPanel
                        {
                            Orientation = Orientation.Horizontal,
                            MinHeight = 8

                        };
                        for (int i = 0; i < 4 && i < element.AppointmentCount; i++)
                        {
                            var ellipse = new Ellipse();
                            ellipse.Fill = new SolidColorBrush(Palette.AppointmentIndicatorColor.ToMediaColor());
                            ellipse.Width = 4;
                            ellipse.Height = 4;
                            ellipse.Margin = new Thickness(1,0,1,1);
                            appointmentPanel.Children.Add(ellipse);
                        }

                        stackPanel.Children.Add(appointmentPanel);
                    }
                   else
                   {
                       var appointmentPanel = new StackPanel
                       {
                           Orientation = Orientation.Horizontal,
                           MinHeight = 8

                       };
                       stackPanel.Children.Add(appointmentPanel);
                    }                    
                }
                //draw appointment indicators


                Control.Content = stackPanel;

            }

            if (Element.BorderWidth > 0 && (e.PropertyName == nameof(element.BorderWidth) || e.PropertyName == "Renderer"))
            {
                Control.BorderThickness = new Thickness(2);
            }
            
        }
    }

    public static class Calendar
    {
        public static void Init()
        {

        }
    }
}
