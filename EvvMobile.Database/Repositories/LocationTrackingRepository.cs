using System.Collections.Generic;
using System.Threading.Tasks;
using EvvMobile.Database.Models;

namespace EvvMobile.Database.Repositories
{
    public class LocationTrackingRepository : BaseRepository
    {
        public Task<List<LocationTracking>> GetLocationTrackingsAsync()
        {
            return AsyncConnection.Table<LocationTracking>().ToListAsync();
        }

        public Task<LocationTracking> GetLocationTrackingAsync(int id)
        {
            return AsyncConnection.Table<LocationTracking>().Where(i => i.Id == id).FirstOrDefaultAsync();
        }

        public Task<int> SaveLocationTrackingAsync(LocationTracking locationTracking)
        {
            if (locationTracking.Id != 0)
            {
                return AsyncConnection.UpdateAsync(locationTracking);
            }
            else
            {
                return AsyncConnection.InsertAsync(locationTracking);
            }
        }

        public Task<int> DeleteLocationTrackingAsync(LocationTracking locationTracking)
        {
            return AsyncConnection.DeleteAsync(locationTracking);
        }
        public Task<int> DeleteLocationTrackingAsync(int id)
        {
            var sql = string.Format("DELETE FROM [LocationTracking] WHERE [Id] = {0}", id);
            return AsyncConnection.ExecuteAsync(sql);
        }
    }
}
