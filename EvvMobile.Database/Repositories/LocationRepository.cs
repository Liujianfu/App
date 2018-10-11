using System.Collections.Generic;
using System.Threading.Tasks;
using EvvMobile.Database.Models;

namespace EvvMobile.Database.Repositories
{
    public class LocationRepository : BaseRepository
    {
        public Task<List<Location>> GetLocationsAsync()
        {
            return AsyncConnection.Table<Location>().ToListAsync();
        }

        public Task<Location> GetLocationAsync(long id)
        {
            return AsyncConnection.Table<Location>().Where(i => i.Id == id).FirstOrDefaultAsync();
        }

        public Task<List<Location>> GetLocationsAsync(int locationTrackingId)
        {
            return AsyncConnection.Table<Location>().Where(i => i.LocationTrackingId == locationTrackingId).ToListAsync();
        }

        public Task<int> DeleteLocationsAsync(int locationTrackingId)
        {
            var sql = string.Format("DELETE FROM [Location] WHERE [LocationTrackingId] = {0}", locationTrackingId);
            return AsyncConnection.ExecuteAsync(sql);
        }

        public Task<int> DeleteLocation(Location location)
        {
            return AsyncConnection.DeleteAsync(location);
        }

        public Task<int> InsertLocation(Location location)
        {
            return AsyncConnection.InsertAsync(location);
        }

        public Task<int> InsertLocations(List<Location> locations)
        {
            return AsyncConnection.InsertAllAsync(locations);
        }
    }
}
