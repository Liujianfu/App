using SQLite;
using System;

namespace EvvMobile.Database.Models
{
    public class VisitTask: IHaveServiceVisitId, IHaveDbId
    {
        #region Properties
        [PrimaryKey, AutoIncrement]
        public int DbId { get; set; }
        public bool IsPrimaryStaff { get; set; }
        public string ServiceVisitId { get; set; }
        public string Code { get; private set; }
        public string Catetory { get; private set; }
        public string Instruction { get; private set; }
        public string TaskName { get; private set; }
        public string Description { get; private set; }
        public bool IsScheduled { get; private set; }
        public string Comment { get; set; }
        public string TaskResult { get; set; }
        public DateTimeOffset? StartDateTime { get; set; }
        public DateTimeOffset? EndDateTime { get; set; }


        #endregion
    }
}
