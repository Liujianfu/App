using EvvMobile.Database.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using EvvMobile.Database.Common;

namespace EvvMobile.Database.Repositories
{
    public class AttributeNameValueRepository : BaseServiceVisitObjectRepository<AttributeNameValue>
    {
        public AttributeNameValueRepository() : base(DbConstants.AttributeNameValue)
        {
        }

        public IList<AttributeNameValue> GetListByVisitMeasurementIdAsync(IList<AttributeNameValue> allAttributes, int MeasurementId)
        {
            return allAttributes.Where(x => x.VisitMeasurementDbId == MeasurementId).ToList();
        }
    }
}
