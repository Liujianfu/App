using EvvMobile.Database.Models;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EvvMobile.Database.Repositories
{
    public class BaseServiceVisitObjectRepository<T> : BaseRepository where T: class, IHaveServiceVisitId, IHaveDbId, new()
    {
        //This field must be overwrite by subClass.

        public BaseServiceVisitObjectRepository(string tableName)
        {
            this.TableName = tableName;
        }

        private readonly string TableName;
        public Task<List<T>> GetAllListAsync()
        {
            return AsyncConnection.Table<T>().ToListAsync();
        }
        public List<T> GetAllListSync()
        {
            return AsyncConnection.Table<T>().ToListAsync().Result;
        }
        public Task<List<T>> GetListByServiceVisitIdAsync(string serviceVisitId)
        {
            return AsyncConnection.Table<T>().Where(
                i => i.ServiceVisitId == serviceVisitId).ToListAsync();
        }
        public List<T> GetListByServiceVisitIdSync(string serviceVisitId)
        {
            return AsyncConnection.Table<T>().Where(
                i => i.ServiceVisitId == serviceVisitId).ToListAsync().Result;
        }
        public Task<T> GetObjectByDbIdAsync(int dbId)
        {
            return AsyncConnection.Table<T>().Where(
                i => i.DbId == dbId).FirstOrDefaultAsync();
        }

        public Task<int> DeleteObjectAsync(T deleteObject)
        {
            return AsyncConnection.DeleteAsync(deleteObject);
        }

        public Task<int> DeleteObjectByIdAsync(string id)
        {
            var sql = $"DELETE FROM [{TableName}] WHERE [Id] = '{id}'";
            return AsyncConnection.ExecuteAsync(sql);
        }

        public Task<int> DeleteObjectsByServiceVisitIdAsync(string serviceVisitId)
        {
            var sql = $"DELETE FROM [{TableName}] WHERE [ServiceVisitId] = '{serviceVisitId}'";
            return AsyncConnection.ExecuteAsync(sql);
        }

        public Task<int> InsertObjectAsync(T saveObject)
        {
            return AsyncConnection.InsertAsync(saveObject);
        }

        public Task<int> SaveObjectAsync(T saveObject)
        {
            if (saveObject.DbId != 0)
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
            var sql = $"DELETE FROM [{TableName}] ";
            return AsyncConnection.ExecuteAsync(sql);
        }

    }
}
