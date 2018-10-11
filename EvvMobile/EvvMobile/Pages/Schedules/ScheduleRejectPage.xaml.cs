using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Evv.Message.Portable.Schedulers.Dtos;
using Evv.Message.Portable.Schedulers.Identifiers;
using EvvMobile.Pages.Base;
using EvvMobile.Statics;
using EvvMobile.ViewModels.Schedules;
using Xamarin.Forms;

namespace EvvMobile.Pages.Schedules
{
    public partial class ScheduleRejectPage : ScheduleRejectPageXaml
    {
        public ScheduleRejectPage()
        {
            InitializeComponent();
            NavigationPage.SetBackButtonTitle(this, "");
            //TODO: picker.Items should be populated using lookup items obtained from db

        }
        protected override void OnAppearing()
        {
            base.OnAppearing();
            var currentStaff = ViewModel.CurrentStaff();


            if (currentStaff != null )
            {
                if (currentStaff.StaffRejectLateReason == null)
                {
                    currentStaff.StaffRejectLateReason = new ReasonsCommentsDto
                    {
                        Category = ReasonCommentCategory.StaffRejctionReasonCategory,
                        Key = ReasonCommentCategory.OtherReasonKeyName,
                        SubKey = ViewModel.CurrentStaffId
                    };
                }
                if (currentStaff.StaffRejectLateReason.Category == ReasonCommentCategory.StaffRejctionReasonCategory)
                {
                    var index = StaffRejectionReasonPicker.Items.IndexOf(currentStaff.StaffRejectLateReason.Key);
                    if (index >= 0)
                        StaffRejectionReasonPicker.SelectedIndex = index;                    
                }

                ViewModel.PropertiesChanged();
            }
            //TODO: for client rejection

        }
        private void OnStaffReasonsSelectedIndexChanged(object sender, EventArgs e)
        {
            Picker picker = sender as Picker;
            if (picker != null)
            {
                var currentStaff = ViewModel.CurrentStaff();
                if (currentStaff != null)
                {
                    currentStaff.StaffRejectLateReason = new ReasonsCommentsDto
                    {
                        Category = ReasonCommentCategory.StaffRejctionReasonCategory,
                        Key =  picker.Items[picker.SelectedIndex],
                        SubKey = ViewModel.CurrentStaffId,
                        Content = picker.Items[picker.SelectedIndex] == ReasonCommentCategory.OtherReasonKeyName ?
                            ViewModel.OtherStaffRejectionReason : picker.Items[picker.SelectedIndex]
                    };
                    ViewModel.PropertiesChanged();
                }       
  
      
            }
        }

        private async void OnRejectButtonClicked(object sender, EventArgs e)
        {
            if (ViewModel.HasValidReason(ReasonCommentCategory.StaffRejctionReasonCategory))
            {
                var answer = await DisplayAlert("Reject a shift",
                    "Rejected shift will be permanently deleted from the list.Are you sure you want to reject the shift?",
                    "Yes", "No");
                if (answer)
                {
                    await ViewModel.ExecuteStaffRejectCommand(ViewModel);
                    if (ViewModel.CurrentStaffStatus != VisitStaffAssignmentStatus.Rejected)
                    {
                        await DisplayAlert("Reject a shift",
                            "Failed to reject the shift.",
                            "OK");
                    }
                   await Navigation.PopAsync();
                   
                }
            }
            else
            {
                await DisplayAlert("Reject a shift",
                    "Please enter the reason for rejecting the shift.",
                    "OK");
            }
        }
    }
    public abstract class ScheduleRejectPageXaml : ModelBoundWithHomeButtonContentPage<ScheduleViewModel> { }
}
