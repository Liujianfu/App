using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using EvvMobile.Statics;
using EvvMobile.ViewModels.Base;

namespace EvvMobile.ViewModels.Schedules
{
    public class AddShiftViewModel:BaseViewModelWithValidation
    {

        public AddShiftViewModel()
        {
            Modifiers = new ObservableCollection<ModifierCellViewModel>();
            ServiceTypes = new ObservableCollection<ServiceTypeViewModel>();
            IsUnscheduled = true;
        }
        #region Provider Info
        private string _providerName;
        [Display(Name = "Provider Name")]
        [Required]
        [StringLength(50)]
        public string ProviderName
        {
            get { return _providerName; }
            set
            {
                _providerName = value;
                ValidateProperty(value);
                OnPropertyChanged("ProviderName");
            }
        }

        private string _providerMaNumber;
        [Display(Name = "Provider Ma#")]
        [Required]
        [StringLength(8,MinimumLength = 8)]//should be 9
        public string ProviderMaNumber
        {
            get { return _providerMaNumber; }
            set
            {
                _providerMaNumber = value;
                ValidateProperty(value);
                OnPropertyChanged("ProviderMaNumber");
            }
        }

        #endregion

        #region Client Info

        private string _clientFullName;
        [Display(Name = "Beneficiary Name")]
        [Required]
        [StringLength(30, MinimumLength = 2)]
        public string ClientFullName
        {
            get { return _clientFullName; }
            set
            {
                _clientFullName = value;
                ValidateProperty(value);
                OnPropertyChanged("ClientFullName");
            }
        }

        private string _clientMaNumber;
        [Display(Name = "Beneficiary MA#")]
        [Required]
        [StringLength(9, MinimumLength = 9)]
        public string ClientMaNumber
        {
            get { return _clientMaNumber; }
            set
            {
                _clientMaNumber = value;
                ValidateProperty(value);
                OnPropertyChanged("ClientMaNumber");
            }
        }

        public string ClientSignatureBase64Img { get; set; }
        #endregion

        #region Visit Info

        public DateTimeOffset ServiceStartDateTime { get; set; }

        public DateTimeOffset ServiceEndDateTime { get; set; }


        private string _procedureCode;
        [Display(Name = "Procedure Code")]
        [Required]
        [StringLength(10, MinimumLength = 1)]
        public string ProcedureCode
        {
            get { return _procedureCode; }
            set
            {
                _procedureCode = value;
                ValidateProperty(value);
                OnPropertyChanged("ProcedureCode");
            }

        }
        private string _serviceName;
        [Display(Name = "Service Name")]
        [Required]
        public string SelectedServiceName
        {
            get { return _serviceName; }
            set
            {
                _serviceName = value;
                ValidateProperty(value);
                OnPropertyChanged("SelectedServiceName");
            }

        }
        public ObservableCollection<ServiceTypeViewModel> ServiceTypes { get; set; }
        public ObservableCollection<ModifierCellViewModel> Modifiers { get; set; }
        public string AuthorizationNumber { get; set; }
        #endregion
        public bool IsUnscheduled { get; set; }
        public ScheduleListViewModel ParentListViewModel { get; set; }
    }
}
