using System.Collections.Generic;
using System.Threading.Tasks;
using EvvMobile.Database.Models;
using SQLite;
using Evv.Message.Portable.Schedulers.Dtos;
using AutoMapper;
using System.Linq;
using System.Diagnostics;
using System;
using System.Text;
using EvvMobile.Database.Common;

namespace EvvMobile.Database.Repositories
{
    public class ServiceVisitRepository : BaseRepository
    {

        public Task<List<ServiceVisit>> GetAllListAsync()
        {           
            return AsyncConnection.Table<ServiceVisit>().ToListAsync();
        }
        public List<ServiceVisit> GetAllListSync()
        {
            return AsyncConnection.Table<ServiceVisit>().ToListAsync().Result;
        }
        public Task<ServiceVisit> GetObjectByIdAsync(string id)
        {
            return AsyncConnection.Table<ServiceVisit>().Where(i => i.Id == id).FirstOrDefaultAsync();
        }
        public ServiceVisit GetObjectByIdSync(string id)
        {
            return AsyncConnection.Table<ServiceVisit>().Where(i => i.Id == id).FirstOrDefaultAsync().Result;
        }
        public Task<int> DeleteObjectAsync(ServiceVisit deleteObject)
        {
            return AsyncConnection.DeleteAsync(deleteObject);
        }
        public int DeleteObjectSync(ServiceVisit deleteObject)
        {
            return AsyncConnection.DeleteAsync(deleteObject).Result;
        }
        public Task<int> DeleteObjectByIdAsync(string id)
        {
            var sql = string.Format("DELETE FROM [ServiceVisit] WHERE [Id] = '{0}'", id);
            return AsyncConnection.ExecuteAsync(sql);
        }

        public Task<int> UpdateIsUnsyncedByIdAsync(string id)
        {   
            var sql = string.Format("update [ServiceVisit] set [IsUnsynced] = 0 WHERE [Id] = '{0}'", id);
            return AsyncConnection.ExecuteAsync(sql);
        }

        public Task<int> SaveObjectAsync(ServiceVisit saveObject)
        {
            if (saveObject.Id!=null)
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
            var sql = string.Format("DELETE FROM [ServiceVisit] ");
            return AsyncConnection.ExecuteAsync(sql);
        }

        #region Transaction Action
        public int UpdateWithComments(ServiceVisit serviceVisit, 
            ReasonsComments reasonsComment)
        {
            int result = 0;
            SQLiteConnectionWithLock conn = AsyncConnection.GetConnection();
            try
            {
                conn.BeginTransaction();

                result = conn.Update(serviceVisit);

                var sql = string.Format("DELETE FROM [ReasonsComments] WHERE [ServiceVisitId] = '{0}' AND [SubKey] = '{1}'",
                    serviceVisit.Id, reasonsComment.SubKey);
                conn.Execute(sql);

                conn.Insert(reasonsComment);

                conn.Commit();
            }
            catch (Exception ex)
            {
                result = 0;
                conn.Rollback();
                ErrMessages.Add("Database operation error.");
                Debug.WriteLine(ex);
            }

            return result;
        }


        public int InsertServiceVisitList(IList<ServiceVisitDto> serviceVisits)
        {
            int result = 0;
            SQLiteConnectionWithLock conn = AsyncConnection.GetConnection();
            try
            {
                conn.BeginTransaction();

                string unsyncedIds = ClearLocalDataReturnUnsyncedIds(conn);

                foreach (ServiceVisitDto visitDto in serviceVisits)
                {
                    if (!unsyncedIds.Contains(visitDto.Id))
                    {
                        var visit = Mapper.Map<ServiceVisitDto, ServiceVisit>(visitDto);
                        conn.Insert(visit);

                        if (visitDto.VisitStaffs!=null && visitDto.VisitStaffs.Any())
                        {
                            var visitStaffs = Mapper.Map<IList<VisitStaffDto>, IList<VisitStaff>>(visitDto.VisitStaffs);
                            conn.InsertAll(visitStaffs);
                        }

                        if (visitDto.VisitTasks!=null && visitDto.VisitTasks.Any())
                        {
                            var visitTasks = Mapper.Map<IList<VisitTaskDto>, IList<VisitTask>>(visitDto.VisitTasks);
                            conn.InsertAll(visitTasks);
                        }

                        if (visitDto.VisitMeasurements!=null && visitDto.VisitMeasurements.Any())
                        {
                            var visitMeasurement = Mapper.Map<IList<VisitMeasurementDto>,IList<
                            VisitMeasurement>> (visitDto.VisitMeasurements);

                            conn.InsertAll(visitMeasurement);

                            for (int index = 0; index < visitMeasurement.Count; index++)
                            {
                                var measurement = visitMeasurement[index];
                                var measurementDto = visitDto.VisitMeasurements[index];

                                var attributes = measurementDto.Attributes.Select(x => new AttributeNameValue()
                                {
                                    AttributeName = x.AttributeName,
                                    AttributeValue = x.AttributeValue,
                                    ServiceVisitId = visitDto.Id,
                                    VisitMeasurementDbId = measurement.DbId
                                });

                                conn.InsertAll(attributes);
                            }
                        }
                    }
                }
                conn.Commit();
                result = 1;
            }
            catch (Exception ex)
            {
                conn.Rollback();
                ErrMessages.Add("Database operation error.");
                Debug.WriteLine(ex);
            }

            return result;
        }

        public string ClearLocalDataReturnUnsyncedIds(SQLiteConnectionWithLock conn)
        {
            string ids = "";
            IList<ServiceVisit> list = conn.Table<ServiceVisit>().ToList();
            foreach (var obj in list)
            {
                if(obj.IsUnsynced)
                {
                    ids +=",'" + obj.Id+"'";
                }
                else
                {
                    conn.Delete(obj);
                }
            }

            if(ids.Length>0)
            {
                ids = ids.Substring(1);
                StringBuilder sbSql = new StringBuilder();
                sbSql.Append("DELETE FROM [VisitStaff] where [ServiceVisitId] not in (" + ids + ") ; ");
                sbSql.Append("DELETE FROM [VisitTask] where [ServiceVisitId] not in (" + ids + ") ; ");
                //sbSql.Append("DELETE FROM [VisitMeasurement] where [ServiceVisitId] not in (" + ids + ") ; ");
                //sbSql.Append("DELETE FROM [AttributeNameValue] where [ServiceVisitId] not in (" + ids + ") ; ");
                conn.Execute(sbSql.ToString());
            }

            return ids;
        }

        public int ReplaceServiceVisit(ServiceVisit serviceVisit, IList<VisitStaff> visitStaffs, IList<VisitTask> visitTasks)
        {
            int result = 0;
            SQLiteConnectionWithLock conn = AsyncConnection.GetConnection();
            try
            {
                conn.BeginTransaction();
                var deleteVisitSubTablesSql = " DELETE FROM [{0}] where ServiceVisitId='{1}'; ";
                var sql = string.Format(" DELETE FROM [ServiceVisit] where Id = '{0}'; ", serviceVisit.Id);
                sql += string.Format(" DELETE FROM [VisitStaff] where ServiceVisitId='{0}'; ", serviceVisit.Id);
                sql += string.Format(deleteVisitSubTablesSql, DbConstants.VisitTask, serviceVisit.Id);
                //sql += string.Format(deleteVisitSubTablesSql, DbConstants.VisitMeasurement, serviceVisit.Id);
                //sql += string.Format(deleteVisitSubTablesSql, DbConstants.AttributeNameValue, serviceVisit.Id);

                conn.Execute(sql);
                conn.Insert(serviceVisit);
                
                if (visitStaffs!=null&&visitStaffs.Any())
                {
                    foreach (var visitStaff in visitStaffs)
                    {
                        visitStaff.ServiceVisitId = serviceVisit.Id;
                    }

                    conn.InsertAll(visitStaffs);
                }

                if (visitTasks!=null&&visitTasks.Any())
                {
                    foreach (var visitTask in visitTasks)
                    {
                        visitTask.ServiceVisitId = serviceVisit.Id;
                    }

                    conn.InsertAll(visitTasks);
                }

                conn.Commit();

                result = 1;
            }
            catch (Exception ex)
            {
                conn.Rollback();
                ErrMessages.Add("Database operation error.");
                Debug.WriteLine(ex);
            }

            return result;
        }

        public int UpdateServiceVisit(ServiceVisit serviceVisit, IList<VisitStaff> visitStaffs, IList<VisitTask> visitTasks=null, IList<VisitMeasurement> visitMeasurements=null, IList<AttributeNameValue> attributeNameValues=null)
        {
            int result = 0;
            SQLiteConnectionWithLock conn = AsyncConnection.GetConnection();
            try
            {
                conn.BeginTransaction();
               
                conn.Update(serviceVisit);
                if (visitStaffs!=null && visitStaffs.Any())
                {
                    conn.UpdateAll(visitStaffs);
                }
                if (visitTasks != null && visitTasks.Any())
                {
                    conn.UpdateAll(visitTasks);
                }
                if (visitMeasurements != null && visitMeasurements.Any())
                {
                    conn.UpdateAll(visitMeasurements);
                }
                if (attributeNameValues != null && attributeNameValues.Any())
                {
                    conn.UpdateAll(attributeNameValues);
                }

                conn.Commit();

                result = 1;
            }
            catch (Exception ex)
            {
                conn.Rollback();
                ErrMessages.Add("Database operation error.");
                Debug.WriteLine(ex);
            }

            return result;
        }


        public Task<int> InsertServiceVisit(ServiceVisit serviceVisit, IList<VisitStaff> visitStaffs, IList<VisitTask> visitTasks)
        {
            return Task.Run<int>(() => {
                int result = 0;
                SQLiteConnectionWithLock conn = AsyncConnection.GetConnection();
                try
                {
                    conn.BeginTransaction();

                    conn.Insert(serviceVisit);
                    conn.Commit();

                    conn.BeginTransaction();
                    if (visitStaffs != null && visitStaffs.Any())
                    {
                        foreach (var visitStaff in visitStaffs)
                        {
                            visitStaff.ServiceVisitId = serviceVisit.Id;
                        }
                        conn.InsertAll(visitStaffs);
                    }

                    if (visitTasks != null && visitTasks.Any())
                    {
                        foreach (var visitTask in visitTasks)
                        {
                            visitTask.ServiceVisitId = serviceVisit.Id;
                        }
                        conn.InsertAll(visitTasks);
                    }

                    conn.Commit();
                    result = 1;
                }
                catch (Exception ex)
                {
                    conn.Rollback();
                    Debug.WriteLine(ex);
                }
                return result;
            });
        }

        public int CascadeDeleteServiceVisit(string id)
        {
            int result = 0;
            SQLiteConnectionWithLock conn = AsyncConnection.GetConnection();
            try
            {
                conn.BeginTransaction();
                var sql = string.Format(" DELETE FROM [ServiceVisit] where Id = '{0}'; ", id);
                sql += string.Format(" DELETE FROM [VisitStaff] where ServiceVisitId='{0}'; ", id);
                sql += string.Format(" DELETE FROM [VisitTask] where ServiceVisitId='{0}'; ", id);

                conn.Execute(sql);
                conn.Commit();

                result = 1;
            }
            catch (Exception ex)
            {
                conn.Rollback();
                ErrMessages.Add("Database operation error.");
                Debug.WriteLine(ex);
            }
            return result;
        }

        #endregion
    }
}
