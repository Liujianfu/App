using System;
using EvvMobile.ViewModels.Base;
using System.Collections.ObjectModel;

namespace EvvMobile.ViewModels.CollectClinicalData
{
    public class VisitMeasurementViewModel : BaseViewModel
    {
       public  VisitMeasurementViewModel()
        {
            IsEditable = true;
        }
        public string Code { get; set; }
        public string Instruction { get; set; }
        public string Name { get; set; }
        public ObservableCollection<MeasurementAttributeViewModel> Attributes { get; set; }
        
        public bool IsEditable { get; set; }
    }
    public class MeasurementAttributeViewModel : BaseViewModel
    {
        public MeasurementAttributeViewModel()
        {
            IsEditable = true;
        }
        public string AttributeName { get; set; }
        public string AttributeValue { get; set; }

        public bool IsEditable { get; set; }
    }

}
