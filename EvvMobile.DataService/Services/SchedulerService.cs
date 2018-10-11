using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AutoMapper;
using Evv.Message.Portable.Schedulers.Dtos;
using EvvMobile.Database.Models;
using EvvMobile.DataService.Interfaces;
using EvvMobile.RestfulWebService.Infrastructure.Extensions;
using EvvMobile.Database.Repositories;
using Evv.Message.Portable.Schedulers.LxService;
using Evv.Message.Portable.Schedulers.Dtos.Provider;
using EvvMobile.RestfulWebService.Infrastructure.Common;

namespace EvvMobile.DataService.Services
{
    public class SchedulerService: ISchedulerService
    {
        public SchedulerService(string baseUrl)
        {
            _httpClientManager = new HttpClientManager();
            _baseUrl = baseUrl;
            _schedulerServiceLocal = new SchedulerServiceLocal();
        }
        /// <summary>
        /// Download my future schedules, store them into local database for offline access
        /// </summary>
        /// <param name="searchCriteria"></param>
        /// <returns></returns>
        public async Task<HttpWebApiResponse<ServiceVisitsWithPaginationResultDto>> DownloadMyFurtureSchedules(ServiceVisitSearchCriteriaDto searchCriteria, bool isOnline)
        {
            if (isOnline)
            {
                try
                {
                    var client = _httpClientManager.GetOrAdd(_baseUrl);
                    var result = await client.ExtensionPostAsJsonAsync<ServiceVisitsWithPaginationResultDto>("SchedulerMobile/FindServiceVisits",
                      searchCriteria);

                     if (result.ModelObject != null)
                     {
                         foreach (var visitDto in result.ModelObject.ServiceVisitDtos)
                         {
                             visitDto.CurrentStaffId = searchCriteria.VisitStaffId;
                         }
                         await _schedulerServiceLocal.InsertServiceVisitList(result.ModelObject.ServiceVisitDtos);
                     }
                     else
                     {
                         return result;
                     }
                   
                }
                catch (Exception e)
                {
                    var result = new HttpWebApiResponse<ServiceVisitsWithPaginationResultDto>();
                    result.ErrorMessage = e.Message;
                    return result;
                }

            }

            //else get shifts from local db
            var serviceVisistsResult = await _schedulerServiceLocal.GetLocalSchedules();
            return new HttpWebApiResponse<ServiceVisitsWithPaginationResultDto>(serviceVisistsResult); 
        }

        /// <summary>
        /// Get all schedules by specified criterias, DO NOT UPDATE local database
        /// need to check if the shifts is in local database, if it is , need to compbine local and online data
        /// </summary>
        /// <param name="searchCriteria"></param>
        /// <returns></returns>
        public async Task<HttpWebApiResponse<ServiceVisitsWithPaginationResultDto>> GetSchedules(ServiceVisitSearchCriteriaDto searchCriteria, bool isOnline)
        {
            //TODO: update result with local data if it exists
            var client = _httpClientManager.GetOrAdd(_baseUrl);
            var result = await client.ExtensionPostAsJsonAsync<ServiceVisitsWithPaginationResultDto>("SchedulerMobile/FindServiceVisits",
                searchCriteria);
            return result;
        }

        public async Task<HttpWebApiResponse<ServiceVisitDto> >StaffAccept(ServiceVisitDto serviceVisit, bool isOnline)
        {
            var currentStaffId = serviceVisit.CurrentStaffId;
            try
            {
                var client = _httpClientManager.GetOrAdd(_baseUrl);
                var result = await client.ExtensionPostAsJsonAsync<ServiceVisitDto>("SchedulerMobile/StaffAccept",
                    serviceVisit);
                if (result.ResponseStatus != ResponseStatuses.Failed)
                {
                    var visitDto = result.ModelObject;

                    //update local db
                    visitDto.CurrentStaffId = currentStaffId;
                    await _schedulerServiceLocal.ReplaceServiceVisit(visitDto);
                    
                }
                return result;
            }
            catch (Exception e)
            {
                var result = new HttpWebApiResponse<ServiceVisitDto>();
                result.ErrorMessage = e.Message;
                return result;
            }

        }

        public async Task<HttpWebApiResponse<ServiceVisitDto>> StaffReject(ServiceVisitDto serviceVisit, bool isOnline)
        {
            var currentStaffId = serviceVisit.CurrentStaffId;
            try
            {
                var client = _httpClientManager.GetOrAdd(_baseUrl);
                var result = await client.ExtensionPostAsJsonAsync<ServiceVisitDto>("SchedulerMobile/StaffReject",
                    serviceVisit);
                if (result.ResponseStatus != ResponseStatuses.Failed)
                {
                    var visitDto = result.ModelObject;
                    //update local db
                    visitDto.CurrentStaffId = currentStaffId;
                    await _schedulerServiceLocal.ReplaceServiceVisit(serviceVisit);

                    
                }
                return result;
            }
            catch (Exception e)
            {
                 var result= new HttpWebApiResponse<ServiceVisitDto>();
                result.ErrorMessage = e.Message;
                return result;
            }
        }

        public async Task<HttpWebApiResponse<ServiceVisitDto>> ClientAccept(ServiceVisitDto serviceVisit, bool isOnline)
        {

            var client = _httpClientManager.GetOrAdd(_baseUrl);
            var result = await client.ExtensionPostAsJsonAsync<ServiceVisitDto>("SchedulerMobile/ClientAccept",
                serviceVisit);
            return result;
        }

        public async Task<HttpWebApiResponse<ServiceVisitDto>> ClientReject(ServiceVisitDto serviceVisit, bool isOnline)
        {
            var client = _httpClientManager.GetOrAdd(_baseUrl);
            var result = await client.ExtensionPostAsJsonAsync<ServiceVisitDto>("SchedulerMobile/ClientReject",
                serviceVisit);
            return result;
        }

        public async Task<HttpWebApiResponse<ServiceVisitDto>> ClockIn(ServiceVisitDto serviceVisit, bool isOnline)
        {
            var visitDto = serviceVisit;
            if (isOnline)
            {
                try
                {
                    var client = _httpClientManager.GetOrAdd(_baseUrl);
                    var result = await client.ExtensionPostAsJsonAsync<ServiceVisitDto>("SchedulerMobile/ClockIn",
                        serviceVisit);
                    if (result.ModelObject != null)
                    {
                        visitDto = result.ModelObject;
                        visitDto.CurrentStaffId = serviceVisit.CurrentStaffId;
                        await _schedulerServiceLocal.ReplaceServiceVisit(visitDto);
                        return result;
                    }
                }
                catch (Exception e)
                {
                    var result = new HttpWebApiResponse<ServiceVisitDto>();
                    result.ErrorMessage = e.Message;
                    return result;
                }
            }
            visitDto.IsUnsynced = true;
            try
            {
                await _schedulerServiceLocal.ClockIn(visitDto, true);
            }
            catch (Exception e)
            {
                var result = new HttpWebApiResponse<ServiceVisitDto>();
                result.ErrorMessage = e.Message;
                return result;
            }


            return new HttpWebApiResponse<ServiceVisitDto>(visitDto);
        }

        public async Task<HttpWebApiResponse<ServiceVisitDto>> ClockOut(ServiceVisitDto serviceVisit, bool isOnline)
        {
            var visitDto = serviceVisit;
            if (isOnline)
            {
                try
                {
                    var client = _httpClientManager.GetOrAdd(_baseUrl);
                    var result = await client.ExtensionPostAsJsonAsync<ServiceVisitDto>("SchedulerMobile/ClockOut",
                        serviceVisit);
                    if (result.ModelObject != null)
                    {
                        result.ModelObject.CurrentStaffId = serviceVisit.CurrentStaffId;
                        await _schedulerServiceLocal.ReplaceServiceVisit(result.ModelObject);
                        return result;
                    }
                }
                catch (Exception e)
                {
                    var result = new HttpWebApiResponse<ServiceVisitDto>();
                    result.ErrorMessage = e.Message;
                    return result;
                }
            }
            visitDto.IsUnsynced = true;
            try
            {
                await _schedulerServiceLocal.ClockOut(visitDto, true);
            }
            catch (Exception e)
            {
                var result = new HttpWebApiResponse<ServiceVisitDto>();
                result.ErrorMessage = e.Message;
                return result;
            }

            return new HttpWebApiResponse<ServiceVisitDto>(visitDto);
        }

        public async Task<HttpWebApiResponse<ServiceVisitDto>> Synchronize(ServiceVisitDto serviceVisit)
        {
            var client = _httpClientManager.GetOrAdd(_baseUrl);
            if (serviceVisit.ActionType == Database.Common.ActionTypes.CreateAndClockIn)
            {
                var synchronizeResult = await client.ExtensionPostAsJsonAsync<ServiceVisitDto>(
                    "SchedulerMobile/CreateAndClockIn", serviceVisit);
                if (synchronizeResult.ModelObject != null)
                {
                    synchronizeResult.ModelObject.CurrentStaffId = serviceVisit.CurrentStaffId;
                    await _schedulerServiceLocal.ReplaceServiceVisit(synchronizeResult.ModelObject);
                }
                return synchronizeResult;
            }
            else
            {
                var synchronizeResult = await client.ExtensionPostAsJsonAsync<ServiceVisitDto>("SchedulerMobile/Synchronize",
                    serviceVisit);

                if (synchronizeResult.ModelObject != null)
                {
                    synchronizeResult.ModelObject.CurrentStaffId = serviceVisit.CurrentStaffId;
                    await _schedulerServiceLocal.ReplaceServiceVisit(synchronizeResult.ModelObject);
                }
                return synchronizeResult;
            }

        }

        public async Task<HttpWebApiResponse<ServiceVisitDto>> CreateAndClockIn(ServiceVisitDto serviceVisit, bool isOnline)
        {
            var currentStaffId = serviceVisit.CurrentStaffId;
            var visitDto = serviceVisit;
            var isUnsynced = true;
            if (isOnline)
            {
                try
                {
                    var client = _httpClientManager.GetOrAdd(_baseUrl);
                    var result = await client.ExtensionPostAsJsonAsync<ServiceVisitDto>("SchedulerMobile/CreateAndClockIn",
                        serviceVisit);
                    if (result.ModelObject != null)
                    {
                        visitDto = result.ModelObject;
                        visitDto.CurrentStaffId = serviceVisit.CurrentStaffId;
                        await _schedulerServiceLocal.ReplaceServiceVisit(visitDto);
                        return result;
                    }
                }
                catch (Exception e)
                {
                    var result= new HttpWebApiResponse<ServiceVisitDto>();
                    result.ErrorMessage = e.Message;
                    return result;
                }
            }
            visitDto.CurrentStaffId = currentStaffId;
            visitDto.IsUnsynced = isUnsynced;
            await _schedulerServiceLocal.CreateAndClockIn(visitDto, isUnsynced);

            return new HttpWebApiResponse<ServiceVisitDto>(visitDto);
        }

        public async Task<HttpWebApiResponse<IList<LxServiceDto>>> GetServiceTypesByMaNumber(string clientMaNumber, string providerMaNumber, DateTimeOffset serviceDate)
        {
            var parameters = new Dictionary<string, object>();
            parameters.Add("clientMaNumber", clientMaNumber);
            parameters.Add("providerMaNumber", providerMaNumber);
            parameters.Add("serviceActivityDate", serviceDate);
            var client = _httpClientManager.GetOrAdd(_baseUrl);
            var result = await client.ExtensionGetAsync<IList<LxServiceDto>>("SchedulerMobile/GetServiceTypesByMaNumber",
                parameters);
            return result;
        }

        public async Task<HttpWebApiResponse<ProviderDto>> GetActiveProviderByMaNumber(string providerMaNumber)
        {
            var parameters = new Dictionary<string, object>();
            parameters.Add("providerMaNumber", providerMaNumber);
            var client = _httpClientManager.GetOrAdd(_baseUrl);
            var result = await client.ExtensionGetAsync<ProviderDto>("SchedulerMobile/GetActiveProviderByMaNumber",
                parameters);
            return result;
        }

        public async Task<HttpWebApiResponse<ClientInformationDto>> GetClientInfoByMaNumber(string clientMaNumber)
        {
            var parameters = new Dictionary<string, object>();
            parameters.Add("clientMaNumber", clientMaNumber);
            var client = _httpClientManager.GetOrAdd(_baseUrl);
            var response = await client.ExtensionGetAsync<ClientInformationDto>("SchedulerMobile/GetClientInfoByMaNumber",
                parameters);
            return response;
        }

        public async Task<bool> DeleteUnsyncedServiceVisit(ServiceVisitDto serviceVisit)
        {
            return await _schedulerServiceLocal.DeleteUnsyncedServiceVisit(serviceVisit);
        }

        public async Task<HttpWebApiResponse<ScheduleStatisticsDto>> GetScheduleStatusStatisticsForStaff(ScheduleStatisticsCriteriaDto scheduleStatisticsCriteria)
        {
            var client = _httpClientManager.GetOrAdd(_baseUrl);
            var result = await client.ExtensionPostAsJsonAsync<ScheduleStatisticsDto>("SchedulerMobile/GetScheduleStatusStatisticsForStaff",
                scheduleStatisticsCriteria);
            return result;
        }

        public async Task<HttpWebApiResponse<MonthlyScheduleStatisticsDto>> GetMonthlyScheduleStatisticsForStaff(ScheduleStatisticsCriteriaDto scheduleStatisticsCriteria)
        {
            var client = _httpClientManager.GetOrAdd(_baseUrl);
            var result = await client.ExtensionPostAsJsonAsync<MonthlyScheduleStatisticsDto>("SchedulerMobile/GetMonthlyScheduleStatisticsForStaff",
                scheduleStatisticsCriteria);
            return result;
        }

        public async Task<HttpWebApiResponse<MobileClientProfileDto>> GetMobileClientProfileByClientId(string ClientId)
        {
            var parameters = new Dictionary<string, object>();
            parameters.Add("clientId", ClientId);
            var client = _httpClientManager.GetOrAdd(_baseUrl);
            var response = await client.ExtensionGetAsync<MobileClientProfileDto>("SchedulerMobile/GetMobileClientProfileByClientId",
                parameters);
            return response;
        }

        #region fields
        private IHttpClientManager _httpClientManager;
        private string _baseUrl;
        private ISchedulerServiceLocal _schedulerServiceLocal; 
        #endregion

    }
}
