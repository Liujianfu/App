using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SQLite;

namespace EvvMobile.Database.Models
{
    public class AttachmentItem  
    {
        [PrimaryKey, AutoIncrement]
        public int DbId { get; set; }
        public string Key { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public string Directory { get; set; }
        public long Size { get; set; }
        public string MineType { get; set; }
        public DateTimeOffset LastModified { get; set; }
        public DateTimeOffset CreationDate { get; set; }
    }
}
