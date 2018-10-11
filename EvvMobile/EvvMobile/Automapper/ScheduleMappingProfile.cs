using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AutoMapper;
using Evv.Message.Portable.Schedulers.Dtos;
using EvvMobile.ViewModels.Schedules;
using EvvMobile.ViewModels.Systems;
using EvvMobile.Database.Models;
using EvvMobile.DataService.Extensions;
using EvvMobile.ViewModels.CollectClinicalData;
namespace EvvMobile.Automapper
{
    public class ScheduleMappingProfile : Profile
    {
        protected override void Configure()
        {

            Mapper.CreateMap<ScheduleViewModel, ServiceVisitDto>();
            //dto to view model, if view model is too complex, we can add a converter to convert the type
            Mapper.CreateMap<ServiceVisitDto, ScheduleViewModel>()
                .IgnoreAllPropertiesWithAnInaccessibleSetter()
                .ForMember(dest=>dest.ClockInTime, opt => opt.MapFrom(x => x.ClockInTime.ToDateTime().ToDateTimeOffset()))//remove time zone part
                .ForMember(dest => dest.ClockOutTime, opt => opt.MapFrom(x => x.ClockOutTime.ToDateTime().ToDateTimeOffset()))//remove time zone part
                .ForMember(dest => dest.ServiceStartDateTime, opt => opt.MapFrom(x => x.ServiceStartDateTime.ToDateTime().ToDateTimeOffset()))//remove time zone part
                .ForMember(dest => dest.ServiceEndDateTime, opt => opt.MapFrom(x => x.ServiceEndDateTime.ToDateTime().ToDateTimeOffset()))//remove time zone part
                .ForMember(dest => dest.Navigation, opt => opt.Ignore())
                .ForMember(dest => dest.IsInitialized, opt => opt.Ignore())
                .ForMember(dest => dest.Title, opt => opt.Ignore())
                .ForMember(dest => dest.Subtitle, opt => opt.Ignore())
                .ForMember(dest => dest.Icon, opt => opt.Ignore())
                .ForMember(dest => dest.IsInitialized, opt => opt.Ignore())
                .ForMember(dest => dest.ClientFullName, opt => opt.Ignore())
                .ForMember(dest => dest.ParentContainerViewModel, opt => opt.Ignore())
                .ForMember(dest => dest.OtherReasonForLateOrEarly, opt => opt.Ignore())
                .ForMember(dest => dest.ProcedureCode, opt => opt.Ignore())
                .ForMember(dest => dest.Modifiers, opt => opt.Ignore())
                .ForMember(dest => dest.OtherStaffRejectionReason, opt => opt.Ignore())
                .ForMember(dest => dest.WorkflowStatusName, opt => opt.Ignore())
                .ForMember(dest => dest.WorkflowStatusDisplayName, opt => opt.Ignore())
                .ForMember(dest => dest.TravelDistance, opt => opt.Ignore())//TODO:remove this
                .ForMember(dest=>dest.ClientFaceRecognized, opt=> opt.Ignore())//TODO:remove this
                .ForMember(dest => dest.LocationTrackingId, opt => opt.Ignore())//TODO:remove this
                //.ForMember(dest => dest.CurrentStaffId, opt => opt.MapFrom(x => SystemViewModel.Instance.CurrentStaffId))//get it from sso login
                .ForMember(dest => dest.CurrentStaffId, opt => opt.MapFrom(x => x.VisitStaffs != null && x.VisitStaffs.FirstOrDefault() != null ?
                         x.VisitStaffs.FirstOrDefault().StaffId : ""));//this is for testing
            //map VisitTaskDto &VisitTaskViewModel
            Mapper.CreateMap<VisitTaskDto, VisitTaskViewModel>()
                .IgnoreAllPropertiesWithAnInaccessibleSetter()
                .ForMember(dest => dest.IsShiftCompleted, opt => opt.Ignore());
            Mapper.CreateMap<VisitTaskViewModel, VisitTaskDto>();

            Mapper.CreateMap<VisitMeasurementDto, VisitMeasurementViewModel>()
                .IgnoreAllPropertiesWithAnInaccessibleSetter()
                .ForMember(dest => dest.IsEditable, opt => opt.Ignore());
            Mapper.CreateMap<VisitMeasurementViewModel, VisitMeasurementDto>();
            Mapper.CreateMap<AttributeNameValueDto, MeasurementAttributeViewModel>()
                .IgnoreAllPropertiesWithAnInaccessibleSetter()
                .ForMember(dest => dest.IsEditable, opt => opt.Ignore());
            Mapper.CreateMap<MeasurementAttributeViewModel, AttributeNameValueDto>();


            Mapper.CreateMap<ServiceVisit, ServiceVisitDto>()
                .IgnoreAllPropertiesWithAnInaccessibleSetter()
                .ForMember(dest => dest.Notes, opt => opt.Ignore())
                .ForMember(dest => dest.VisitStaffs, opt => opt.Ignore())
                .ForMember(dest => dest.ClientName, opt => opt.MapFrom(x => new PersonNameDto()
                { FirstName = x.ClientFirstName, LastName = x.ClientLastName, MiddleName = x.ClientMiddleName, Suffix = x.ClientNameSuffix }))
                .ForMember(dest => dest.ClientPhone, opt => opt.MapFrom(x => new PhoneDto()
                { ExtensionNumber = x.ClientPhoneExtNumber, Number = x.ClientPhoneNumber }))
                .ForMember(dest => dest.WorkflowStatus, opt => opt.MapFrom(x => new WorkflowStatusDto()
                { Name = x.WorkflowStatusName, DisplayName = x.WorkflowStatusDisplayName }))
                .ForMember(dest => dest.TaxIdentifier, opt => opt.MapFrom(x => new TaxIdentifierDto() { Value = x.TaxIdValue }))
                .ForMember(dest => dest.ClockInTime, opt => opt.MapFrom(x => x.ClockInTime.ToDateTimeOffset()))
                .ForMember(dest => dest.ClockOutTime, opt => opt.MapFrom(x => x.ClockOutTime.ToDateTimeOffset()))
                .ForMember(dest => dest.ServiceStartDateTime, opt => opt.MapFrom(x => x.ServiceStartDateTime.ToDateTimeOffset()))
                .ForMember(dest => dest.ServiceEndDateTime, opt => opt.MapFrom(x => x.ServiceEndDateTime.ToDateTimeOffset()));

            Mapper.CreateMap<ReasonsComments, ReasonsCommentsDto>();
            Mapper.CreateMap<VisitStaff, VisitStaffDto>()
                .ForMember(dest => dest.StaffName, opt => opt.MapFrom(x => new PersonNameDto()
                { FirstName = x.FirstName, LastName = x.LastName, MiddleName = x.MiddleName, Suffix = x.Suffix }))
                .ForMember(dest => dest.StaffPhone, opt => opt.MapFrom(x => new PhoneDto()
                { ExtensionNumber = x.ExtensionNumber, Number = x.Number }))
                .ForMember(dest => dest.Status, opt => opt.MapFrom(x => new WorkflowStatusDto()
                { Name = x.StatusName, DisplayName = x.StatusDisplayName }))
                .ForMember(dest => dest.StaffRejectLateReason, opt => opt.MapFrom(x => new ReasonsCommentsDto()
                {
                    Category = x.StaffRejectLateReasonCategory,
                    Content = x.StaffRejectLateReasonContent,
                    Key = x.StaffRejectLateReasonKey,
                    SubKey = x.StaffRejectLateReasonSubKey
                }));
            ;

            Mapper.CreateMap<ServiceVisitDto, ServiceVisit>()
                .ForMember(dest => dest.ClientFirstName, opt => opt.MapFrom(x => x.ClientName.FirstName))
                .ForMember(dest => dest.ClientLastName, opt => opt.MapFrom(x => x.ClientName.LastName))
                .ForMember(dest => dest.ClientMiddleName, opt => opt.MapFrom(x => x.ClientName.MiddleName))
                .ForMember(dest => dest.ClientNameSuffix, opt => opt.MapFrom(x => x.ClientName.Suffix))
                .ForMember(dest => dest.ClientPhoneExtNumber, opt => opt.MapFrom(x => x.ClientPhone.ExtensionNumber))
                .ForMember(dest => dest.ClientPhoneNumber, opt => opt.MapFrom(x => x.ClientPhone.Number))
                .ForMember(dest => dest.TaxIdValue, opt => opt.MapFrom(x => x.TaxIdentifier.Value))
                .ForMember(dest => dest.WorkflowStatusName, opt => opt.MapFrom(x => x.WorkflowStatus.Name))
                .ForMember(dest => dest.WorkflowStatusDisplayName, opt => opt.MapFrom(x => x.WorkflowStatus.DisplayName))
                .ForMember(dest => dest.ClientAcceptanceStatus, opt => opt.MapFrom(x => (int)x.ClientAcceptanceStatus))
                .ForMember(dest => dest.StaffAcceptanceStatus, opt => opt.MapFrom(x => (int)x.StaffAcceptanceStatus))
                .ForMember(dest => dest.ClockInTime, opt => opt.MapFrom(x => x.ClockInTime.ToDateTime()))
                .ForMember(dest => dest.ClockOutTime, opt => opt.MapFrom(x => x.ClockOutTime.ToDateTime()))
                .ForMember(dest => dest.ServiceStartDateTime, opt => opt.MapFrom(x => x.ServiceStartDateTime.ToDateTime()))
                .ForMember(dest => dest.ServiceEndDateTime, opt => opt.MapFrom(x => x.ServiceEndDateTime.ToDateTime()));

            Mapper.CreateMap<ReasonsCommentsDto, ReasonsComments>();
            Mapper.CreateMap<VisitStaffDto, VisitStaff>()
                .ForMember(dest => dest.FirstName, opt => opt.MapFrom(x => x.StaffName.FirstName))
                .ForMember(dest => dest.LastName, opt => opt.MapFrom(x => x.StaffName.LastName))
                .ForMember(dest => dest.MiddleName, opt => opt.MapFrom(x => x.StaffName.MiddleName))
                .ForMember(dest => dest.Suffix, opt => opt.MapFrom(x => x.StaffName.Suffix))
                .ForMember(dest => dest.ExtensionNumber, opt => opt.MapFrom(x => x.StaffPhone.ExtensionNumber))
                .ForMember(dest => dest.Number, opt => opt.MapFrom(x => x.StaffPhone.Number))
                .ForMember(dest => dest.StatusName, opt => opt.MapFrom(x => x.Status.Name))
                .ForMember(dest => dest.StatusDisplayName, opt => opt.MapFrom(x => x.Status.DisplayName))
                .ForMember(dest => dest.StaffRejectLateReasonCategory, opt => opt.MapFrom(x => x.StaffRejectLateReason.Category))
                .ForMember(dest => dest.StaffRejectLateReasonContent, opt => opt.MapFrom(x => x.StaffRejectLateReason.Content))
                .ForMember(dest => dest.StaffRejectLateReasonKey, opt => opt.MapFrom(x => x.StaffRejectLateReason.Key))
                .ForMember(dest => dest.StaffRejectLateReasonSubKey, opt => opt.MapFrom(x => x.StaffRejectLateReason.SubKey))
                ;

            //Map VisitTask & VisitTaskDto
            Mapper.CreateMap<VisitTask, VisitTaskDto>()
                 .ForMember(dest => dest.StartDateTime, opt => opt.MapFrom(x => x.StartDateTime.ToDateTime().ToDateTimeOffset()))//remove time zone part
                .ForMember(dest => dest.EndDateTime, opt => opt.MapFrom(x => x.EndDateTime.ToDateTime().ToDateTimeOffset()));//remove time zone part
                
            Mapper.CreateMap<VisitTaskDto, VisitTask>()
                 .ForMember(dest => dest.StartDateTime, opt => opt.MapFrom(x => x.StartDateTime.ToDateTime().ToDateTimeOffset()))//remove time zone part
                .ForMember(dest => dest.EndDateTime, opt => opt.MapFrom(x => x.EndDateTime.ToDateTime().ToDateTimeOffset()));//remove time zone part

            Mapper.CreateMap<EvvMobile.ViewModels.Clients.ClientProfileViewModel, MobileClientProfileDto>();
            Mapper.CreateMap<MobileClientProfileDto, EvvMobile.ViewModels.Clients.ClientProfileViewModel>()
               .ForMember(dest => dest.ClientFullName, opt => opt.MapFrom(x => x.ClientName.FullName))
               .ForMember(dest => dest.Age, opt => opt.Ignore())
               .ForMember(dest => dest.Gender, opt => opt.MapFrom(x => (x.Gender == null) ? "" : x.Gender.Name))
               .ForMember(dest => dest.CurrentEmergencyContact, opt => opt.MapFrom(x => x.CurrentEmergencyContact.FullName))
               .ForMember(dest => dest.CurrentEmergencyContactPhoneNumber, opt => opt.MapFrom(x => x.CurrentEmergencyContactPhoneNumber.Number + "/" + x.CurrentEmergencyContactPhoneNumber.ExtensionNumber))
               .ForMember(dest => dest.CurrentEmergencyContactRelation, opt => opt.MapFrom(x => x.CurrentEmergencyContactRelation.Name))
               .ForMember(dest => dest.MaritalStatus, opt => opt.MapFrom(x => (x.MaritalStatus == null) ? "" : x.MaritalStatus.Name))
               .ForMember(dest => dest.PhoneNumber, opt => opt.MapFrom(x => x.PhoneNumber.Number + "/" + x.PhoneNumber.ExtensionNumber))
               .ForMember(dest => dest.Race, opt => opt.MapFrom(x => (x.Race == null) ? "" : x.Race.Name))
               .ForMember(dest => dest.SecondEmergencyContact, opt => opt.MapFrom(x => x.SecondEmergencyContact.FullName))
               .ForMember(dest => dest.SecondEmergencyContactPhoneNumber, opt => opt.MapFrom(x => x.SecondEmergencyContactPhoneNumber.Number + "/" + x.SecondEmergencyContactPhoneNumber.ExtensionNumber))
               .ForMember(dest => dest.SecondEmergencyContactRelation, opt => opt.MapFrom(x => x.SecondEmergencyContactRelation.Name))
               .ForMember(dest => dest.Comment, opt => opt.Ignore())
               .ForMember(dest => dest.CurrentCasemanagerOrServiceCoordinatorContact, opt => opt.Ignore())
               .ForMember(dest => dest.CurrentDurablePowerOfAttorneyContact, opt => opt.Ignore())
               .ForMember(dest => dest.CurrentGuardianOfPerson, opt => opt.Ignore())
               .ForMember(dest => dest.CurrentPhysician, opt => opt.Ignore())
               .ForMember(dest => dest.CurrentPowerOfAttorneyContact, opt => opt.Ignore())
               .ForMember(dest => dest.CurrentRepresentativePayee, opt => opt.Ignore())
               .ForMember(dest => dest.CurrentSurrogate, opt => opt.Ignore())
               .ForMember(dest => dest.FacilityAddress, opt => opt.Ignore())
               .ForMember(dest => dest.FacilityName, opt => opt.Ignore())
               .ForMember(dest => dest.Icon, opt => opt.Ignore())
               .ForMember(dest => dest.IsBusy, opt => opt.Ignore())
               .ForMember(dest => dest.IsInitialized, opt => opt.Ignore())
               .ForMember(dest => dest.IsLivingWithFamily, opt => opt.Ignore())
               .ForMember(dest => dest.LivingStatus, opt => opt.Ignore())
               .ForMember(dest => dest.Navigation, opt => opt.Ignore())
               .ForMember(dest => dest.OtherRaceSpecify, opt => opt.Ignore())
               .ForMember(dest => dest.PrimaryLanguage, opt => opt.Ignore())
               .ForMember(dest => dest.Subtitle, opt => opt.Ignore())
               .ForMember(dest => dest.Title, opt => opt.Ignore());

            Mapper.CreateMap<VisitMeasurementDto, VisitMeasurement>();
               
            Mapper.CreateMap<VisitMeasurement, VisitMeasurementDto>()
                .ForMember(dest => dest.Attributes, opt => opt.Ignore());
        }
    }
}
