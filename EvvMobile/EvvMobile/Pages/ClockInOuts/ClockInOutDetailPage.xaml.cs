using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using EvvMobile.ViewModels.Systems;
using Xamarin.Forms;

namespace EvvMobile.Pages.ClockInOuts
{
    public partial class ClockInOutDetailPage : ContentPage
    {
        public ClockInOutDetailPage()
        {
            InitializeComponent();
            SystemViewModel.Instance.CleanMessages();
        }
    }
}
