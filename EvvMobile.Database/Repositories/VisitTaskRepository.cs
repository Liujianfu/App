using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using EvvMobile.Database.Models;
using EvvMobile.Database.Common;

namespace EvvMobile.Database.Repositories
{
    public class VisitTaskRepository : BaseServiceVisitObjectRepository<VisitTask>
    {
        public VisitTaskRepository() : base(DbConstants.VisitTask)
        {
        }
    }
}
