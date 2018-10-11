using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EvvMobile.Database.Common
{
    public class DbConstants
    {
        public static readonly string NewVisitIdPrefix = "createdbymobile";

        #region TableNames

        public static readonly string VisitTask = "VisitTask";
        public static readonly string VisitMeasurement = "VisitMeasurement";
        public static readonly string AttributeNameValue = "AttributeNameValue";
        public static readonly string VisitStaff = "VisitStaff";

        #endregion
    }
}
