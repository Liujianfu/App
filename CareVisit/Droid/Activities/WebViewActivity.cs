using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Webkit;
using Android.Widget;
using CareVisit.Core;

namespace CareVisit.Droid.Activities
{
    [Activity(Label = "WebViewActivity")]
    public class WebViewActivity : Activity
    {
        WebView web_view;
        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
            SetContentView(Resource.Layout.WebView);
            var resetPasswordUrl = GlobalSetting.Instance.ResetPasswordUrl;
            web_view = FindViewById<WebView>(Resource.Id.webview);
            web_view.Settings.JavaScriptEnabled = true;
            web_view.SetWebViewClient(new WebViewClient());
            web_view.LoadUrl(resetPasswordUrl);
        }
    }

    public class WebViewClient : Android.Webkit.WebViewClient
    {
        public override bool ShouldOverrideUrlLoading(WebView view, IWebResourceRequest request)
        {
            view.LoadUrl(request.Url.ToString());
            return false;
        }
    }
}