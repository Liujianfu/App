using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using EvvMobile.Localization;
using EvvMobile.Pages.Base;
using EvvMobile.ViewModels.Messages;
using EvvMobile.ViewModels.Systems;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace EvvMobile.Pages.Messages
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class NewMessageListPage : NewMessageListPageXaml
    {
        public NewMessageListPage()
        {
            InitializeComponent();
        }
        protected override void OnAppearing()
        {
            base.OnAppearing();

            SystemViewModel.Instance.CleanMessages();
            ListItemSelected = false;
            if (ViewModel.IsInitialized)
            {
                return;
            }

            ViewModel.IsInitialized = true;
        }

        private bool ListItemSelected { get; set; }
        private static object locker = new object();
    }
    public class NewMessageListPageXaml : ModelBoundWithHomeButtonContentPage<MessageListViewModel> { }
}