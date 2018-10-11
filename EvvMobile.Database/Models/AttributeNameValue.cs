using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SQLite;

namespace EvvMobile.Database.Models
{
    public class AttributeNameValue : IHaveServiceVisitId, IHaveDbId
    {
        [PrimaryKey, AutoIncrement]
        public int DbId { get; set; }
        public string AttributeName { get; set; }
        public string AttributeValue { get; set; }
        public string ServiceVisitId { get; set; }
        public int VisitMeasurementDbId { get; set; }
    }
}
