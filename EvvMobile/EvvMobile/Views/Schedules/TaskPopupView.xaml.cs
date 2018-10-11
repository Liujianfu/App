using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace EvvMobile.Views.Schedules
{
    public partial class TaskPopupView : ContentView
    {
        public TaskPopupView()
        {
            InitializeComponent();
        }
        public event EventHandler OkButtonClicked;
        public event EventHandler CancelButtonClicked;
        private void OkButton_OnClicked(object sender, EventArgs e)
        {
            if(OkButtonClicked!=null)
                OkButtonClicked(sender,  e);
        }

        private void CancelButton_OnClicked(object sender, EventArgs e)
        {
            if (CancelButtonClicked != null)
                CancelButtonClicked(sender, e);
        }
    }
}