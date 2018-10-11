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
    public class AddVisitTaskViewModel : BaseViewModelWithValidation
    {
        public AddVisitTaskViewModel()
        {
        }
        private string _taskName;
        [Display(Name = "Task Name")]
        [Required]
        [StringLength(50, MinimumLength = 3)]
        public string TaskName
        {
            get { return _taskName; }
            set
            {
                _taskName = value;
                ValidateProperty(value);
                OnPropertyChanged("TaskName");
            }
        }
        private string _taskInstruction;
        [Display(Name = "Task Instruction")]
        [Required]
        [StringLength(150, MinimumLength = 20)]
        public string Instruction
        {
            get { return _taskInstruction; }
            set
            {
                _taskInstruction = value;
                ValidateProperty(value);
                OnPropertyChanged("Instruction");
            }
        }

        private string _taskCode;
        [Display(Name = "Task Code")]
        [Required]
        [StringLength(50, MinimumLength = 3)]
        public string TaskCode
        {
            get { return _taskCode; }
            set
            {
                _taskCode = value;
                ValidateProperty(value);
                OnPropertyChanged("TaskCode");
            }
        }

        private string _category;
        [Display(Name = "Category")]
        [Required]
        [StringLength(50, MinimumLength = 3)]
        public string Category
        {
            get { return _category; }
            set
            {
                _category = value;
                ValidateProperty(value);
                OnPropertyChanged("Category");
            }
        }
        /// <summary>
        /// TODO:define lookup items
        /// </summary>

        public string Comment { get; set; }

    }
}
