using System;
using EvvMobile.Customizations;
using EvvMobile.Statics;
using EvvMobile.ViewModels.Messages;
using EvvMobile.ViewModels.Schedules;
using EvvMobile.Views.Base;
using EvvMobile.Views.Schedules;
using Xamarin.Forms;

namespace EvvMobile.Views.Messages
{
    public partial class MessageListView : NonPersistentSelectedItemListView
    {
        public MessageListView()
        {
            InitializeComponent();
        }
        public PopupLayout PopupLayout { get; set; }
        void MessageDetailClicked(object sender, EventArgs e)
        {
            if (SelectedItem == null)
            {
                SelectedItem = null;
                return;
            }
            var popupLayout = PopupLayout;
            if (popupLayout.IsPopupActive)
            {
                popupLayout.DismissPopup();
            }
            else
            {
                var viewModel = SelectedItem as MessageViewModel;
                if (viewModel == null)
                    return;
                if (!viewModel.IsViewed)
                    viewModel.SaveMessageCommand.Execute(null);

                var popupView = new MessageDetailPopupView()
                {

                    BackgroundColor = Color.Gray,
                    BindingContext = viewModel,
                    HeightRequest = App.ScreenHeight * .5,
                    WidthRequest = App.ScreenWidth * .8

                };
                popupView.OkButtonClicked += (s, args) =>
                {
                    popupLayout.DismissPopup();
                };

                popupLayout.ShowPopup(popupView);
            }
        }
    }
}
