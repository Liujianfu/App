using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Evv.Message.Portable.Schedulers.Dtos.Common;
using EvvMobile.ViewModels.Base;
using Xamarin.Forms;

namespace EvvMobile.ViewModels.Schedules
{
    public class VisitTaskViewModel :BaseViewModel
    {
        public VisitTaskViewModel()
        {
        }
        public string ServiceVisitId { get; set; }
        public string Code { get; set; }
        public string Category { get; set; }
        public string Instruction { get; set; }
        public string TaskName { get; set; }
        public string Description { get;  set; }
        public LookupItemDto TaskType { get; set; }

        public string Comment { get; set; }

        private DateTimeOffset? _startDateTime;

        public DateTimeOffset? StartDateTime
        {
            get { return _startDateTime; }
            set
            {
                _startDateTime = value;
                OnPropertyChanged("StartDateTime");
                OnPropertyChanged("IsStarted");
                OnPropertyChanged("ShowCompleteButton");
            }
        }
        private DateTimeOffset? _endDateTime;

        public DateTimeOffset? EndDateTime
        {
            get { return _endDateTime; }
            set
            {
                _endDateTime = value;
                OnPropertyChanged("EndDateTime");
                OnPropertyChanged("IsCompleted");
                OnPropertyChanged("ShowCompleteButton");
            }
        }

        public bool IsScheduled { get; set; }
        public string TaskResult { get; set; }

        private bool _isTaskDeclined;
        public bool IsTaskDeclined
        {
            get { return _isTaskDeclined; }
            set
            {
                _isTaskDeclined = value;
                OnPropertyChanged("IsTaskDeclined");
            }
            
        }
        /// <summary>
        /// page use only, ignore this field when mapping dto to view model
        /// </summary>

        public bool IsShiftCompleted { get; set; }

        public bool IsStarted
        {
            get { return StartDateTime != null; }
        }
        public bool IsCompleted
        {
            get { return IsStarted &&EndDateTime != null; }
        }
        public bool ShowCompleteButton
        {
            get { return IsStarted && !IsCompleted; }
        }
    }
}
