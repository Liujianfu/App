using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;
using EvvMobile.ViewModels.Schedules;

namespace EvvMobile.Statics
{

    /// <summary>
    /// TODO: will get service types from back end service, this is for testing
    /// </summary>
    public static class EvvServiceTypes
    {
        public static ServiceTypeViewModel EandDInHomeRespite = new ServiceTypeViewModel() {
            ServiceName = "E & D - In Home Respite",
            ProcedureCode = "S5150",
            AvailableModifiers =new List<string> {"U1"} };
        public static ServiceTypeViewModel EandDNonEmergencyTransportation = new ServiceTypeViewModel()
        {
            ServiceName = "E & D - Non Emergency Transportation ",
            ProcedureCode = "U1123",
            AvailableModifiers = new List<string> { "T1", "T2", "T3" ,"T4"}
        };
    }
}
