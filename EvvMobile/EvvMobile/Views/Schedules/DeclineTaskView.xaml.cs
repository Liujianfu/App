using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using EvvMobile.ViewModels.Schedules;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace EvvMobile.Views.Schedules
{
    public partial class DeclineTaskView : ContentView
    {
        public DeclineTaskView()
        {
            InitializeComponent();
        }
        public event EventHandler OkButtonClicked;
        public event EventHandler CancelButtonClicked;
        private void OkButton_OnClicked(object sender, EventArgs e)
        {
            var viewModel = BindingContext as DeclineVisitTaskViewModel;
            if (viewModel != null)
            {
                viewModel.Validate();
                if (viewModel.HasErrors)
                {
                    // Error message
                    viewModel.ScrollToControlProperty(viewModel.GetFirstInvalidPropertyName);
                    return;
                }                
            }

            OkButtonClicked?.Invoke(sender, e);
        }

        private void CancelButton_OnClicked(object sender, EventArgs e)
        {
            CancelButtonClicked?.Invoke(sender, e);
        }
    }
}