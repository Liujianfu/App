using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Evv.Message.Portable.Schedulers.Dtos;
using EvvMobile.ViewModels.Base;
using Xamarin.Forms;

namespace EvvMobile.ViewModels.Schedules
{
    public class DeclineVisitTaskViewModel : BaseViewModelWithValidation
    {
        public DeclineVisitTaskViewModel()
        {
        }
        public string TaskName { get; set; }
        private string _declinedComment;
        [Display(Name = "Declined Comment")]
        [Required]
        [StringLength(50, MinimumLength = 3)]
        public string DeclinedComment
        {
            get { return _declinedComment; }
            set
            {
                _declinedComment = value;
                ValidateProperty(value);
                OnPropertyChanged("DeclinedComment");
            }
        }

    }
}
