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
        public static void EditEmployeeLeaveByApplyLeave(int employee_id, string LeaveType, DateTime? FromDate, DateTime? ToDate, decimal noofdays)
        {
            EMSEntities datacontext = new EMSEntities();
            try
            {
                if (LeaveType != "Maternity Leave")
                {
                    Leave leave = new Leave();

                    leave.employee_id = employee_id;
                    leave.leave_type = LeaveType;
                    leave.from_date = (DateTime)FromDate;
                    history.to_date = (DateTime)ToDate;
                    history.no_of_days = noofdays;
                    datacontext.Leave_history.Add(history);
                    datacontext.SaveChanges();
                }
                else
                {
                    Leave_history history = new Leave_history();
                    history.employee_id = employee_id;
                    history.leave_type = LeaveType;
                    history.from_date = (DateTime)FromDate;
                    history.to_date = (DateTime)ToDate;
                    history.no_of_days = noofdays;
                    datacontext.Leave_history.Add(history);
                    datacontext.SaveChanges();
                }
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
    }
}