using SQLite;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EvvMobile.Database.Common
{
    public class Entity:IEntity
    {
        [PrimaryKey, AutoIncrement]
        public int Id { get; set; }
    }
}
