using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Diagnostics;
using System.Linq;
using System.Web;

namespace EMS.Repository
{
    public class LeaveRepo
    {
        public static List<Holiday_List> GetHoliday()
        {
            EMSEntities datacontext = new EMSEntities();
            try
            {
                var query = from h in datacontext.Holiday_List
                            select h;
                return query.ToList();
            }
            catch (Exception e)
            {
                Debug.WriteLine(e.Message);
                Debug.WriteLine(e.GetBaseException());
                return null;
            }
            finally
            {
                datacontext.Dispose();
            }
        }
        public static List<DateTime> GetDateFromHoliday()
        {
            EMSEntities datacontext = new EMSEntities();
            try
            {
                var query = from h in datacontext.Holiday_List
                            select h.holiday_date;
                return query.ToList();
            }
            catch (Exception e)
            {
                Debug.WriteLine(e.Message);
                Debug.WriteLine(e.GetBaseException());
                return null;
            }
            finally
            {
                datacontext.Dispose();
            }
        }
        public static int GetLeavetypeIdByLeavetype(string type_name)
        {
            EMSEntities datacontext = new EMSEntities();
            try
            {
                var query = from lt in datacontext.Leave_type
                            where lt.type_name == type_name
                            select lt.id;
                return query.FirstOrDefault();
            }
            catch (Exception e)
            {
                Debug.WriteLine(e.Message);
                Debug.WriteLine(e.GetBaseException());
                return 0;
            }
            finally
            {
                datacontext.Dispose();
            }
        }
        public static decimal GetNoofDaysById(int id)
        {
            EMSEntities datacontext = new EMSEntities();
            try
            {
                var query = from lbs in datacontext.Leavebalance_sheet
                            where lbs.leavetype_id == id
                            select lbs.no_of_days;
                return query.FirstOrDefault();
            }
            catch (Exception e)
            {
                Debug.WriteLine(e.Message);
                Debug.WriteLine(e.GetBaseException());
                return 0;
            }
            finally
            {
                datacontext.Dispose();
            }
        }
        public static void EditLeaveHistory(Leave leave)
        {
            EMSEntities datacontext = new EMSEntities();
            try
            {
                datacontext.Leaves.Add(leave);
                datacontext.SaveChanges();
            }
            catch (Exception e)
            {
                Debug.WriteLine(e.Message);
                Debug.WriteLine(e.GetBaseException());
            }
            finally
            {
                datacontext.Dispose();
            }
        }
        public static Holiday_List GetHolidayById(int id)
        {
            EMSEntities datacontext = new EMSEntities();
            try
            {
                var query = from hl in datacontext.Holiday_List
                            where hl.id == id
                            select hl;
                return query.FirstOrDefault();
            }
            catch (Exception e)
            {
                Debug.WriteLine(e.Message);
                Debug.WriteLine(e.GetBaseException());
                return null;
            }
            finally
            {
                datacontext.Dispose();
            }
        }
        public static void EditHolidayList(Holiday_List holiday_list)
        {
            EMSEntities datacontext = new EMSEntities();
            try
            {
                datacontext.Entry(holiday_list).State = EntityState.Modified;
                datacontext.SaveChanges();
            }
            catch (Exception e)
            {
                Debug.WriteLine(e.Message);
                Debug.WriteLine(e.GetBaseException());
            }
            finally
            {
                datacontext.Dispose();
            }
        }
        public static void DeleteHolidayList(Holiday_List holiday_list)
        {
            EMSEntities datacontext = new EMSEntities();
            try
            {
                datacontext.Entry(holiday_list).State = EntityState.Deleted;
                datacontext.SaveChanges();
            }
            catch (Exception e)
            {
                Debug.WriteLine(e.Message);
                Debug.WriteLine(e.GetBaseException());
            }
            finally
            {
                datacontext.Dispose();
            }
        }
        public static void CreateHoliday(Holiday_List holiday_list)
        {
            EMSEntities datacontext = new EMSEntities();
            try
            {
                datacontext.Holiday_List.Add(holiday_list);
                datacontext.SaveChanges();
            }
            catch (Exception e)
            {
                Debug.WriteLine(e.Message);
                Debug.WriteLine(e.GetBaseException());
            }
            finally
            {
                datacontext.Dispose();
            }
        }
        public static Leave GetLeaveById(int id)
        {
            EMSEntities datacontext = new EMSEntities();
            try
            {
                var query = from l in datacontext.Leaves
                            where l.id == id
                            select l;
                return query.FirstOrDefault();
            }
            catch (Exception e)
            {
                Debug.WriteLine(e.Message);
                Debug.WriteLine(e.GetBaseException());
                return null;
            }
            finally
            {
                datacontext.Dispose();
            }
        }
        public static void ApproveLeave(Leave leave)
        {
            EMSEntities datacontext = new EMSEntities();
            try
            {
                datacontext.Entry(leave).State = EntityState.Modified;
                datacontext.SaveChanges();
            }
            catch (Exception e)
            {
                Debug.WriteLine(e.Message);
                Debug.WriteLine(e.GetBaseException());

            }
            finally
            {
                datacontext.Dispose();
            }
        }
        public static decimal GetYearleave(string Leavetype)
        {
            EMSEntities datacontext = new EMSEntities();
            try
            {
                var query = from lt in datacontext.Leave_type
                            where lt.type_name == Leavetype
                            select lt.days_per_year;
                return query.FirstOrDefault();
            }
            catch (Exception e)
            {
                Debug.WriteLine(e.Message);
                Debug.WriteLine(e.GetBaseException());
                return 0;
            }
            finally
            {
                datacontext.Dispose();
            }
        }
    }
}