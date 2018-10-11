using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SQLite;

namespace EvvMobile.Database.Models
{
    public class VisitMeasurement: IHaveServiceVisitId, IHaveDbId
    {
        [PrimaryKey, AutoIncrement]
        public int DbId { get; set; }

        public string ServiceVisitId { get; set; }
        public string Code { get; set; }
        public string Instruction { get; set; }
        public string Name { get; set; }
    }
}
