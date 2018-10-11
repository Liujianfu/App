using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using EvvMobile.Pages.Base;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;
using EvvMobile.ViewModels.CollectClinicalData;
namespace EvvMobile.Pages.CollectClinicalData
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class CollectClinicalDataPage : CollectClinicalDataPageXaml
    {
        public CollectClinicalDataPage()
        {
            InitializeComponent();
        }
        protected override void OnAppearing()
        {
            base.OnAppearing();
        }


    }


    public abstract class CollectClinicalDataPageXaml : ModelBoundContentPage<ClinicalDataViewModel> { }
}