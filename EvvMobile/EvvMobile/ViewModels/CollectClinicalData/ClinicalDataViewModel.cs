using System;
using EvvMobile.ViewModels.Base;
using EvvMobile.ViewModels.Schedules;
using System.Collections.ObjectModel;
namespace EvvMobile.ViewModels.CollectClinicalData
{
    public class ClinicalDataViewModel : BaseViewModel
    {
        public  ClinicalDataViewModel(ScheduleViewModel scheduleViewModel)
        {
            this._scheduleViewModel = scheduleViewModel;
            VisitMeasurements = scheduleViewModel.VisitMeasurements;
            
        }
        public bool IsEditable { get; set; }
        public ObservableCollection<VisitMeasurementViewModel> VisitMeasurements { get; set; }
        private ScheduleViewModel _scheduleViewModel;
    }
}
