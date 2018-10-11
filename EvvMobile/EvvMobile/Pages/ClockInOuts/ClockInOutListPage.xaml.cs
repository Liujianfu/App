using EvvMobile.Pages.Base;
using EvvMobile.ViewModels.ClockInOuts;
using EvvMobile.ViewModels.Systems;
using Xamarin.Forms;

namespace EvvMobile.Pages.ClockInOuts
{
    public partial class ClockInOutListPage : ClockInOutListPageXaml
    {
        public ClockInOutListPage()
        {
            InitializeComponent();
        }
        protected override void OnAppearing()
        {
            base.OnAppearing();
            SystemViewModel.Instance.CleanMessages();
            if (ViewModel.IsInitialized)
                return;

            ViewModel.IsInitialized = true;
        }


        async void ClockInItemTapped(object sender, ItemTappedEventArgs e)
        {
        }
    }
    public abstract class ClockInOutListPageXaml : ModelBoundWithHomeButtonContentPage<ClockInOutListViewModel> { }
}
