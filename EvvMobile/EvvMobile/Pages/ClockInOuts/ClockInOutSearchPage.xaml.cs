using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using EvvMobile.Customizations.CustomControls;
using EvvMobile.Pages.Base;
using EvvMobile.ViewModels.ClockInOuts;
using EvvMobile.ViewModels.Systems;
using Xamarin.Forms;

namespace EvvMobile.Pages.ClockInOuts
{
    public partial class ClockInOutSearchPage : ClockInOutSearchPageXaml
    {
        public ClockInOutSearchPage()
        {
            InitializeComponent();
        }
        protected override void OnAppearing()
        {
            base.OnAppearing();
            SystemViewModel.Instance.CleanMessages();
            if (ViewModel.IsInitialized)
                return;

            ViewModel.InitSearchModelCommand.Execute(null);

        }

    }
    public class ClockInOutSearchPageXaml : ModelBoundWithHomeButtonContentPage<ClockInOutSearchViewModel> { }

}
