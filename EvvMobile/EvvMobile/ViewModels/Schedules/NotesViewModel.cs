using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Evv.Message.Portable.Schedulers.Dtos;
using EvvMobile.ViewModels.Base;

namespace EvvMobile.ViewModels.Schedules
{
    public class NotesViewModel: BaseViewModel
    {
        public NotesViewModel()
        {
            Notes = new ObservableCollection<ReasonsCommentsDto>();
        }
        public ObservableCollection<ReasonsCommentsDto> Notes { get; set; }
    }
}
