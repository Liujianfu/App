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
    public partial class MessageListPage : MessageListPageXaml
    {
        public MessageListPage()
        {
            InitializeComponent();
        }
        protected override void OnAppearing()
        {
            base.OnAppearing();

            SystemViewModel.Instance.CleanMessages();

            if (ViewModel.IsInitialized)
            {
                return;
            }
            ViewModel.LoadMessagesCommand.Execute(null);
            ViewModel.IsInitialized = true;
        }



    }
    public class MessageListPageXaml : ModelBoundWithHomeButtonContentPage<MessageListViewModel> { }
}