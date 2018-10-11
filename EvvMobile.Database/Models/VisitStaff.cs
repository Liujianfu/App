using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SQLite;

namespace EvvMobile.Database.Models
{
    public class VisitStaff
    {
        [PrimaryKey, AutoIncrement]
        public int DbId { get; set; }

        public string ServiceVisitId { get; set; }
        public string StaffId { get; set; }
        public string StaffIdentifier { get; set; }
        public bool IsPrimaryStaff { get; set; }
        public string BusinessTitle { get; set; }
        public DateTimeOffset AssignmentDate { get; set; }
        public string StatusReason { get; set; }

        //PersonName in the field
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string MiddleName { get; set; }
        public string Suffix { get; set; }

        //Phone in the field
        public virtual string ExtensionNumber { get; set; }
        public virtual string Number { get; set; }

        //Status
        public string StatusName { get; set; }
        public string StatusDisplayName { get; set; }

        //StaffRejectReason
        public string StaffRejectLateReasonCategory { get; set; }
        public string StaffRejectLateReasonContent { get; set; }
        public string StaffRejectLateReasonKey { get; set; }
        public string StaffRejectLateReasonSubKey { get; set; }


    }
}
