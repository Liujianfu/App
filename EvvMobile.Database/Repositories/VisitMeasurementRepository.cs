using EvvMobile.Database.Models;
using EvvMobile.Database.Common;

namespace EvvMobile.Database.Repositories
{
    public class VisitMeasurementRepository : BaseServiceVisitObjectRepository<VisitMeasurement>
    {
        public VisitMeasurementRepository() : base(DbConstants.VisitMeasurement)
        {
        }
    }
}
