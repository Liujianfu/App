using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Evv.Message.Portable.Schedulers.Dtos;
using EvvMobile.DataService.Interfaces;
using EvvMobile.Database.Repositories;
using EvvMobile.Database.Models;
using AutoMapper;
using Evv.Message.Portable.Schedulers.Identifiers;
using EvvMobile.DataService.Extensions;

namespace EvvMobile.DataService.Services
{
    public class SchedulerServiceLocal : ISchedulerServiceLocal
    {
        public SchedulerServiceLocal()
        {
            serviceVisitRepository = new ServiceVisitRepository();
            visitStaffRepository = new VisitStaffRepository();
            visitTaskRepository = new VisitTaskRepository();
            visitMeasurementRepository = new VisitMeasurementRepository();
            atttAttributeNameValueRepository = new AttributeNameValueRepository();
        }

        public async Task<bool> InsertServiceVisitList(IList<ServiceVisitDto> serviceVisits)
        {
            bool result = false;
            await Task.Run(() =>
            {
                result = serviceVisitRepository.InsertServiceVisitList(serviceVisits) > 0;
            });
            return result;
        }

        public async Task<ServiceVisitsWithPaginationResultDto> GetLocalSchedules()
        {
           
   //         var serviceVisitList = await serviceVisitRepository.GetAllListAsync();
   //async has exception, don't know why
            var serviceVisitList = serviceVisitRepository.GetAllListSync();

            var dtos = Mapper.Map<List<ServiceVisit>, List<ServiceVisitDto>>(serviceVisitList);

            foreach (ServiceVisitDto visit in dtos)
            {
                var staffList =  visitStaffRepository.GetListByServiceVisitIdSync(visit.Id);
                 var taskList =  visitTaskRepository.GetListByServiceVisitIdSync(visit.Id);
                var measurementList = await visitMeasurementRepository.GetListByServiceVisitIdAsync(visit.Id);
                var attributeList = await atttAttributeNameValueRepository.GetListByServiceVisitIdAsync(visit.Id);

                MapServiceVisitToServiceVisitDto(visit, staffList, taskList, measurementList,  attributeList);
            }

            var result = new ServiceVisitsWithPaginationResultDto();
            result.ServiceVisitDtos = dtos;
            result.TotalNumber = dtos.Count;
            return result;
        }

        private static void MapServiceVisitToServiceVisitDto(ServiceVisitDto visit, List<VisitStaff> staffList, List<VisitTask> taskList,
            List<VisitMeasurement> measurementList, List<AttributeNameValue> attributeList)
        {
            if (staffList != null && staffList.Any())
            {
                visit.VisitStaffs = Mapper.Map<List<VisitStaff>, List<VisitStaffDto>>(staffList);
            }

            if (taskList != null && taskList.Any())
            {
                visit.VisitTasks = Mapper.Map<IList<VisitTask>, IList<VisitTaskDto>>(taskList);
            }

            if (measurementList != null && measurementList.Any())
            {
                visit.VisitMeasurements =
                    Mapper.Map<IList<VisitMeasurement>, IList<VisitMeasurementDto>>(measurementList);

                var attributes = attributeList.Where(x => x.ServiceVisitId == visit.Id).ToList();

                for (int index = 0; index < measurementList.Count; index++)
                {
                    var measurement = measurementList[index];
                    var measurementAttributes = attributes.Where(x => x.VisitMeasurementDbId == measurement.DbId).ToList();
                    visit.VisitMeasurements[index].Attributes =
                        Mapper.Map<IList<AttributeNameValue>, IList<AttributeNameValueDto>>(measurementAttributes);
                }
            }
        }

        public async Task<List<ServiceVisitDto>> GetUnsyncedSchedules()
        {
            List<ServiceVisitDto> UnsyncedList = null;
            var serviceVisitList = await serviceVisitRepository.GetAllListAsync();
 
            var UnsyncedVisitList = serviceVisitList.Where(x => x.IsUnsynced).ToList();
            if (UnsyncedVisitList.Any())
            {
                UnsyncedList = Mapper.Map<List<ServiceVisit>, List<ServiceVisitDto>>(UnsyncedVisitList);

                foreach (ServiceVisitDto visit in UnsyncedList)
                {
                    var staffList = await visitStaffRepository.GetListByServiceVisitIdAsync(visit.Id);
                    var taskList = await visitTaskRepository.GetListByServiceVisitIdAsync(visit.Id);
                    var measurementList = await visitMeasurementRepository.GetListByServiceVisitIdAsync(visit.Id);
                    var attributeList = await atttAttributeNameValueRepository.GetListByServiceVisitIdAsync(visit.Id);
                    MapServiceVisitToServiceVisitDto(visit, staffList, taskList, measurementList, attributeList);
                }
            }

            return UnsyncedList;
        }

        public async Task<bool> ClockIn(ServiceVisitDto serviceVisitDto, bool isUnsynced)
        {
            var visitInLocal = await serviceVisitRepository.GetObjectByIdAsync(serviceVisitDto.Id);

            visitInLocal.ClockInTime = serviceVisitDto.ClockInTime.ToDateTime();
            visitInLocal.IsUnsynced = isUnsynced;
            visitInLocal.IsClockInDone = true;
            visitInLocal.ActionType = Database.Common.ActionTypes.ClockIn;

            visitInLocal.ClockInAddress = serviceVisitDto.ClockInAddress;//current address. call google api?
            visitInLocal.ClockInLatitude = serviceVisitDto.ClockInLatitude;
            visitInLocal.ClockInLongitude = serviceVisitDto.ClockInLongitude;

            visitInLocal.WorkflowStatusName = serviceVisitDto.WorkflowStatus.Name;
            visitInLocal.WorkflowStatusDisplayName = serviceVisitDto.WorkflowStatus.DisplayName;

            //update visit staff in local db
            var localVisitStaffs = await visitStaffRepository.GetListByServiceVisitIdAsync(serviceVisitDto.Id);
            var localVisitStaff = localVisitStaffs.FirstOrDefault(x =>
                (x.StaffId == serviceVisitDto.CurrentStaffId) &&
                (x.StatusName == VisitStaffAssignmentStatus.Accepted.Name || x.StatusName == VisitStaffAssignmentStatus.AcceptanceNotRequired.Name)
            );
            var currentStaffDto =
                    serviceVisitDto.VisitStaffs.FirstOrDefault(x =>
                    x.StaffId.Equals(serviceVisitDto.CurrentStaffId, StringComparison.OrdinalIgnoreCase) &&
                    (x.Status.Name == VisitStaffAssignmentStatus.Accepted.Name || x.Status.Name == VisitStaffAssignmentStatus.AcceptanceNotRequired.Name)
                    );

            int count = 0;
            List<VisitStaff> visitStaffs = null;
            if (localVisitStaff != null && currentStaffDto != null)
            {
                localVisitStaff.StaffRejectLateReasonCategory = currentStaffDto.StaffRejectLateReason.Category;
                localVisitStaff.StaffRejectLateReasonContent = currentStaffDto.StaffRejectLateReason.Content;
                localVisitStaff.StaffRejectLateReasonKey = currentStaffDto.StaffRejectLateReason.Key;
                localVisitStaff.StaffRejectLateReasonSubKey = currentStaffDto.StaffRejectLateReason.SubKey;

            }

            visitStaffs = localVisitStaffs;

            List<VisitTask> visitTasks = null;
            if (serviceVisitDto.VisitTasks != null && serviceVisitDto.VisitTasks.Count > 0)
            {
                visitTasks = Mapper.Map<List<VisitTaskDto>, List<VisitTask>>(serviceVisitDto.VisitTasks.ToList());
            }

            //List<VisitMeasurement> visitMeasurements = null;
            //List<AttributeNameValue> attributes = null;
            //if (serviceVisit.VisitMeasurements != null && serviceVisit.VisitMeasurements.Count > 0)
            //{
            //    visitMeasurements =
            //        Mapper.Map<List<VisitMeasurementDto>, List<VisitMeasurement>>(serviceVisit.VisitMeasurements.ToList());

            //    attributes = new List<AttributeNameValue>();


            //    foreach (var visitMeasurement in serviceVisit.VisitMeasurements)
            //    {
            //        if (visitMeasurement.Attributes != null && visitMeasurement.Attributes.Count > 0)
            //        {
            //            attributes.AddRange(visitMeasurement.Attributes.Select(x => new AttributeNameValue()
            //            {
            //                AttributeName = x.AttributeName,
            //                AttributeValue = x.AttributeValue,


            //            }));
            //        }
            //    }
            //}

            count = serviceVisitRepository.ReplaceServiceVisit(visitInLocal, visitStaffs, visitTasks);

            return count > 0;
        }

        public async Task<bool> CreateAndClockIn(ServiceVisitDto serviceVisitDto, bool isUnsynced)
        {
            var visitInLocal = new ServiceVisit();

            visitInLocal.Id = "servicevisits/" + Database.Common.DbConstants.NewVisitIdPrefix + "/" + Guid.NewGuid().ToString();
            visitInLocal.ClockInTime = serviceVisitDto.ClockInTime.ToDateTime();
            visitInLocal.IsClockInDone = true;
            //sync info
            visitInLocal.IsUnsynced = isUnsynced;
            visitInLocal.IsUnscheduled = serviceVisitDto.IsUnscheduled;
            visitInLocal.ActionType = Database.Common.ActionTypes.CreateAndClockIn;
            visitInLocal.CurrentStaffId = serviceVisitDto.CurrentStaffId;

            visitInLocal.ClockInAddress = serviceVisitDto.ClockInAddress;//current address. call google api?
            visitInLocal.ClockInLatitude = serviceVisitDto.ClockInLatitude;
            visitInLocal.ClockInLongitude = serviceVisitDto.ClockInLongitude;

            visitInLocal.WorkflowStatusName = serviceVisitDto.WorkflowStatus.Name;
            visitInLocal.WorkflowStatusDisplayName = serviceVisitDto.WorkflowStatus.DisplayName;

            visitInLocal.ProviderName = serviceVisitDto.ProviderName;
            visitInLocal.ProviderMaNumber = serviceVisitDto.ProviderMaNumber;
            visitInLocal.ClientFirstName = serviceVisitDto.ClientName.FirstName;
            visitInLocal.ClientLastName = serviceVisitDto.ClientName.LastName;
            visitInLocal.ClientMiddleName = serviceVisitDto.ClientName.MiddleName;
            visitInLocal.ClientNameSuffix = serviceVisitDto.ClientName.Suffix;
            visitInLocal.ClientMaNumber = serviceVisitDto.ClientMaNumber;
            visitInLocal.ServiceStartDateTime = serviceVisitDto.ServiceStartDateTime.ToDateTime();
            visitInLocal.ServiceEndDateTime = serviceVisitDto.ServiceEndDateTime.ToDateTime();
            visitInLocal.ServiceName = serviceVisitDto.ServiceName;
            visitInLocal.ProcedureCodeAndModifier = serviceVisitDto.ProcedureCodeAndModifier;
            IList<VisitStaff> visitStaffs = new List<VisitStaff>();
            visitStaffs.Add(new VisitStaff() { ServiceVisitId = visitInLocal.Id, StaffId = serviceVisitDto.CurrentStaffId });

            IList<VisitTask> visitTasks = null;
            if (serviceVisitDto.VisitTasks != null && serviceVisitDto.VisitTasks.Any())
            {
                visitTasks = Mapper.Map<IList<VisitTaskDto>, IList<VisitTask>>(serviceVisitDto.VisitTasks);
            }

            var count = await serviceVisitRepository.InsertServiceVisit(visitInLocal, visitStaffs, visitTasks);
            return count > 0;
        }

        public async Task<bool> ClockOut(ServiceVisitDto serviceVisitDto, bool isUnsynced)
        {
            var visitInLocal = await serviceVisitRepository.GetObjectByIdAsync(serviceVisitDto.Id);

            IList<VisitStaff> visitStaffs = null;
            if (serviceVisitDto.VisitStaffs.Any())
            {
                visitStaffs = new List<VisitStaff>();
                visitStaffs = Mapper.Map<IList<VisitStaffDto>, IList<VisitStaff>>(serviceVisitDto.VisitStaffs);
            }

            IList<VisitTask> visitTasks = null;
            if (serviceVisitDto.VisitTasks.Any())
            {
                visitTasks = new List<VisitTask>();
                visitTasks = Mapper.Map<IList<VisitTaskDto>, IList<VisitTask>>(serviceVisitDto.VisitTasks);
            }

            visitInLocal.ClockOutTime = serviceVisitDto.ClockOutTime.ToDateTime();
            visitInLocal.IsUnsynced = isUnsynced;
            visitInLocal.IsClockOutDone = true;
            if (serviceVisitDto.ActionType != Database.Common.ActionTypes.CreateAndClockIn)
            {
                visitInLocal.ActionType = Database.Common.ActionTypes.ClockOut;
            }

            visitInLocal.ClockOutAddress = serviceVisitDto.ClockOutAddress;
            visitInLocal.ClockOutLatitude = serviceVisitDto.ClockOutLatitude;
            visitInLocal.ClockOutLongitude = serviceVisitDto.ClockOutLongitude;

            visitInLocal.WorkflowStatusName = serviceVisitDto.WorkflowStatus.Name;
            visitInLocal.WorkflowStatusDisplayName = serviceVisitDto.WorkflowStatus.DisplayName;

           var result = serviceVisitRepository.ReplaceServiceVisit(visitInLocal, visitStaffs, visitTasks) > 0;
            return result;
        }

        public async Task<bool> ReplaceServiceVisit(ServiceVisitDto serviceVisitDto)
        {
            bool result = false;
            await Task.Run(() =>
            {
                var visit = Mapper.Map<ServiceVisitDto, ServiceVisit>(serviceVisitDto);

                IList<VisitStaff> visitStaffs = null;
                if (serviceVisitDto.VisitStaffs.Any())
                {
                    visitStaffs = new List<VisitStaff>();
                    visitStaffs = Mapper.Map<IList<VisitStaffDto>, IList<VisitStaff>>(serviceVisitDto.VisitStaffs);
                }

                IList<VisitTask> visitTasks = null;
                if (serviceVisitDto.VisitTasks.Any())
                {
                    visitTasks = new List<VisitTask>();
                    visitTasks = Mapper.Map<IList<VisitTaskDto>, IList<VisitTask>>(serviceVisitDto.VisitTasks);
                }

                result = serviceVisitRepository.ReplaceServiceVisit(visit, visitStaffs, visitTasks) > 0;
            });
            return result;
        }

        public async Task<bool> UpdateIsUnsyncedToFalseById(string id)
        {
            return await serviceVisitRepository.UpdateIsUnsyncedByIdAsync(id) > 0;
        }

        public async Task<bool> DeleteUnsyncedServiceVisit(ServiceVisitDto serviceVisitDto)
        {

            await visitStaffRepository.DeleteObjectsByServiceVisitIdAsync(serviceVisitDto.Id);

            return await serviceVisitRepository.DeleteObjectByIdAsync(serviceVisitDto.Id) > 0;
        }

        #region Helpers

        //private void ChangeStaffAcceptanceStatus(ServiceVisit serviceVisit, IList<VisitStaff> visitStaffs, AcceptanceStatus staffAcceptanceStatus)
        //{
        //    switch (staffAcceptanceStatus)
        //    {
        //        case AcceptanceStatus.Rejected:
        //            {
        //                if (serviceVisit.StaffAcceptanceStatus != (int)AcceptanceStatus.Rejected && serviceVisit.ClientAcceptanceStatus != (int)AcceptanceStatus.Rejected &&
        //                    (serviceVisit.WorkflowStatusName == ServiceVisitWorkflowStatus.Scheduled.Name || serviceVisit.WorkflowStatusName == ServiceVisitWorkflowStatus.Initiated.Name))
        //                {
        //                    var allActiveStaffs =
        //                        visitStaffs.Where(x => x.StatusName != VisitStaffAssignmentStatus.RemovedByProvider.Name);
        //                    if (allActiveStaffs.Any(x => x.StatusName == VisitStaffAssignmentStatus.Rejected.Name))
        //                    {
        //                        serviceVisit.WorkflowStatusName = ServiceVisitWorkflowStatus.RejectedByStaff.Name;
        //                        serviceVisit.WorkflowStatusDisplayName = ServiceVisitWorkflowStatus.RejectedByStaff.DisplayName;
        //                        serviceVisit.StaffAcceptanceStatus = (int)AcceptanceStatus.Rejected;
        //                    }
        //                }
        //                break;
        //            }

        //        case AcceptanceStatus.Accepted:
        //            {
        //                //TODO: if both staff acceptance and mmis approval met,go to Scheduled
        //                var allActiveStaffs =
        //                    visitStaffs.Where(x => x.StatusName != VisitStaffAssignmentStatus.RemovedByProvider.Name);
        //                if (allActiveStaffs.All(x => x.StatusName == VisitStaffAssignmentStatus.AcceptanceNotRequired.Name ||
        //                                      x.StatusName == VisitStaffAssignmentStatus.Accepted.Name))
        //                {
        //                    if (serviceVisit.MmisAppovalStatus == MmisAppovalStatus.NotRequired.ToString() ||
        //                        serviceVisit.MmisAppovalStatus == MmisAppovalStatus.Approved.ToString())
        //                    {
        //                        serviceVisit.WorkflowStatusName = ServiceVisitWorkflowStatus.Scheduled.Name;
        //                        serviceVisit.WorkflowStatusDisplayName = ServiceVisitWorkflowStatus.Scheduled.DisplayName;
        //                    }
        //                    serviceVisit.StaffAcceptanceStatus = (int)AcceptanceStatus.Accepted;
        //                }
        //                break;
        //            }
        //    }

        //}

        #endregion

        #region Fields

        private ServiceVisitRepository serviceVisitRepository;
        private VisitStaffRepository visitStaffRepository;
        private VisitTaskRepository visitTaskRepository;
        private VisitMeasurementRepository visitMeasurementRepository;
        private AttributeNameValueRepository atttAttributeNameValueRepository;

        #endregion

    }
}
