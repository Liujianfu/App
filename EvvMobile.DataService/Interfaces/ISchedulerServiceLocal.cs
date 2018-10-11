using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Evv.Message.Portable.Schedulers.Dtos;
using EvvMobile.Database.Models;


namespace EvvMobile.DataService.Interfaces
{
    public interface ISchedulerServiceLocal
    {
        Task<ServiceVisitsWithPaginationResultDto> GetLocalSchedules();
        Task<List<ServiceVisitDto>> GetUnsyncedSchedules();
        Task<bool> InsertServiceVisitList(IList<ServiceVisitDto> serviceVisits);
        Task<bool> ClockIn(ServiceVisitDto serviceVisit, bool isUnsynced);
        Task<bool> ClockOut(ServiceVisitDto serviceVisit, bool isUnsynced);
        Task<bool> CreateAndClockIn(ServiceVisitDto serviceVisit, bool isUnsynced);
        Task<bool> ReplaceServiceVisit(ServiceVisitDto serviceVisitDto);
        Task<bool> UpdateIsUnsyncedToFalseById(string id);
        Task<bool> DeleteUnsyncedServiceVisit(ServiceVisitDto serviceVisitDto);
    }
}
