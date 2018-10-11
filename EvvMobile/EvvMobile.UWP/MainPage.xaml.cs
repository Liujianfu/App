using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.Graphics.Display;
using Windows.UI.ViewManagement;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;

namespace EvvMobile.UWP
{
    public sealed partial class MainPage
    {
        public MainPage()
        {
            this.InitializeComponent();
            Xamarin.FormsMaps.Init("54IrxSJTW9mGduNMLSSQ~BppdJoZ9wY0gwAS7a1bKJw~AoT7uI-4iwC6I6B1usEohfmwt3kLwsdg0IDDNxWMyAofEdkLAPsaEyRyzrT6HbY2");
            var applicationView = ApplicationView.GetForCurrentView(); 
            var displayInformation = DisplayInformation.GetForCurrentView(); 
            var bounds = applicationView.VisibleBounds; 

            EvvMobile.App.ScreenWidth = bounds.Width ;
            EvvMobile.App.ScreenHeight = bounds.Height;
            EvvMobile.App.ScreenHeightForTrackingMap= bounds.Height - 200;
            LoadApplication(new EvvMobile.App());
        }
    }
}
