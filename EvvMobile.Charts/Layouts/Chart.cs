using EvvMobile.Charts.Extensions;

namespace EvvMobile.Charts
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using SkiaSharp;

    public abstract class Chart
    {
        #region Properties

        /// <summary>
        /// Gets or sets the global margin.
        /// </summary>
        /// <value>The margin.</value>
        public float Margin { get; set; } = 30;
        public float MarginLeft { get; set; } = 50;
        /// <summary>
        /// Gets or sets the text size of the labels.
        /// </summary>
        /// <value>The size of the label text.</value>
        public float LabelTextSize { get; set; } = 16;
        public float LabelTextMaxSpace { get; set; } = 100;
        /// <summary>
        /// Gets or sets the Label Height margin.
        /// </summary>
        /// <value>The margin.</value>
        public float LabelHeightMargin { get; set; } = 50;
        /// <summary>
        /// Gets or sets the color of the chart background.
        /// </summary>
        /// <value>The color of the background.</value>
        public SKColor BackgroundColor { get; set; } = SKColors.White;
        public SKColor AxisColor { get; set; } = SKColors.Black;
        public SKColor LabelColor { get; set; } = SKColors.Black;
        public bool IsValueLabelOnTop { get; set; } = false;
        /// <summary>
        /// Gets or sets the data entries.
        /// </summary>
        /// <value>The entries.</value>
        public IEnumerable<Entry> Entries { get; set; }

        /// <summary>
        /// Gets or sets the minimum value from entries. If not defined, it will be the minimum between zero and the 
        /// minimal entry value.
        /// </summary>
        /// <value>The minimum value.</value>
        public float MinValue
        {
            get
            {
                if (!this.Entries.Any())
                {
                    return 0;
                } 

                if (this.InternalMinValue == null)
                {
                    return Math.Min(0, this.Entries.Min(x => x.Value));
                } 

                return Math.Min(this.InternalMinValue.Value, this.Entries.Min(x => x.Value));
            }

            set
            {
                this.InternalMinValue = value;
            } 
        }

        /// <summary>
        /// Gets or sets the maximum value from entries. If not defined, it will be the maximum between zero and the 
        /// maximum entry value.
        /// </summary>
        /// <value>The minimum value.</value>
        public float MaxValue
        {
            get
            {
                if (!this.Entries.Any()) 
                {
                    return 0;
                } 

                if (this.InternalMaxValue == null)
                {
                   return Math.Max(0, this.Entries.Max(x => x.Value)); 
                } 

                return Math.Max(this.InternalMaxValue.Value, this.Entries.Max(x => x.Value));
            }

            set
            {
               this.InternalMaxValue = value; 
            } 
        }
        public string XUnitLabel { get; set; }
        public string YUnitLabel { get; set; }
        /// <summary>
        /// Gets or sets the internal minimum value (that can be null).
        /// </summary>
        /// <value>The internal minimum value.</value>
        protected float? InternalMinValue { get; set; }

        /// <summary>
        /// Gets or sets the internal max value (that can be null).
        /// </summary>
        /// <value>The internal max value.</value>
        protected float? InternalMaxValue { get; set; }

        #endregion

        #region Methods

        /// <summary>
        /// Draw the  graph onto the specified canvas.
        /// </summary>
        /// <param name="canvas">The canvas.</param>
        /// <param name="width">The width.</param>
        /// <param name="height">The height.</param>
        public void Draw(SKCanvas canvas, int width, int height)
        {
            canvas.Clear(this.BackgroundColor);

            this.DrawContent(canvas, width, height);
        }

        /// <summary>
        /// Draws the chart content.
        /// </summary>
        /// <param name="canvas">The canvas.</param>
        /// <param name="width">The width.</param>
        /// <param name="height">The height.</param>
        public abstract void DrawContent(SKCanvas canvas, int width, int height);

        /// <summary>
        /// Draws caption elements on the right or left side of the chart.
        /// </summary>
        /// <param name="canvas">The canvas.</param>
        /// <param name="width">The width.</param>
        /// <param name="height">The height.</param>
        /// <param name="entries">The entries.</param>
        /// <param name="isLeft">If set to <c>true</c> is left.</param>
        protected void DrawCaptionElements(SKCanvas canvas, int width, int height, List<Entry> entries, bool isLeft)
        {
            var labelHeightMargin = 2* this.LabelHeightMargin;
            var availableHeight = height - (2 * labelHeightMargin);
            var x = isLeft ? this.Margin : (width - this.Margin - this.LabelTextSize);
            var ySpace = (availableHeight - this.LabelTextSize) / ((entries.Count <= 1) ? 1 : entries.Count - 1);

            for (int i = 0; i < entries.Count; i++)
            {
                var entry = entries.ElementAt(i);
                var y = labelHeightMargin + (i * ySpace);
                if (entries.Count <= 1)
                {
                    y += (availableHeight - this.LabelTextSize) / 2;
                }

                var hasLabel = !string.IsNullOrEmpty(entry.Label);
                var hasValueLabel = !string.IsNullOrEmpty(entry.ValueLabel);

                if (hasLabel || hasValueLabel)
                {
                    var hasOffset = hasLabel && hasValueLabel;
                    var captionMargin = this.LabelTextSize * 0.60f;
                    var space = hasOffset ? captionMargin : 0;
                    var captionX = isLeft ? this.Margin : width - this.Margin - this.LabelTextSize;

                    using (var paint = new SKPaint
                    {
                        Style = SKPaintStyle.Fill,
                        Color = entry.Color,
                    })
                    {
                        var rect = SKRect.Create(captionX, y, this.LabelTextSize, this.LabelTextSize);
                        canvas.DrawRect(rect, paint);
                    }

                    if (isLeft)
                    {
                        captionX += this.LabelTextSize + captionMargin;
                    }
                    else
                    {
                        captionX -= captionMargin;
                    }

                    canvas.DrawCaptionLabels(entry.Label, entry.TextColor, entry.ValueLabel, entry.Color, this.LabelTextSize, new SKPoint(captionX, y + (this.LabelTextSize / 2)), isLeft ? SKTextAlign.Left : SKTextAlign.Right);
                }
            }
        }

        public void DrawGridLine(SKCanvas canvas, float xFrom,  float yFrom,float xTo, float yTo,SKColor lineColor)
        {
            using (var paint = new SKPaint
            {
                Style = SKPaintStyle.Stroke,
                StrokeWidth = 4,
                Color = lineColor,
            })
            {
                canvas.DrawLine(xFrom, yFrom, xTo, yTo,paint);
            }
        }
        #endregion
    }
}