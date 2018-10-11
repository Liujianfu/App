using System;
using SQLite;

namespace EvvMobile.Database.Models
{
    public class Location
    {
        [PrimaryKey, AutoIncrement]
        public int Id { get; set; }
        public int LocationTrackingId { get; set; }
        public double Latitude { get; set; }
        public double Longitude { get; set; }

        public double Accuracy { get; set; }
        public DateTimeOffset Timestamp { get; set; }
    }
}
