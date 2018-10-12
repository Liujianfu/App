using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Android.Graphics;
using Android.Graphics.Drawables;
using Android.Runtime;
using EvvMobile.Customizations.CustomControls.Calendar;
using EvvMobile.Droid.Customizations.CustomControls.Calendar;
using EvvMobile.Statics;
using Xamarin.Forms;
using Xamarin.Forms.Platform.Android;
using Xamarin.Forms.Xaml;

[assembly: Xamarin.Forms.ExportRenderer(typeof(CalendarButton), typeof(CalendarButtonRenderer))]
namespace EvvMobile.Droid.Customizations.CustomControls.Calendar
{
    [Preserve(AllMembers = true)]
    public class CalendarButtonRenderer : ButtonRenderer
    {
        protected override void OnElementChanged(ElementChangedEventArgs<Xamarin.Forms.Button> e)
        {
            base.OnElementChanged(e);
            if (Control == null) return;
            Control.TextChanged += (sender, a) =>
            {
                var element = Element as CalendarButton;
                if (Control.Text == element.TextWithoutMeasure || (string.IsNullOrEmpty(Control.Text) && string.IsNullOrEmpty(element.TextWithoutMeasure))) return;
              //  Control.Text = element.TextWithoutMeasure;
            };

            Control.SetPadding(1, 1, 1, 10);
            Control.ViewTreeObserver.GlobalLayout += (sender, args) => ChangeBackgroundPattern();
        }
        protected override void OnElementPropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            base.OnElementPropertyChanged(sender, e);
            var element = Element as CalendarButton;

            if (e.PropertyName == nameof(element.TextWithoutMeasure) || e.PropertyName == "Renderer")
            {
               // Control.Text = element.TextWithoutMeasure;
            }

            if (e.PropertyName == nameof(Element.TextColor) || e.PropertyName == "Renderer")
            {
                Control.SetTextColor(Element.TextColor.ToAndroid());
            }

            if (e.PropertyName == nameof(Element.BorderWidth) || e.PropertyName == nameof(Element.BorderColor) || 
                e.PropertyName == nameof(Element.BackgroundColor) ||e.PropertyName == nameof(element.AppointmentCount)||
                e.PropertyName == nameof(Element.Height) || e.PropertyName == nameof(Element.TextColor) ||e.PropertyName == "Renderer")
            {
                if (element.BackgroundPattern == null)
                {
                    if (element.BackgroundImage == null)
                    {
                        var drawable = new GradientDrawable();
                        drawable.SetShape(ShapeType.Rectangle);
                        var borderWidth = (int)Math.Ceiling(Element.BorderWidth);
                        drawable.SetStroke(borderWidth > 0 ? borderWidth + 1 : borderWidth, Element.BorderColor.ToAndroid());
                        drawable.SetColor(Element.BackgroundColor.ToAndroid());

                            var d = new List<Drawable>();
                             d.Add(drawable);
                        if(string.IsNullOrWhiteSpace(Control.Text))
                        {
                            d.Add((Drawable)new TextDrawable(Android.Graphics.Color.Transparent) { Pattern = new Pattern {
                                Text = element.TextWithoutMeasure,
                                TextAlign = TextAlign.CenterTop,
                                TextColor = element.TextColor,
                                TextSize =(float)element.FontSize,
                                AppointmentCount = element.AppointmentCount,
                                AppointmentFlagColor =element.AppointmentFlagColor,
                                IsSelected  =element.IsSelected,

                            } });
                        }

                        var layer = new LayerDrawable(d.ToArray());
                        int startIndex = 0;
                        layer.SetLayerInset(0, 0, 0, 0, 0);
                        startIndex++;
                        if (string.IsNullOrWhiteSpace(Control.Text))
                        {
                            layer.SetLayerInset(1, 0, 0, 0, 0);
                            startIndex++;
                        }
                 
                        Control.SetBackground(layer);   

                    }
                    else
                    {
                        ChangeBackgroundImage();
                    }
                }
                else
                {
                    ChangeBackgroundPattern();
                }
            }

            if (e.PropertyName == nameof(element.BackgroundPattern))
            {
                ChangeBackgroundPattern();
            }

            if (e.PropertyName == nameof(element.BackgroundImage))
            {
                ChangeBackgroundImage();
            }
        }

        protected async void ChangeBackgroundImage()
        {
            var element = Element as CalendarButton;
            if (element == null || element.BackgroundImage == null) return;

            var d = new List<Drawable>();
            var image = await GetBitmap(element.BackgroundImage);
            d.Add(new BitmapDrawable(image));
            var drawable = new GradientDrawable();
            drawable.SetShape(ShapeType.Rectangle);
            var borderWidth = (int)Math.Ceiling(Element.BorderWidth);
            drawable.SetStroke(borderWidth > 0 ? borderWidth + 1 : borderWidth, Element.BorderColor.ToAndroid());
            drawable.SetColor(Android.Graphics.Color.Transparent);
            d.Add(drawable);
            var layer = new LayerDrawable(d.ToArray());
            layer.SetLayerInset(d.Count - 1, 0, 0, 0, 0);
            Control.SetBackground(layer);
        }

        protected void ChangeBackgroundPattern()
        {
            var element = Element as CalendarButton;
            if (element == null || element.BackgroundPattern == null || Control.Width == 0) return;

            var d = new List<Drawable>();
            for (var i = 0; i < element.BackgroundPattern.Pattern.Count; i++)
            {
                var bp = element.BackgroundPattern.Pattern[i];
                if (!string.IsNullOrEmpty(bp.Text))
                {
                    d.Add((Drawable) new TextDrawable(bp.Color.ToAndroid()) { Pattern = bp });
                }
                else
                {
                    d.Add(new ColorDrawable(bp.Color.ToAndroid()));
                }
            }
            var drawable = new GradientDrawable();
            drawable.SetShape(ShapeType.Rectangle);
            var borderWidth = (int)Math.Ceiling(Element.BorderWidth);
            drawable.SetStroke(borderWidth > 0 ? borderWidth + 1 : borderWidth, Element.BorderColor.ToAndroid());
            drawable.SetColor(Android.Graphics.Color.Transparent);
            d.Add(drawable);
            var layer = new LayerDrawable(d.ToArray());
            for (var i = 0; i < element.BackgroundPattern.Pattern.Count; i++)
            {
                var l = (int)Math.Ceiling(Control.Width * element.BackgroundPattern.GetLeft(i));
                var t = (int)Math.Ceiling(Control.Height * element.BackgroundPattern.GetTop(i));
                var r = (int)Math.Ceiling(Control.Width * (1 - element.BackgroundPattern.Pattern[i].WidthPercent)) - l;
                var b = (int)Math.Ceiling(Control.Height * (1 - element.BackgroundPattern.Pattern[i].HightPercent)) - t;
                layer.SetLayerInset(i, l, t, r, b);
            }
            layer.SetLayerInset(d.Count - 1, 0, 0, 0, 0);
            Control.SetBackground(layer);
        }

        Task<Bitmap> GetBitmap(FileImageSource image)
        {
            var handler = new FileImageSourceHandler();
            return handler.LoadImageAsync(image, this.Control.Context);
        }
    }

    public static class Calendar
    {
        public static void Init()
        {
            var d = "";
        }
    }

    public class TextDrawable : ColorDrawable
    {
        Paint paint;
        public Pattern Pattern { get; set; }

        public TextDrawable(Android.Graphics.Color color)
            : base(color)
        {
            paint = new Paint();
            paint.AntiAlias = true;
            paint.SetStyle(Paint.Style.Stroke);
            paint.TextAlign = Paint.Align.Left;
        }

        public override void Draw(Canvas canvas)
        {
            base.Draw(canvas);
            if(Pattern.IsSelected)
            {
                var radius = Bounds.Height() > Bounds.Width() ? (Bounds.Width()-10) / 2 : (Bounds.Height()-10) / 2;
                var selectedBackPaint = new Paint();
                selectedBackPaint.AntiAlias = true;
                selectedBackPaint.SetStyle(Paint.Style.Fill);
                selectedBackPaint.Color = Android.Graphics.Color.LightSkyBlue;
                canvas.DrawOval(new RectF(Bounds.CenterX()- radius, Bounds.CenterY()- radius, Bounds.CenterX() + radius, Bounds.CenterY() + radius), selectedBackPaint);

            }

            paint.Color = Pattern.TextColor.ToAndroid();
            paint.TextSize = Android.Util.TypedValue.ApplyDimension(Android.Util.ComplexUnitType.Sp, Pattern.TextSize > 0 ? Pattern.TextSize : 12, Forms.Context.Resources.DisplayMetrics);
            var bounds = new Rect();
            paint.GetTextBounds(Pattern.Text, 0, Pattern.Text.Length, bounds);
            var al = (int)Pattern.TextAlign;
            var x = Bounds.Left;
            if ((al & 2) == 2) // center
            {
                x = Bounds.CenterX() - Math.Abs(bounds.CenterX());
            }
            else if ((al & 4) == 4) // right
            {
                x = Bounds.Right - bounds.Width();
            }
            var y = Bounds.CenterY();
            if ((al & 16) == 16) // middle
            {
                y = Bounds.CenterY() + Math.Abs(bounds.CenterY());
            }
            else if ((al & 32) == 32) // bottom
            {
                y = Bounds.Bottom - Math.Abs(bounds.Bottom);
            }
            canvas.DrawText(Pattern.Text.ToCharArray(), 0, Pattern.Text.Length, x, y, paint);

            if(Pattern.AppointmentCount >0)
            {
                var dotpaint = new Paint();
                dotpaint.AntiAlias = true;
                dotpaint.SetStyle(Paint.Style.Fill);
                dotpaint.Color = Pattern.AppointmentFlagColor.ToAndroid();
                var startX = 10;
                if (Pattern.AppointmentCount == 1)
                    startX = (int)(Bounds.CenterX() - 5);
                else
                if (Pattern.AppointmentCount == 2)
                    startX = (int)(Bounds.CenterX() - 15);
                else
                    startX = (int)(Bounds.CenterX() - 25);
                for(int i=0;i< Pattern.AppointmentCount&&i<3;i++)
                {
                    canvas.DrawOval(new RectF(startX+i*20, y + 20, startX + 10 + i * 20, y+10), dotpaint);

                }

            }
        }
    }
}