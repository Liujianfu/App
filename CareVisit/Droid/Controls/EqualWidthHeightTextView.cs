using System;
using Android.Content;
using Android.Graphics;
using Android.Text;
using Android.Util;
using Android.Views;
using Android.Widget;


namespace CareVisit.Droid.Controls
{

    public class EqualWidthHeightTextView : TextView
    {

        public EqualWidthHeightTextView(Context context):base(context)
    {

    }

    public EqualWidthHeightTextView(Context context, IAttributeSet attrs) : base(context, attrs)
        {

    }

        public EqualWidthHeightTextView(Context context, IAttributeSet attrs, int defStyleAttr):base(context, attrs, defStyleAttr)
    {
        
    }

        protected override void OnMeasure(int widthMeasureSpec, int heightMeasureSpec)
        {
            base.OnMeasure(widthMeasureSpec,  heightMeasureSpec);
            int h = this.MeasuredHeight;
            int w = this.MeasuredWidth;
            int r = Math.Max(w, h);
            this.Gravity = GravityFlags.Center;
            SetMeasuredDimension(r, r);

        }
        public override void Draw(Canvas canvas)
        {
            // Grab canvas dimensions.
             int canvasWidth = canvas.Width;
             int canvasHeight = canvas.Height;

            // Calculate horizontal center.
             float centerX = canvasWidth * 0.5f;

            // Draw the background.
            float centerY = (float)Math.Round(canvasHeight * 0.5f);
            Paint circlePaint = new Paint();
            circlePaint.Flags = PaintFlags.AntiAlias;
            circlePaint.Color = Color.LightSkyBlue;
            int h = this.MeasuredHeight;
            int w = this.MeasuredWidth;
            int r = Math.Max(w, h);
            canvas.DrawCircle(centerX, centerY, r/2,circlePaint);

            // Draw text.
            TextPaint textPaint = new TextPaint();
            textPaint.Flags = PaintFlags.AntiAlias;
            textPaint.Color=Color.Black;

            textPaint.TextSize=this.TextSize;
             float baselineY = (float)Math.Round(canvasHeight * 0.5f+ this.TextSize/3);
            // Measure the width of text to display.
            float textWidth = textPaint.MeasureText(this.Text);
            // Figure out an x-coordinate that will center the text in the canvas.
             float textX =(float) Math.Round(centerX - textWidth * 0.5f);
            // Draw.
            canvas.DrawText(this.Text, textX, baselineY, textPaint);
        }
    }
}
