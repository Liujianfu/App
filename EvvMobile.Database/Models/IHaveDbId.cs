﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EvvMobile.Database.Models
{
    public interface IHaveDbId
    {
        int DbId { get; set; }
    }
}
