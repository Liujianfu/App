using System;
using SQLite;

namespace EvvMobile.Database.Models
{
    public class Claim
    {
        [PrimaryKey, AutoIncrement]
        public int Id { get; set; }

        public int ClockInId { get; set; }

        public int ClockOutId { get; set; }

        public string ClientMaNumber { get; set; }

        public string ClientFullName { get; set; }

        public string ProcedureCode { get; set; }

        public string ServiceName { get; set; }

        public string ProviderNumber { get; set; }

        public string StaffId { get; set; }
        public string Status { get; set; }
        public DateTimeOffset ClockInTime { get; set; }
        public DateTimeOffset ClockOutTime { get; set; }
        public int ScheduleId { get; set; }
    }
}
