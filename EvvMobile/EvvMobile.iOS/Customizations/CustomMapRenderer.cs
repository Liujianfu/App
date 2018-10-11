using CoreLocation;
using EvvMobile.Customizations;
using EvvMobile.iOS.Customizations;
using MapKit;
using UIKit;
using Xamarin.Forms;
using Xamarin.Forms.Maps.iOS;
using Xamarin.Forms.Platform.iOS;

[assembly: ExportRenderer(typeof(CustomMap), typeof(CustomMapRenderer))]
namespace EvvMobile.iOS.Customizations
{
    public class CustomMapRenderer : MapRenderer
    {
        MKPolylineRenderer _polylineRenderer;

        protected override void OnElementChanged(ElementChangedEventArgs<View> e)
        {
            base.OnElementChanged(e);

            if (e.OldElement != null)
            {
                var nativeMap = Control as MKMapView;
                if (nativeMap != null) nativeMap.OverlayRenderer = null;
            }

            if (e.NewElement != null )
            {
                var formsMap = (CustomMap)e.NewElement;
                var nativeMap = Control as MKMapView;
               // if(nativeMap!=null)
              //      nativeMap.AddGestureRecognizer(new UIPanGestureRecognizer { CancelsTouchesInView = false });
                if (nativeMap != null && formsMap.ShowRoute)
                {
                    nativeMap.OverlayRenderer = GetOverlayRenderer;

                    CLLocationCoordinate2D[] coords = new CLLocationCoordinate2D[formsMap.RouteCoordinates.Count];

                    int index = 0;
                    foreach (var position in formsMap.RouteCoordinates)
                    {
                        coords[index] = new CLLocationCoordinate2D(position.Latitude, position.Longitude);
                        index++;
                    }

                    var routeOverlay = MKPolyline.FromCoordinates(coords);
                    nativeMap.AddOverlay(routeOverlay);
                }
            }
        }

        MKOverlayRenderer GetOverlayRenderer(MKMapView mapView, IMKOverlay overlay)
        {
            if (_polylineRenderer == null)
            {
                _polylineRenderer = new MKPolylineRenderer(overlay as MKPolyline);
                _polylineRenderer.FillColor = UIColor.Blue;
                _polylineRenderer.StrokeColor = UIColor.Red;
                _polylineRenderer.LineWidth = 3;
                _polylineRenderer.Alpha = 0.4f;
            }
            return _polylineRenderer;
        }
    }
}
