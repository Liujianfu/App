using EvvMobile.Charts.Controls;
using EvvMobile.Charts.Events;
using AndroidColor = Android.Graphics.Color;

namespace EvvMobile.Droid.Charts
{
	using Android.Content;
	using Android.Graphics;
	using Android.Views;

    /// <summary>
	/// Class ChartSurface.
	/// </summary>
	public class ChartSurface : SurfaceView
	{
		/// <summary>
		/// The chart
		/// </summary>
		public Chart Chart;
		/// <summary>
		/// The paint
		/// </summary>
		public Paint Paint;
		/// <summary>
		/// The colors
		/// </summary>
		public AndroidColor[] Colors;
		/// <summary>
		/// The canvas
		/// </summary>
		public Canvas Canvas;

	    public AndroidColor BackGroundColor;
		/// <summary>
		/// Initializes a new instance of the <see cref="ChartSurface"/> class.
		/// </summary>
		/// <param name="context">The context.</param>
		/// <param name="chart">The chart.</param>
		/// <param name="color">The color.</param>
		/// <param name="colors">The colors.</param>
		public ChartSurface(Context context, Chart chart, AndroidColor color, AndroidColor[] colors,AndroidColor backGroundColor)
			: base(context)
		{
			SetWillNotDraw(false);

			Chart = chart;
			Paint = new Paint() { Color = color, StrokeWidth = 6 };
		    Paint.TextSize = 30;
		    Paint.FakeBoldText = true;
		    BackGroundColor = backGroundColor;
            Colors = colors;
		}

		/// <summary>
		/// Called when [draw].
		/// </summary>
		/// <param name="canvas">The canvas.</param>
		protected override void OnDraw(Canvas canvas)
		{
			base.OnDraw(canvas);
		    canvas.DrawColor(BackGroundColor);
            Canvas = canvas;

			Chart.OnDrawBar -= _chart_OnDrawBar;
			Chart.OnDrawBar += _chart_OnDrawBar;
			Chart.OnDrawCircle -= _chart_OnDrawCircle;
			Chart.OnDrawCircle += _chart_OnDrawCircle;
			Chart.OnDrawGridLine -= _chart_OnDrawGridLine;
			Chart.OnDrawGridLine += _chart_OnDrawGridLine;
			Chart.OnDrawLine -= _chart_OnDrawLine;
			Chart.OnDrawLine += _chart_OnDrawLine;
			Chart.OnDrawText -= _chart_OnDrawText;
			Chart.OnDrawText += _chart_OnDrawText;
			Chart.OnDrawPie -= _chart_OnDrawPie;
			Chart.OnDrawPie += _chart_OnDrawPie;

			Chart.DrawChart(canvas.Height,canvas.Width);
		}

		/// <summary>
		/// _chart_s the on draw bar.
		/// </summary>
		/// <param name="sender">The sender.</param>
		/// <param name="e">The e.</param>
		void _chart_OnDrawBar(object sender, Chart.DrawEventArgs<DoubleDrawingData> e)
		{
			Canvas.DrawRect((float)e.Data.XFrom, (float)e.Data.YFrom, (float)e.Data.XTo, (float)e.Data.YTo, new Paint() { Color = Colors[e.Data.SeriesNo] });
		}

		/// <summary>
		/// _chart_s the on draw circle.
		/// </summary>
		/// <param name="sender">The sender.</param>
		/// <param name="e">The e.</param>
		void _chart_OnDrawCircle(object sender, Chart.DrawEventArgs<SingleDrawingData> e)
		{
			Canvas.DrawCircle((float)e.Data.X, (float)e.Data.Y, (float)e.Data.Size, new Paint() { Color = Colors[e.Data.SeriesNo] , StrokeWidth = 7.5F });
		}

		/// <summary>
		/// _chart_s the on draw grid line.
		/// </summary>
		/// <param name="sender">The sender.</param>
		/// <param name="e">The e.</param>
		void _chart_OnDrawGridLine(object sender, Chart.DrawEventArgs<DoubleDrawingData> e)
		{
			Canvas.DrawLine((float)e.Data.XFrom, (float)e.Data.YFrom, (float)e.Data.XTo, (float)e.Data.YTo, Paint);
		}

		/// <summary>
		/// _chart_s the on draw line.
		/// </summary>
		/// <param name="sender">The sender.</param>
		/// <param name="e">The e.</param>
		void _chart_OnDrawLine(object sender, Chart.DrawEventArgs<DoubleDrawingData> e)
		{
			Canvas.DrawLine((float)e.Data.XFrom, (float)e.Data.YFrom, (float)e.Data.XTo, (float)e.Data.YTo, new Paint() { Color = Colors[e.Data.SeriesNo], StrokeWidth = 7.5F });
		}

		/// <summary>
		/// _chart_s the on draw text.
		/// </summary>
		/// <param name="sender">The sender.</param>
		/// <param name="e">The e.</param>
		void _chart_OnDrawText(object sender, Chart.DrawEventArgs<TextDrawingData> e)
		{
            
            Canvas.DrawText(e.Data.Text, (float)e.Data.X, (float)e.Data.Y, Paint);
		}

		/// <summary>
		/// _chart_s the on draw pie.
		/// </summary>
		/// <param name="sender">The sender.</param>
		/// <param name="e">The e.</param>
		void _chart_OnDrawPie(object sender, Chart.DrawEventArgs<PieDrawingData> e)
		{
			double pieDegrees = 360;
			double size = ((e.Data.X > e.Data.Y) ? e.Data.Y * 2 : e.Data.X * 2);
			for(int i = 0; i < e.Data.Percentages.Length; i++)
			{
				double value = e.Data.Percentages[i];

				Canvas.DrawArc(new RectF(0, 0, (float)size, (float)size), 0, (float)pieDegrees, true, new Paint() { Color = Colors[i] });
				pieDegrees -= value;
			}
		}
	}
}
