using System;
using System.Collections.Generic;
using System.Diagnostics.Contracts;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EvvMobile.Database.Models
{
    public interface IHaveServiceVisitId
    {
        string ServiceVisitId { get; set; }
    }
}
