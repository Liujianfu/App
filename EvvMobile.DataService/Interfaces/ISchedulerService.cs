using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Evv.Message.Portable.Schedulers.Dtos;
using Evv.Message.Portable.Schedulers.Dtos.Provider;
using Evv.Message.Portable.Schedulers.LxService;
using EvvMobile.RestfulWebService.Infrastructure.Common;

namespace EvvMobile.DataService.Interfaces
{
    public interface ISchedulerService
    {
        Task<HttpWebApiResponse<ServiceVisitsWithPaginationResultDto>> DownloadMyFurtureSchedules(ServiceVisitSearchCriteriaDto searchCriteria,bool isOnline);
        Task<HttpWebApiResponse<ServiceVisitsWithPaginationResultDto>> GetSchedules(ServiceVisitSearchCriteriaDto searchCriteria, bool isOnline);
        Task<HttpWebApiResponse<ServiceVisitDto>> StaffAccept(ServiceVisitDto serviceVisit, bool isOnline);
       

        Task<HttpWebApiResponse<ServiceVisitDto>> StaffReject(ServiceVisitDto serviceVisit, bool isOnline);
        Task<HttpWebApiResponse<ServiceVisitDto>> ClientAccept(ServiceVisitDto serviceVisit, bool isOnline);

        Task<HttpWebApiResponse<ServiceVisitDto>> ClientReject(ServiceVisitDto serviceVisit, bool isOnline);
        Task<HttpWebApiResponse<ServiceVisitDto>> ClockIn(ServiceVisitDto serviceVisit, bool isOnline);

        Task<HttpWebApiResponse<ServiceVisitDto>> ClockOut(ServiceVisitDto serviceVisit, bool isOnline);
        Task<HttpWebApiResponse<ServiceVisitDto>> Synchronize(ServiceVisitDto serviceVisit);

        Task<HttpWebApiResponse<ServiceVisitDto>> CreateAndClockIn(ServiceVisitDto serviceVisit, bool isOnline);
        Task<HttpWebApiResponse<IList<LxServiceDto> >> GetServiceTypesByMaNumber(string clientMaNumber, string providerMaNumber, DateTimeOffset serviceDate);

        Task<HttpWebApiResponse<ProviderDto>> GetActiveProviderByMaNumber(string providerMaNumber);
        Task<HttpWebApiResponse<ClientInformationDto>> GetClientInfoByMaNumber(string clientMaNumber);
        Task<bool> DeleteUnsyncedServiceVisit(ServiceVisitDto serviceVisit);

        Task<HttpWebApiResponse<ScheduleStatisticsDto>> GetScheduleStatusStatisticsForStaff(
            ScheduleStatisticsCriteriaDto scheduleStatisticsCriteria);

        Task<HttpWebApiResponse<MonthlyScheduleStatisticsDto>> GetMonthlyScheduleStatisticsForStaff(
            ScheduleStatisticsCriteriaDto scheduleStatisticsCriteria);
        Task<HttpWebApiResponse<MobileClientProfileDto>> GetMobileClientProfileByClientId(string ClientId);

    }
}
