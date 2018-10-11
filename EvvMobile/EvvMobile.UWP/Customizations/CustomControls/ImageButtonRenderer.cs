using System;
using System.ComponentModel;
using System.Threading.Tasks;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Media;
using EvvMobile.Customizations.CustomControls;
using EvvMobile.UWP.Customizations.CustomControls;
using Xamarin.Forms;
using Xamarin.Forms.Platform.UWP;
using Button = Xamarin.Forms.Button;
using Image = Windows.UI.Xaml.Controls.Image;
using ImageSource = Xamarin.Forms.ImageSource;
using Orientation = Windows.UI.Xaml.Controls.Orientation;
using TextAlignment = Windows.UI.Xaml.TextAlignment;
using Thickness = Windows.UI.Xaml.Thickness;

#if WINDOWS_APP || WINDOWS_PHONE_APP
using Xamarin.Forms.Platform.WinRT;
#elif WINDOWS_UWP

#endif


[assembly: ExportRenderer(typeof(ImageButton), typeof(ImageButtonRenderer))]

namespace EvvMobile.UWP.Customizations.CustomControls
{
    /// <summary>
    ///     Draws a button on the Windows Phone platform with the image shown in the right
    ///     position with the right size.
    /// </summary>
    public partial class ImageButtonRenderer : ButtonRenderer
    {
        /// <summary>
        ///     The image displayed in the button.
        /// </summary>
        private Image _currentImage;

        /// <summary>
        ///     Handles the initial drawing of the button.
        /// </summary>
        /// <param name="e">Information on the <see cref="ImageButton" />.</param>
        protected override async void OnElementChanged(ElementChangedEventArgs<Button> e)
        {
            base.OnElementChanged(e);
            UpdatePadding();
            await AssignContent();
        }

        /// <summary>
        ///     Called when the underlying model's properties are changed.
        /// </summary>
        /// <param name="sender">Model sending the change event.</param>
        /// <param name="e">Event arguments.</param>
        protected override void OnElementPropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            base.OnElementPropertyChanged(sender, e);

            if (e.PropertyName == nameof(ImageButton.Padding))
            {
                UpdatePadding();
            }
            if (e.PropertyName != ImageButton.SourceProperty.PropertyName &&
                e.PropertyName != ImageButton.DisabledSourceProperty.PropertyName &&
                e.PropertyName != VisualElement.IsEnabledProperty.PropertyName)
            {
                return;
            }

            var sourceButton = this.Element as ImageButton;
            if (sourceButton == null)
            {
                return;
            }

            Device.BeginInvokeOnMainThread(async () => await AssignContent());
        }

        private Task<Image> GetCurrentImage()
        {
            var sourceButton = this.Element as ImageButton;
            var targetButton = this.Control;
            if (sourceButton == null|| targetButton==null) return null;

            return GetImageAsync(
                (!sourceButton.IsEnabled && sourceButton.DisabledSource != null) ? sourceButton.DisabledSource : sourceButton.Source,
                GetHeight(sourceButton.ImageHeightRequest) - (int)(targetButton.FocusVisualPrimaryThickness.Bottom + targetButton.FocusVisualPrimaryThickness.Top),
                GetWidth(sourceButton.ImageWidthRequest) - (int)(targetButton.FocusVisualPrimaryThickness.Left + targetButton.FocusVisualPrimaryThickness.Right),
                null);
        }

        private async Task AssignContent()
        {
            var sourceButton = this.Element as ImageButton;
            var targetButton = this.Control;
            if (sourceButton != null && targetButton != null && sourceButton.Source != null)
            {
                var stackPanel = new StackPanel
                {
                    //Background = sourceButton.BackgroundColor.ToBrush(),
                    Orientation =
                    (sourceButton.Orientation == ImageOrientation.ImageOnTop
                     || sourceButton.Orientation == ImageOrientation.ImageOnBottom)
                        ? Orientation.Vertical
                        : Orientation.Horizontal,

                };

                this._currentImage = await GetCurrentImage();
                SetImageMargin(this._currentImage, sourceButton.Orientation);


                if (sourceButton.Orientation == ImageOrientation.ImageToLeft)
                {
                    targetButton.HorizontalContentAlignment = Windows.UI.Xaml.HorizontalAlignment.Left;
                }
                else if (sourceButton.Orientation == ImageOrientation.ImageToRight)
                {
                    targetButton.HorizontalContentAlignment = Windows.UI.Xaml.HorizontalAlignment.Right;
                }
                if (sourceButton.Orientation == ImageOrientation.ImageOnTop
                    || sourceButton.Orientation == ImageOrientation.ImageToLeft)
                {
                    this._currentImage.HorizontalAlignment = Windows.UI.Xaml.HorizontalAlignment.Left;
                    stackPanel.Children.Add(this._currentImage);
                    if (!string.IsNullOrWhiteSpace(sourceButton.Text))
                    {
                        var label = new TextBlock
                        {
                            TextAlignment = GetTextAlignment(sourceButton.Orientation),
                            FontSize = 16,
                            VerticalAlignment = Windows.UI.Xaml.VerticalAlignment.Center,
                            Text = sourceButton.Text
                        };
                        stackPanel.Children.Add(label);
                    }

                }
                else
                {

                    if (!string.IsNullOrWhiteSpace(sourceButton.Text))
                    {
                        var label = new TextBlock
                        {
                            TextAlignment = GetTextAlignment(sourceButton.Orientation),
                            FontSize = 16,
                            VerticalAlignment = Windows.UI.Xaml.VerticalAlignment.Center,
                            Text = sourceButton.Text
                        };
                        stackPanel.Children.Add(label);
                    }
                    stackPanel.Children.Add(this._currentImage);
                }

                targetButton.Content = stackPanel;
            }
        }
        /// <summary>
        ///     Returns the alignment of the label on the button depending on the orientation.
        /// </summary>
        /// <param name="imageOrientation">The orientation to use.</param>
        /// <returns>The alignment to use for the text.</returns>
        private static TextAlignment GetTextAlignment(ImageOrientation imageOrientation)
        {
            TextAlignment returnValue;
            switch (imageOrientation)
            {
                case ImageOrientation.ImageToLeft:
                    returnValue = TextAlignment.Left;
                    break;
                case ImageOrientation.ImageToRight:
                    returnValue = TextAlignment.Right;
                    break;
                default:
                    returnValue = TextAlignment.Center;
                    break;
            }

            return returnValue;
        }

        /// <summary>
        /// Returns a <see cref="Xamarin.Forms.Image" /> from the <see cref="Xamarin.Forms.ImageSource" /> provided.
        /// </summary>
        /// <param name="source">The <see cref="Xamarin.Forms.ImageSource" /> to load the image from.</param>
        /// <param name="height">The height for the image (divides by 2 for the Windows Phone platform).</param>
        /// <param name="width">The width for the image (divides by 2 for the Windows Phone platform).</param>
        /// <param name="currentImage">The current image.</param>
        /// <returns>A properly sized image.</returns>
        private static async Task<Image> GetImageAsync(ImageSource source, int height, int width, Image currentImage)
        {
            var image = currentImage ?? new Image();
            var handler = GetHandler(source);
            Windows.UI.Xaml.Media.ImageSource imageSource;
            if (source is FileImageSource)
            {
                var fileSource = source as FileImageSource;
                if (!fileSource.File.StartsWith("Assets/"))
                {
                    fileSource.File = "Assets/" + fileSource.File;                    
                }

                imageSource = await handler.LoadImageAsync(fileSource);
            }
            else
            {
                 imageSource = await handler.LoadImageAsync(source);
                
            }

            image.Source = imageSource;
            image.Height =height;
            image.Width =width;
            image.Stretch= Stretch.Fill;

            return image;
        }

        /// <summary>
        ///     Sets a margin of 10 between the image and the text.
        /// </summary>
        /// <param name="image">The image to add a margin to.</param>
        /// <param name="orientation">The orientation of the image on the button.</param>
        private static void SetImageMargin(Image image, ImageOrientation orientation)
        {
            const int DefaultMargin = 0;
            var left = 0;
            var top = 0;
            var right = 0;
            var bottom = 0;

            switch (orientation)
            {
                case ImageOrientation.ImageToLeft:
                    right = DefaultMargin;
                    break;
                case ImageOrientation.ImageOnTop:
                    bottom = DefaultMargin;
                    break;
                case ImageOrientation.ImageToRight:
                    left = DefaultMargin;
                    break;
                case ImageOrientation.ImageOnBottom:
                    top = DefaultMargin;
                    break;
            }

            image.Margin = new Thickness(left, top, right, bottom);
        }
        private static IImageSourceHandler GetHandler(ImageSource source)
        {
            IImageSourceHandler returnValue = null;
            if (source is UriImageSource)
            {
                returnValue = new UriImageSourceHandler();
            }
            else if (source is FileImageSource)
            {
                returnValue = new FileImageSourceHandler();
            }
            else if (source is StreamImageSource)
            {
                returnValue = new StreamImageSourceHandler();
            }
            return returnValue;
        }

        /// <summary>
        /// Gets the width based on the requested width, if request less than 0, returns 50.
        /// </summary>
        /// <param name="requestedWidth">The requested width.</param>
        /// <returns>The width to use.</returns>
        private int GetWidth(int requestedWidth)
        {
            const int DefaultWidth = 50;
            return requestedWidth <= 0 ? DefaultWidth : requestedWidth;
        }

        /// <summary>
        /// Gets the height based on the requested height, if request less than 0, returns 50.
        /// </summary>
        /// <param name="requestedHeight">The requested height.</param>
        /// <returns>The height to use.</returns>
        private int GetHeight(int requestedHeight)
        {
            const int DefaultHeight = 50;
            return requestedHeight <= 0 ? DefaultHeight : requestedHeight;
        }
        private void UpdatePadding()
        {
            var element = this.Element as ImageButton;
            if (element != null)
            {
                this.Control.Padding = new Thickness(

                    (int)element.Padding.Top,
                    (int)element.Padding.Left,
                    (int)element.Padding.Bottom,
                    (int)element.Padding.Right
                );
            }
        }


    }
}