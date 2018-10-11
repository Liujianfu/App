using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.Devices.Geolocation;
using Windows.Storage.Streams;
using Windows.UI.Xaml.Controls.Maps;
using EvvMobile.Customizations;
using EvvMobile.UWP.Customizations;
using Xamarin.Forms.Maps;
using Xamarin.Forms.Maps.UWP;
using Xamarin.Forms.Platform.UWP;

[assembly: ExportRenderer(typeof(CustomMap), typeof(CustomMapRenderer))]
namespace EvvMobile.UWP.Customizations
{
    public class CustomMapRenderer : MapRenderer
    {
        protected override void OnElementChanged(ElementChangedEventArgs<Map> e)
        {
            base.OnElementChanged(e);

            if (e.OldElement != null)
            {
                // Unsubscribe
            }

            if (e.NewElement != null)
            {
                var formsMap = (CustomMap)e.NewElement;
                var nativeMap = Control as MapControl;
                nativeMap.Children.Clear();
                if (formsMap != null)
                {
                    foreach (var pin in formsMap.Pins)
                    {
                        var snPosition = new BasicGeoposition { Latitude = pin.Position.Latitude, Longitude = pin.Position.Longitude };
                        var snPoint = new Geopoint(snPosition);

                        var mapIcon = new MapIcon();
                        //customized pin image
                     //   mapIcon.Image = RandomAccessStreamReference.CreateFromUri(new Uri("ms-appx:///pin.png"));
                        mapIcon.CollisionBehaviorDesired = MapElementCollisionBehavior.RemainVisible;
                        mapIcon.Location = snPoint;
                        mapIcon.NormalizedAnchorPoint = new Windows.Foundation.Point(0.5, 1.0);
                        mapIcon.Title = pin.Label;
                        nativeMap.MapElements.Add(mapIcon);
                    }
                }
                if (formsMap!=null &&formsMap.ShowRoute)
                {
                    var coordinates = new List<BasicGeoposition>();
                    foreach (var position in formsMap.RouteCoordinates)
                    {
                        coordinates.Add(new BasicGeoposition() { Latitude = position.Latitude, Longitude = position.Longitude });

                    }
                    if (coordinates.Any())
                    {
                        var polyline = new MapPolyline();
                        polyline.StrokeColor = Windows.UI.Color.FromArgb(128, 255, 0, 0);
                        polyline.StrokeThickness = 5;
                        polyline.Path = new Geopath(coordinates);
                        nativeMap.MapElements.Add(polyline);                    
                    }                    
                }


            }
        }
    }
}
