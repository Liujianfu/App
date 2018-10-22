using SQLite;
using System;

namespace CareVisitProviderApp.Models
{
    public class ServiceVisit
    {
        [PrimaryKey, AutoIncrement]
        public int DbId { get; set; }

        public string Id { get; set; }

        #region Sync Info
        /// <summary>
        /// The value is true if save data on offline
        /// </summary>
        public bool IsUnsynced { get; set; }

        public string ActionType { get; set; }


        #endregion

        #region Provider Info

        public string OrganizationUnitId { get; set; }

        public string ProviderId { get; set; }

        public string ProviderName { get; set; }

        public string ProviderMaNumber { get; set; }

        public string TaxIdentifier { get; set; }

        #endregion

        #region Client Info

        public string ClientId { get; set; }

        public string ClientFirstName { get; set; }

        public string ClientLastName { get; set; }

        public string ClientMiddleName { get; set; }

        public string ClientNameSuffix { get; set; }

        public string ClientIdentifier { get; set; }

        public string ClientMaNumber { get; set; }

        public string ClientPhoneNumber { get; set; }

        public string ClientPhoneExtNumber { get; set; }

        public string PrimaryDiagnosisCode { get; set; }

        public string SecondaryDiagnosisCode { get; set; }

        #endregion

        #region Visit Info

        public DateTime? ServiceStartDateTime { get; set; }

        public DateTime? ServiceEndDateTime { get; set; }

        public string ServiceRenderAddress { get; set; }

        public string WaiverTypeName { get; set; }

        public string WaiverTypeId { get; set; }

        public string ServiceName { get; set; }  

        public string ProcedureCodeAndModifier { get; set; }

        public string NoteToCareGiver { get; set; }

        public string NoteToClient { get; set; }
        public string CurrentStaffId { get; set; }

        public bool IsUnscheduled { get; set; }
        #endregion

        #region MMIS Granted

        public string MmisAppovalStatus { get; set; }

        #endregion

        #region Clock Out/In 

        public DateTime? ClockInTime { get; set; } 
        public DateTime? ClockOutTime { get; set; }

        #region ClockIn/Out locations
        public double ClockInLatitude { get; set; }
        public double ClockInLongitude { get; set; }
        public string ClockInAddress { get; set; }
        public double ClockOutLatitude { get; set; }
        public double ClockOutLongitude { get; set; }
        public string ClockOutAddress { get; set; }
        public string ClientSignatureBase64Img { get; set; }
        #endregion
        #endregion

        #region Staff Acceptence Info
        public int StaffAcceptanceStatus { get; set; }
        #endregion

        #region Client Acceptence Info
        public int ClientAcceptanceStatus { get; set; }
        #endregion

        public long UniqueVisitId { get; set; }

        public bool Overlapped { get; set; }

        public bool AutoAccepted { get; set; }

        public int HoursOfAutoAccepted { get; set; }

        #region LateSettings
        /// <summary>
        /// Minutes, how many minutes early/ late than scheduled time will be treated as too early / too late. 
        /// user will be asked to fill the early/late reason
        /// </summary>
        public int LateOrEralyThreshold { get; set; }
        public bool IsVisitLate { get; set; }

        #endregion

        #region Location

        public double LocationLatitude { get; set; }

        public double LocationLongitude { get; set; }

        public bool LocationMatched { get; set; }

        #endregion

        #region IWorkflowAware

        public string WorkflowStatusName { get; set; }

        public string WorkflowStatusDisplayName { get; set; }

        #endregion
    }
}
