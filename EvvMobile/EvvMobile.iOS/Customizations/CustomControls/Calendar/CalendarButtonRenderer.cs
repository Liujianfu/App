using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using CoreAnimation;
using CoreGraphics;
using EvvMobile.Customizations.CustomControls.Calendar;
using EvvMobile.iOS.Customizations.CustomControls.Calendar;
using EvvMobile.Statics;
using Foundation;
using UIKit;
using Xamarin.Forms;
using Xamarin.Forms.Platform.iOS;

[assembly: Xamarin.Forms.ExportRenderer(typeof(CalendarButton), typeof(CalendarButtonRenderer))]

namespace EvvMobile.iOS.Customizations.CustomControls.Calendar
{
    [Preserve(AllMembers = true)]
    public class CalendarButtonRenderer : ButtonRenderer
    {
        protected override void OnElementChanged(ElementChangedEventArgs<Xamarin.Forms.Button> e)
        {
            base.OnElementChanged(e);
            if (Control == null) return;

            var btnelement = Element as CalendarButton;
            if (btnelement != null && !btnelement.IsNotDateButton)
                Control.ContentEdgeInsets = new UIEdgeInsets(
                (float)1,
                (float)1,
                (float)10,
                (float)1);
            else
                Control.ContentEdgeInsets = new UIEdgeInsets(
                    (float)1,
                    (float)1,
                    (float)1,
                    (float)1);
        }
        protected override void OnElementPropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            base.OnElementPropertyChanged(sender, e);
            var element = Element as CalendarButton;
            if (e.PropertyName == nameof(element.TextWithoutMeasure) || e.PropertyName == "Renderer")
            {
                Control.SetTitle(element.TextWithoutMeasure, UIControlState.Normal);
               Control.SetTitle(element.TextWithoutMeasure, UIControlState.Disabled);
            }
            if (e.PropertyName == nameof(element.TextColor) || e.PropertyName == "Renderer")
            {
                Control.SetTitleColor(element.TextColor.ToUIColor(), UIControlState.Disabled);
                Control.SetTitleColor(element.TextColor.ToUIColor(), UIControlState.Normal);
            }
            if (e.PropertyName == nameof(element.BackgroundPattern))
            {
                DrawBackgroundPattern();
            }
            if (e.PropertyName == nameof(element.BackgroundImage))
            {
                DrawBackgroundImage();
            }
            if (e.PropertyName == nameof(element.IsSelected) ||
                e.PropertyName == nameof(Element.BackgroundColor) || 
                e.PropertyName == nameof(element.AppointmentCount) ||
                e.PropertyName == nameof(Element.Height) || e.PropertyName == "Renderer")
            {
                DrawAppointments();
            }
        }

        public override void Draw(CGRect rect)
        {
            base.Draw(rect);
            Control.SetBackgroundImage(null, UIControlState.Normal);
            Control.SetBackgroundImage(null, UIControlState.Disabled);
            DrawBackgroundImage();
            DrawBackgroundPattern();
            DrawAppointments();
        }

        protected  void DrawAppointments()
        {
            var element = Element as CalendarButton;
            if (element == null || Control.Frame.Width == 0||element.IsNotDateButton) return;
            UIImage image;
            UIGraphics.BeginImageContext(Control.Frame.Size);
            using (CGContext g = UIGraphics.GetCurrentContext())
            {                

                    g.SetFillColor(element.BackgroundColor.ToCGColor());
                    
                    var w = (int)Math.Ceiling(Control.Frame.Width );
                  
                    var h = (int)Math.Ceiling(Control.Frame.Height);
                    var r = new CGRect { X = 0, Y = 0, Width = w, Height = h };
                    g.FillRect(r);
                  DrawEvents(g, new Pattern{
                    Text = element.TextWithoutMeasure,
                        TextAlign = TextAlign.Middle,
                        TextColor = element.TextColor,
                        TextSize = (float)element.FontSize,
                        AppointmentCount = element.AppointmentCount,

                        IsSelected = element.IsSelected
                     }, r);



                image = UIGraphics.GetImageFromCurrentImageContext();
            }
            UIGraphics.EndImageContext();
            Control.SetBackgroundImage(image, UIControlState.Normal);
            Control.SetBackgroundImage(image, UIControlState.Disabled);
        }
        protected  void DrawBackgroundImage()
        {
            var element = Element as CalendarButton;
            if (element == null || element.BackgroundImage == null) return;
            var image = GetImage(element.BackgroundImage).Result;
            Control.SetBackgroundImage(image, UIControlState.Normal);
            Control.SetBackgroundImage(image, UIControlState.Disabled);
        }

        protected void DrawBackgroundPattern()
        {
            var element = Element as CalendarButton;
            if (element == null || element.BackgroundPattern == null || Control.Frame.Width == 0) return;

            UIImage image;
            UIGraphics.BeginImageContext(Control.Frame.Size);
            using (CGContext g = UIGraphics.GetCurrentContext())
            {
                for (var i = 0; i < element.BackgroundPattern.Pattern.Count; i++)
                {
                    var p = element.BackgroundPattern.Pattern[i];
                    g.SetFillColor(p.Color.ToCGColor());
                    var l = (int)Math.Ceiling(Control.Frame.Width * element.BackgroundPattern.GetLeft(i));
                    var t = (int)Math.Ceiling(Control.Frame.Height * element.BackgroundPattern.GetTop(i));
                    var w = (int)Math.Ceiling(Control.Frame.Width * element.BackgroundPattern.Pattern[i].WidthPercent);
                    var h = (int)Math.Ceiling(Control.Frame.Height * element.BackgroundPattern.Pattern[i].HightPercent);
                    var r = new CGRect { X = l, Y = t, Width = w, Height = h };
                    g.FillRect(r);
                    DrawText(g, p, r);
                }

               
                image = UIGraphics.GetImageFromCurrentImageContext();
            }
            UIGraphics.EndImageContext();
            Control.SetBackgroundImage(image, UIControlState.Normal);
            Control.SetBackgroundImage(image, UIControlState.Disabled);
        }

        Task<UIImage> GetImage(FileImageSource image)
        {
            var handler = new FileImageSourceHandler();
            return handler.LoadImageAsync(image);
        }
        protected void DrawEvents(CGContext g, Pattern p, CGRect r)
        {
            if (string.IsNullOrEmpty(p.Text)) return;
            if (p.IsSelected)
            {
                var radius = Bounds.Height > Bounds.Width ? (Bounds.Width - 4) / 2 : (Bounds.Height - 4) / 2;
                g.SetFillColor(Color.LightSkyBlue.ToCGColor());

                var circleArea = new CGRect(r.X + Bounds.Width / 2 - radius, r.Y + Bounds.Height / 2 - radius, 2 * radius, 2 * radius);
                g.FillEllipseInRect(circleArea.Inset(1, 1));
            }
            if (p.AppointmentCount > 0)
            {
                for (int i = 0; i < 3 && i < p.AppointmentCount; i++)
                {
                    var startX = 10;
                    if (p.AppointmentCount == 1)
                        startX = (int)((int)Math.Round(Bounds.Width / 2.0) - 3);
                    else
                        if (p.AppointmentCount == 2)
                        startX = (int)((int)Math.Round(Bounds.Width / 2.0) - 8);
                    else
                        startX = (int)((int)Math.Round(Bounds.Width / 2.0) - 13);



                    var circleArea = new CGRect(startX + i * 10, r.Y + (int)Math.Round(Bounds.Height / 2.0) + 5, 6, 6);
                    switch (i)
                    {
                        case 0:
                            g.SetFillColor(Palette.AppointmentIndicatorColor.ToCGColor());
                            break;
                        case 1:
                            g.SetFillColor(Palette.AppointmentIndicatorColor1.ToCGColor());
                            break;
                        case 2:
                            g.SetFillColor(Palette.AppointmentIndicatorColor2.ToCGColor());
                            break;
                    }
                    g.FillEllipseInRect(circleArea.Inset(1, 1));
                }

            }
        }
        protected void DrawText(CGContext g, Pattern p, CGRect r)
        {
            if (string.IsNullOrEmpty(p.Text)) return;
            var bounds = p.Text.StringSize(UIFont.FromName("Helvetica", p.TextSize));
            var al = (int)p.TextAlign;
            var x = r.X; ;

            if ((al & 2) == 2) // center
            {
                x = r.X + (int)Math.Round(r.Width / 2.0) - (int)Math.Round(bounds.Width / 2.0);
            }
            else if ((al & 4) == 4) // right
            {
                x = (r.X + r.Width) - bounds.Width - 2;
            }
            var y = r.Y + (int)Math.Round(bounds.Height / 2.0)+2;
            if ((al & 16) == 16) // middle
            {
                y = r.Y + (int)Math.Ceiling(r.Height / 2.0) + (int)Math.Round(bounds.Height / 6.0);
            }
            else if ((al & 32) == 32) // bottom
            {
                y = (r.Y + r.Height) - 2;
            }
            g.SaveState();
            g.TranslateCTM(0, Bounds.Height);
            g.ScaleCTM(1, -1);
            g.SetFillColor(p.TextColor.ToCGColor());
            g.SetTextDrawingMode(CGTextDrawingMode.Fill);
            g.SelectFont("Helvetica", p.TextSize, CGTextEncoding.MacRoman);
            g.ShowTextAtPoint(x, Bounds.Height - y, p.Text);
            g.RestoreState();
        }
    }

    public static class Calendar
    {
        public static void Init()
        {
            var d = "";
        }
    }
}
