using System;
using Evv.Message.Portable.Schedulers.Dtos;
using SQLite;

namespace EvvMobile.Database.Models
{
    public class LocationTracking
    {
        [PrimaryKey, AutoIncrement]
        public int Id { get; set; }
        public string ClientName { get; set; }
        public DateTimeOffset StartTime { get; set; }

        public DateTimeOffset EndTime { get; set; }

        public int TotalNumberOfLocations { get; set; }

        public double Distance { get; set; }
    }
}
