using System.Collections.Generic;
using EvvMobile.Database.Common;
using SQLite;

namespace EvvMobile.Database.Repositories
{
    public class BaseRepository
    {
        public BaseRepository()
        {
            ErrMessages = new List<string>() { };
        }

        public SQLiteAsyncConnection AsyncConnection
        {
            get
            {
                return EvvDatabase.Instance.Connection;
            }
        }

        public List<string> ErrMessages { get; set; }

    }
}
