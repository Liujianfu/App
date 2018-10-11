using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EvvMobile.ViewModels.Schedules
{
    public class ServiceTypeViewModel
    {
        public string ServiceName { get; set; }
        public string ProcedureCode { get; set; }
        public IList<string> AvailableModifiers { get; set; }
    }
}
