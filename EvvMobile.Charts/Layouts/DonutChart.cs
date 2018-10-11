
using EvvMobile.Charts.Helpers;

namespace EvvMobile.Charts.Layouts
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using SkiaSharp;

    /// <summary>
    /// ![chart](../images/Donut.png)
    /// 
    /// A donut chart.
    /// </summary>
    public class DonutChart : Chart
    {
        #region Properties

        /// <summary>
        /// Gets or sets the radius of the hole in the center of the chart.
        /// </summary>
        /// <value>The hole radius.</value>
        public float HoleRadius { get; set; } = 0;

        #endregion

        #region Methods

        public override void DrawContent(SKCanvas canvas, int width, int height)
        {
            this.DrawCaption(canvas, width, height);
            using (new SKAutoCanvasRestore(canvas))
            {
                canvas.Translate(width / 2, height / 2);
                var sumValue = this.Entries.Sum(x => Math.Abs(x.Value));
                var radius = Math.Min(width-MarginLeft- Margin -2* LabelTextMaxSpace, height- 2 * Margin)  / 2;

                var start = 0.0f;
                for (int i = 0; i < this.Entries.Count(); i++)
                {
                    var entry = this.Entries.ElementAt(i);
                    var end = start + (Math.Abs(entry.Value) / sumValue);

                    // Sector
                    var path = RadialHelpers.CreateSectorPath(start, end, radius, radius * this.HoleRadius);
                    using (var paint = new SKPaint
                    {
                        Style = SKPaintStyle.Fill,
                        Color = entry.Color,
                        IsAntialias = true,
                    })
                    {
                        canvas.DrawPath(path, paint);
                    }

                    start = end;
                }
            }
        }

        private void DrawCaption(SKCanvas canvas, int width, int height)
        {
            var rightValues = new List<Entry>();
            var leftValues = new List<Entry>();

            int i = 0;
            var midIndex = this.Entries.Count() > 5 ? this.Entries.Count() / 2 : this.Entries.Count();
            while (i < midIndex)
            {
                var entry = this.Entries.ElementAt(i);
                rightValues.Add(entry);
                i++;
            }
            while (i < this.Entries.Count())
            {
                var entry = this.Entries.ElementAt(i);
                leftValues.Add(entry);
                i++;
            }

            this.DrawCaptionElements(canvas, width, height, rightValues, false);
            this.DrawCaptionElements(canvas, width, height, leftValues, true);
        }

        #endregion
    }
}