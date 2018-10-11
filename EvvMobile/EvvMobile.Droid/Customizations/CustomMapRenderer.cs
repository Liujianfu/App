using System.Collections.Generic;
using Android.Gms.Maps;
using Android.Gms.Maps.Model;
using EvvMobile.Customizations;
using EvvMobile.Droid.Customizations;
using Xamarin.Forms;
using Xamarin.Forms.Maps;
using Xamarin.Forms.Maps.Android;

[assembly: ExportRenderer(typeof(CustomMap), typeof(CustomMapRenderer))]
namespace EvvMobile.Droid.Customizations
{
    public class CustomMapRenderer : MapRenderer, IOnMapReadyCallback
    {
        GoogleMap map;
        private CustomMap formsMap;
        List<Position> routeCoordinates;
        private bool isDrawn = false;
        protected override void OnElementChanged(Xamarin.Forms.Platform.Android.ElementChangedEventArgs<Map> e)
        {
            base.OnElementChanged(e);

            if (e.OldElement != null)
            {
                // Unsubscribe
            }

            if (e.NewElement != null)
            {
                formsMap = (CustomMap)e.NewElement;
                routeCoordinates = formsMap.RouteCoordinates;

                ((MapView)Control).GetMapAsync(this);

            }
        }
        protected override void OnElementPropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            base.OnElementPropertyChanged(sender, e);

            if (e.PropertyName.Equals("VisibleRegion"))
            {
                map = NativeMap;
                map.Clear();
                if (formsMap != null)
                {
                    foreach (var pin in formsMap.Pins)
                    {
                        var marker = new MarkerOptions();
                        marker.SetPosition(new LatLng(pin.Position.Latitude, pin.Position.Longitude));
                        marker.SetTitle(pin.Label);
                        marker.SetSnippet(pin.Address);
                        map.AddMarker(marker);
                    }                    
                }
                if (formsMap !=null && formsMap.ShowRoute)
                {
                    var polylineOptions = new PolylineOptions();
                    polylineOptions.InvokeColor(0x66FF0000);

                    foreach (var position in routeCoordinates)
                    {
                        polylineOptions.Add(new LatLng(position.Latitude, position.Longitude));
                    }

                    map.AddPolyline(polylineOptions);                    
                }

                isDrawn = true;
            }
        }

    }
}