using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using EvvMobile.Database.Models;
using Xamarin.Forms.Maps;

namespace EvvMobile.Services
{
    public static class RouteService
    {
        public static Double DistanceInMiles(double lat1, double lon1, double lat2, double lon2)
        {

            if (lat1 == lat2 && lon1 == lon2)
                return 0.0;

            var theta = lon1 - lon2;

            var distance = Math.Sin(deg2rad(lat1)) * Math.Sin(deg2rad(lat2)) +
                           Math.Cos(deg2rad(lat1)) * Math.Cos(deg2rad(lat2)) *
                           Math.Cos(deg2rad(theta));

            distance = Math.Acos(distance);
            if (Double.IsNaN(distance))
                return 0.0;

            distance = rad2deg(distance);
            distance = distance * 60.0 * 1.1515;

            return (distance);
        }
        public static MapSpan CenterOfRoute(List<Location> locations)
        {
            if (locations == null || !locations.Any())
                return null;
            if (locations.Count == 1)
            {
                return new MapSpan(new Position(locations.First().Latitude, locations.First().Longitude), 0.1, 0.1);
            }
            Location firstPostion = null;
            var crossed180Longitude = false;
            foreach (var p in locations)
            {
                if (firstPostion == null)
                {
                    firstPostion = p;
                    continue;
                }

                var secondPosition = p;
                crossed180Longitude = IsCrossedLongitude180(firstPostion, secondPosition);
                if (crossed180Longitude == true)
                    break;
                firstPostion = secondPosition;
            }
            double maxLat = -90;
            double maxLon = -180;
            double minLat = 90;
            double minLon = 180;
            double centerLatitude = 0.0;
            double centerLongitude = 0.0;
            double latitudeDegrees = 0.0;
            double logitudeDegrees = 0.0;

            if (crossed180Longitude)
            {
                maxLon = 180;
                minLon = -180;
            }
            foreach (var location in locations)
            {
                if (crossed180Longitude)
                {
                    if (location.Latitude > maxLat)
                        maxLat = location.Latitude;
                    if (location.Latitude < minLat)
                        minLat = location.Latitude;
                    if (location.Longitude > 0 && location.Longitude < 180 && location.Longitude < maxLon)
                        maxLon = location.Longitude;
                    else
                    if (location.Longitude <= 0 && location.Longitude >= -180 && location.Longitude > minLon)
                        minLon = location.Longitude;

                }
                else
                {
                    if (location.Latitude > maxLat)
                        maxLat = location.Latitude;
                    if (location.Latitude < minLat)
                        minLat = location.Latitude;

                    if (location.Longitude > maxLon)
                        maxLon = location.Longitude;
                    if (location.Longitude < minLon)
                        minLon = location.Longitude;
                }
            }
            if (crossed180Longitude)
            {
                centerLatitude = (maxLat + minLat) / 2;
                latitudeDegrees = (centerLatitude - minLat) * 2;
                centerLongitude = (maxLon + minLon) / 2;
                if (centerLongitude < 0)
                {
                    centerLongitude = 180 + centerLongitude;
                    logitudeDegrees = (centerLongitude - maxLon) * 2;
                }
                else
                {
                    centerLongitude = -180 + centerLongitude;
                    logitudeDegrees = (minLon - centerLongitude) * 2;
                }
            }
            else
            {
                centerLatitude = (maxLat + minLat) / 2;
                latitudeDegrees = (centerLatitude - minLat) * 2 + 0.005;
                centerLongitude = (maxLon + minLon) / 2;
                logitudeDegrees = (centerLongitude - minLon) * 2 + 0.005;
            }

            return new MapSpan(new Position(centerLatitude, centerLongitude), latitudeDegrees, logitudeDegrees);
        }

        private static bool IsCrossedLongitude180(Location start, Location end)
        {
            if ((start.Longitude > -90.00 && start.Longitude < -180.00) &&
                (end.Longitude < 180.00 && end.Longitude > 90.00))
            {
                return true;
            }
            else
            if ((end.Longitude > -90.00 && end.Longitude < -180.00) &&
                (start.Longitude < 180.00 && start.Longitude > 90.00))
            {
                return true;
            }

            return false;
        }

        private static double deg2rad(double deg)
        {
            return (deg * Math.PI / 180.0);
        }

        private static double rad2deg(double rad)
        {
            return (rad / Math.PI * 180.0);
        }
    }
}
