using System;
using System.Collections.ObjectModel;
using EvvMobile.Customizations;
using EvvMobile.ViewModels.Schedules;
using EvvMobile.Views.Base;
using Xamarin.Forms;

namespace EvvMobile.Views.Schedules
{
    public partial class VisitTaskListView : NonPersistentSelectedItemListView
    {
        public VisitTaskListView()
        {
            InitializeComponent();
            
        }
        public PopupLayout PopupLayout { get; set; }
        /// <summary>
        /// only comment can be edited
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void TaskDetailClicked(object sender, EventArgs e)
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
                var viewModel = SelectedItem as VisitTaskViewModel;
                if (viewModel == null)
                    return;
                var copySelectedItem = new VisitTaskViewModel
                {
                    Comment = viewModel.Comment,
                    EndDateTime = viewModel.EndDateTime,
                    IsScheduled = viewModel.IsScheduled,
                    IsTaskDeclined = viewModel.IsTaskDeclined,
                    StartDateTime = viewModel.StartDateTime,
                    Instruction = viewModel.Instruction,
                    TaskName = viewModel.TaskName,
                    IsShiftCompleted = viewModel.IsShiftCompleted,
                    Code = viewModel.Code,
                    Category = viewModel.Category
                };

                var popupView = new TaskPopupView()
                {

                    BackgroundColor = Color.Gray,
                    BindingContext = copySelectedItem,
                    HeightRequest = App.ScreenHeight * .5,
                    WidthRequest = App.ScreenWidth * .8

                };
                popupView.OkButtonClicked += (s, args) =>
                {
                    popupLayout.DismissPopup();
                    viewModel.Comment = copySelectedItem.Comment;

                };
                popupView.CancelButtonClicked += (s, args) =>
                {
                    popupLayout.DismissPopup();

                };

                popupLayout.ShowPopup(popupView);
            }
        }

        private void OnDeclineButtonClicked(object sender, EventArgs e)
        {
            var button = sender as Button;
            var viewModel = button?.BindingContext as VisitTaskViewModel;
            
            var popupLayout = PopupLayout;
            if (popupLayout.IsPopupActive)
            {
                popupLayout.DismissPopup();
            }
            else
            {

                if (viewModel == null)
                    return;
                if (!viewModel.IsScheduled)
                {
                    ((ScheduleViewModel)BindingContext).VisitTasks.Remove(viewModel);
                    return;
                }
                var declineVisitTaskViewModel = new DeclineVisitTaskViewModel
                {
                    DeclinedComment = viewModel.Comment,
                    TaskName = viewModel.TaskName
                };

                var popupView = new DeclineTaskView()
                {

                    BackgroundColor = Color.Gray,
                    BindingContext = declineVisitTaskViewModel,
                    HeightRequest = App.ScreenHeight * .5,
                    WidthRequest = App.ScreenWidth * .8

                };
                popupView.OkButtonClicked += (s, args) =>
                {
                    popupLayout.DismissPopup();
                    viewModel.Comment = declineVisitTaskViewModel.DeclinedComment;
                    viewModel.IsTaskDeclined = true;
                    viewModel.TaskResult = "Declined";
                };
                popupView.CancelButtonClicked += (s, args) =>
                {
                    popupLayout.DismissPopup();

                };

                popupLayout.ShowPopup(popupView);
            }
        }

        private void StartButton_OnClicked(object sender, EventArgs e)
        {
            var button = sender as Button;

            var viewModel = button?.BindingContext as VisitTaskViewModel;
            if (viewModel == null)
                return;
            viewModel.StartDateTime = DateTimeOffset.Now;
        }

        private void CompleteButton_OnClicked(object sender, EventArgs e)
        {
            var button = sender as Button;

            var viewModel = button?.BindingContext as VisitTaskViewModel;
            if (viewModel == null)
                return;
            viewModel.EndDateTime = DateTimeOffset.Now;
            viewModel.TaskResult = "Completed";
        }
    }
}