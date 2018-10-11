using System.Collections.Generic;
using System.Threading.Tasks;
using EvvMobile.Database.Models;
using SQLite;

namespace EvvMobile.Database.Repositories
{
    public class VisitStaffRepository : BaseRepository
    {
        public Task<List<VisitStaff>> GetAllListAsync()
        {
            return AsyncConnection.Table<VisitStaff>().ToListAsync();
        }
        public List<VisitStaff> GetAllListSync()
        {
            return AsyncConnection.Table<VisitStaff>().ToListAsync().Result;
        }
        public Task<List<VisitStaff>> GetListByServiceVisitIdAsync(string serviceVisitId)
        {
            return AsyncConnection.Table<VisitStaff>().Where(
                i => i.ServiceVisitId == serviceVisitId).ToListAsync();
        }
        public List<VisitStaff> GetListByServiceVisitIdSync(string serviceVisitId)
        {
            return AsyncConnection.Table<VisitStaff>().Where(
                i => i.ServiceVisitId == serviceVisitId).ToListAsync().Result;
        }
        public Task<VisitStaff> GetObjectByDbIdAsync(int dbId)
        {
            return AsyncConnection.Table<VisitStaff>().Where(
                i => i.DbId == dbId).FirstOrDefaultAsync();
        }

        public Task<int> DeleteObjectAsync(VisitStaff deleteObject)
        {
            return AsyncConnection.DeleteAsync(deleteObject);
        }

        public Task<int> DeleteObjectByDbIdAsync(int dbId)
        {
            var sql = string.Format("DELETE FROM [VisitStaff] WHERE [DbId] = {0}", dbId);
            return AsyncConnection.ExecuteAsync(sql);
        }

        public Task<int> DeleteObjectsByServiceVisitIdAsync(string serviceVisitId)
        {
            var sql = string.Format("DELETE FROM [VisitStaff] WHERE [ServiceVisitId] = '{0}'", serviceVisitId);
            return AsyncConnection.ExecuteAsync(sql);
        }

        public Task<int> InsertObjectAsync(VisitStaff saveObject)
        {
           return AsyncConnection.InsertAsync(saveObject);
        }

        public Task<int> SaveObjectAsync(VisitStaff saveObject)
        {
            if (saveObject.DbId!=0)
            {
                return AsyncConnection.UpdateAsync(saveObject);
            }
            else
            {
                return AsyncConnection.InsertAsync(saveObject);
            }
        }

        public Task<int> DeleteAllAsync()
        {
            var sql = string.Format("DELETE FROM [VisitStaff] ");
            return AsyncConnection.ExecuteAsync(sql);
        }

    }
}
