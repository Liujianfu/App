using System;
using System.Threading.Tasks;
using EvvMobile.Pages.Base;
using EvvMobile.ViewModels.ClockInOuts;
using EvvMobile.ViewModels.Systems;
using Xamarin.Forms;

namespace EvvMobile.Pages.ClockInOuts
{
    public partial class ClockInOutOnlineCreatePage : ClockInOutOnlineCreatePageXaml
    {
        public ClockInOutOnlineCreatePage()
        {
            InitializeComponent();
            SystemViewModel.Instance.CleanMessages();

            Task.Run(() => { InitializeClockInOut(); });
            Title = "Clock In/Out";
        }

        private void ClockInButtonClicked(object sender, EventArgs e)
        {
            ClockInOut(true);
        }

        private void ClockOutButtonClicked(object sender, EventArgs e)
        {
            ClockInOut(false);
        }


        private void ClockInOut(bool clockIn)
        {
            try
            {
                ViewModel.IsBusy = true;
                SystemViewModel.Instance.CleanMessages();
                ViewModel.ClockInOutType = clockIn ? "ClockIn" : "ClockOut";

                ViewModel.SaveClockinCommand.Execute(null);
                
                var clockInOutDescription = clockIn ? "Clock In" : "Clock Out";

                SystemViewModel.Instance.InfoMessage = string.Format("{0} Successfully!", clockInOutDescription);
                ViewModel.IsBusy = false;
            }
            catch (Exception)
            {
                SystemViewModel.Instance.ErrorMessage = "Clock In/Out Failed";
            }

        }

        private void InitializeClockInOut()
        {
            if (SystemViewModel.Instance.HasNetworkConnection)
            {
                ViewModel.IsBusy = true;

                Task.Delay(1000).Wait();

                //
                ViewModel.ProviderNumber = "123456789";
                ViewModel.ClientMaNumber = "987654321";
                ViewModel.ProcedureCode = "U123:9:8";

                ViewModel.IsBusy = false;
            }
            else
            {

            }




            Device.BeginInvokeOnMainThread(
                () =>
                {

                });


        }

    }
    public abstract class ClockInOutOnlineCreatePageXaml : ModelBoundWithHomeButtonContentPage<ClockInOutViewModel> { }
}
