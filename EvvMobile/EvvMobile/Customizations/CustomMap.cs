using System;
using System.Collections.Generic;
using EvvMobile.Customizations.CustomControls.Calendar;
using Xamarin.Forms;
using Xamarin.Forms.Maps;

namespace EvvMobile.Customizations
{
    public class CustomMap : Map
    {
        public List<Position> RouteCoordinates { get; set; }
        public static readonly BindableProperty ShowRouteProperty =
            BindableProperty.Create(nameof(ShowRoute), typeof(bool), typeof(CustomMap), true);
        public bool ShowRoute { get; set; }
        public CustomMap()
        {
            RouteCoordinates = new List<Position>();
        }
    }
}
