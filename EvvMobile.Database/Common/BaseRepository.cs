using SQLite;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace EvvMobile.Database.Common
{
    public class BaseRepository<T> where T:IEntity,new ()
    {
        public BaseRepository(SQLiteAsyncConnection connection)
        {
            this.connection = connection;
        }

        #region CRUD

        public Task<List<T>> GetAllAsync()
        {
            return connection.Table<T>().ToListAsync();
        }

        /// <summary>
        /// throw exctpion if object is not exist in db
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public Task<T> GetAsync(int id)
        {
            return connection.GetAsync<T>(id);
        }

        public Task<int> SaveAsync(T item)
        {
            if (item.Id != 0)
            {
                return connection.UpdateAsync(item);
            }
            else
            {
                return connection.InsertAsync(item);
            }
        }

        public Task<int> DeleteAsync(int id)
        {
            var item = new T() { Id = id };
            return connection.DeleteAsync(item);
        }
        /// <summary>
        /// return null if not find object by pk in db
        /// </summary>
        /// <param name="pk"></param>
        /// <returns></returns>
        public Task<T> FindAsync(object pk)
        {
            return connection.FindAsync<T>(pk);
        }

        #endregion

        #region connection
        protected readonly SQLiteAsyncConnection connection; 
        #endregion
    }
}
