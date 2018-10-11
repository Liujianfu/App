using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Xamarin.Forms;

namespace EvvMobile.Views.Account
{
    public partial class LogonView : ContentView
    {
        public LogonView()
        {
            InitializeComponent();
        }
        private void RegisterButtonClicked(object sender, EventArgs e)
        {
            var parent = this.Parent as StackLayout;
            if (parent != null)
            {
                parent.Children.Clear();
                
            }
        }

        private void LogonButtonClicked(object sender, EventArgs e)
        {


        }
    }
}
