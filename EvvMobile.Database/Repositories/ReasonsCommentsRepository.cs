using System.Collections.Generic;
using System.Threading.Tasks;
using EvvMobile.Database.Models;
using SQLite;

namespace EvvMobile.Database.Repositories
{
    public class ReasonsCommentsRepository : BaseRepository
    {
        public Task<List<ReasonsComments>> GetAllListAsync()
        {
            return AsyncConnection.Table<ReasonsComments>().ToListAsync();
        }

        public Task<List<ReasonsComments>> GetListByServiceVisitIdAsync(string serviceVisitId)
        {
            return AsyncConnection.Table<ReasonsComments>().Where(
                i => i.ServiceVisitId == serviceVisitId).ToListAsync();
        }

        public Task<ReasonsComments> GetObjectByDbIdAsync(int dbId)
        {
            return AsyncConnection.Table<ReasonsComments>().Where(
                i => i.DbId == dbId).FirstOrDefaultAsync();
        }

        public Task<int> DeleteObjectAsync(ReasonsComments deleteObject)
        {
            return AsyncConnection.DeleteAsync(deleteObject);
        }

        public Task<int> DeleteObjectByDbIdAsync(int dbId)
        {
            var sql = string.Format("DELETE FROM [ReasonsComments] WHERE [DbId] = {0}", dbId);
            return AsyncConnection.ExecuteAsync(sql);
        }

        public Task<int> DeleteObjectsByServiceVisitIdAsync(string serviceVisitId)
        {
            var sql = string.Format("DELETE FROM [ReasonsComments] WHERE [ServiceVisitId] = '{0}'", serviceVisitId);
            return AsyncConnection.ExecuteAsync(sql);
        }

        public Task<int> InsertObjectAsync(ReasonsComments saveObject)
        {
           return AsyncConnection.InsertAsync(saveObject);
        }

        public Task<int> SaveObjectAsync(ReasonsComments saveObject)
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
            var sql = string.Format("DELETE FROM [ReasonsComments] ");
            return AsyncConnection.ExecuteAsync(sql);
        }

    }
}
