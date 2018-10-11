using SQLite;
using System;

namespace EvvMobile.Database.Common
{
    public class EvvDatabase
    {
        #region Initialize
        /// <summary>
        /// Initlize db like: create connection and create table etc.
        /// This methods should be explicit called for EvvDatabase.Instance
        /// </summary>
        /// <param name="path"></param>
        /// <param name="initAction"></param>
        public void Initialize(string path, Action<SQLiteAsyncConnection> initAction)
        {
            connection = new SQLiteAsyncConnection(path);
            initAction(connection);
        } 
        #endregion

        #region EvvDatabase Instance

        public static EvvDatabase Instance
        {
            get
            {
                if (evvDB == null)
                {
                    lock (locker)
                    {
                        if (evvDB == null)
                        {
                            evvDB = new EvvDatabase();
                        }
                    }
                }

                return evvDB;
            }
        }

        #endregion

        #region Connection
        public SQLiteAsyncConnection Connection {
            get {
                if (connection == null)
                {
                    throw new MemberAccessException("The connection is null,you must initilize it before using.");
                }
                return connection;
            }
        } 
        #endregion

        #region fileds

        private  SQLiteAsyncConnection connection;

        private static object locker = new object();

        private static EvvDatabase evvDB; 

        #endregion
    }
}
