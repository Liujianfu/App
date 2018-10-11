using System;
using SQLite;

namespace EvvMobile.Database.Models
{
    public class ReasonsComments
    {
        [PrimaryKey, AutoIncrement]
        public int DbId { get; set; }
        public string ServiceVisitId { get; set; }
        public string Category { get;  set; }
        public string Key { get;  set; }
        public string SubKey { get;  set; }
        public string Content { get;  set; }
        public DateTime NoteTime { get; set; }
        public string Name { get; set; }
    }
}
