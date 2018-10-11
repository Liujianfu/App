using System.Collections.Generic;
using System.Threading.Tasks;
using EvvMobile.Database.Models;
using SQLite;

namespace EvvMobile.Database.Repositories
{
    public class ClaimRepository : BaseRepository
    {
        #region CRUD Action
        public Task<List<Claim>> GetClaimsAsync()
        {
            return AsyncConnection.Table<Claim>().ToListAsync();
        }

        public Task<Claim> GetClaimAsync(int id)
        {
            return AsyncConnection.Table<Claim>().Where(i => i.Id == id).FirstOrDefaultAsync();
        }

        public Task<int> DeleteClaimAsync(Claim claim)
        {
            return AsyncConnection.DeleteAsync(claim);
        }

        public Task<int> DeleteClaimAsync(int id)
        {
            var sql = string.Format("DELETE FROM [Claim] WHERE [Id] = {0}", id);
            return AsyncConnection.ExecuteAsync(sql);
        }

        public Task<int> SaveClaimAsync(Claim claim)
        {
            if (claim.Id != 0)
            {
                return AsyncConnection.UpdateAsync(claim);
            }
            else
            {
                return AsyncConnection.InsertAsync(claim);
            }
        }

        public Task<int> DeleteClaimAsync()
        {
            var sql = string.Format("DELETE FROM [Claim] ");
            return AsyncConnection.ExecuteAsync(sql);
        }
        #endregion

        #region Transaction Action
        public int InsertClaimList(List<Claim> claims)
        {
            int result = 0;
            SQLiteConnectionWithLock conn = AsyncConnection.GetConnection();
            try
            {
                conn.BeginTransaction();

                result = conn.InsertAll(claims);

                conn.Commit();
            }
            catch
            {
                conn.Rollback();
                ErrMessages.Add("Database operation error.");
            }

            return result;
        }
        #endregion
    }
}
