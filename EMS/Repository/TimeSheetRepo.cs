using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Diagnostics;
using System.Linq;
using System.Web;
using EMS.Models;

namespace EMS.Repository
{
	public class TimeSheetRepo
	{
        public static void AddTimeSheetRecord(List<Timesheet> sheetrecord)
        {
            EMSEntities datacontext = new EMSEntities();
            try
            {
                foreach(Timesheet items in sheetrecord)
                {
                    datacontext.Timesheets.Add(items);
                }
                datacontext.SaveChanges();
            }
            catch(Exception exception)
            {
                Debug.WriteLine(exception.Message);
                Debug.WriteLine(exception.GetBaseException());
                throw exception;
            }
            finally
            {
                datacontext.Dispose();
            }
        }

        public static TimeSheetModel GetSheetById(int s_id)//s_id timesheet_id
        {
            EMSEntities datacontext = new EMSEntities();
            try
            {
                var query = from sheet in datacontext.Timesheets
                            where sheet.id == s_id
                            select new TimeSheetModel {
                                id = sheet.id,
                                employee_id = sheet.employee_id,
                                task_id = sheet.task_id,
                                project_id = sheet.project_id,
                                work_date = sheet.work_date,
                                work_hour = sheet.work_hour
                            };
                return query.FirstOrDefault();
            }
            catch (Exception exception)
            {
                Debug.WriteLine(exception.Message);
                Debug.WriteLine(exception.GetBaseException());
                throw exception;
            }
            finally
            {
                datacontext.Dispose();
            }
        }

        public static void UpdateTimeSheetRecord(Timesheet sheetdetails)
        {
            EMSEntities datacontext = new EMSEntities();
            try
            {
                datacontext.Entry(sheetdetails).State = EntityState.Modified;
                datacontext.SaveChanges();
            }
            catch (Exception exception)
            {
                Debug.WriteLine(exception.Message);
                Debug.WriteLine(exception.GetBaseException());
                throw exception;
            }
            finally
            {
                datacontext.Dispose();
            }
        }

        public static List<TimeSheetModel> GetSheetByEmpId(int e_id)//e_id employee_id
        {
            EMSEntities datacontext = new EMSEntities();
            try
            {
                var query = from sheet in datacontext.Timesheets
                            where sheet.employee_id == e_id
                            select new TimeSheetModel
                            {
                                id = sheet.id,
                                employee_id = sheet.employee_id,
                                task_id = sheet.task_id,
                                project_id = sheet.project_id,
                                work_date = sheet.work_date,
                                work_hour = sheet.work_hour
                            };
                return query.ToList();
            }
            catch (Exception exception)
            {
                Debug.WriteLine(exception.Message);
                Debug.WriteLine(exception.GetBaseException());
                throw exception;
            }
            finally
            {
                datacontext.Dispose();
            }
        }

        public static List<TimeSheetModel> GetSheetByProjId(int p_id)//p_id project_id
        {
            EMSEntities datacontext = new EMSEntities();
            try
            {
                var query = from sheet in datacontext.Timesheets
                            where sheet.project_id == p_id
                            select new TimeSheetModel
                            {
                                id = sheet.id,
                                employee_id = sheet.employee_id,
                                task_id = sheet.task_id,
                                project_id = sheet.project_id,
                                work_date = sheet.work_date,
                                work_hour = sheet.work_hour
                            };
                return query.ToList();
            }
            catch (Exception exception)
            {
                Debug.WriteLine(exception.Message);
                Debug.WriteLine(exception.GetBaseException());
                throw exception;
            }
            finally
            {
                datacontext.Dispose();
            }
        }
        
        public static void DeleteTimeSheetRecord(int s_id)//timesheet_id
        {
            EMSEntities datacontext = new EMSEntities();
            try
            {
                var query = from sheet in datacontext.Timesheets
                            where sheet.id == s_id
                            select sheet;
                datacontext.Entry(query.FirstOrDefault()).State = EntityState.Deleted;
                datacontext.SaveChanges();
            }
            catch (Exception exception)
            {
                Debug.WriteLine(exception.Message);
                Debug.WriteLine(exception.GetBaseException());
                throw exception;
            }
            finally
            {
                datacontext.Dispose();
            }
        }
    }
}