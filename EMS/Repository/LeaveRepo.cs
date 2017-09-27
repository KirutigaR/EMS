using EMS.Models;
using EMS.Utility;
using LinqKit;
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
                throw e;
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
                throw e;
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
                throw e;
            }
            finally
            {
                datacontext.Dispose();
            }
        }
        public static decimal GetNoofDaysById(int id, int emp_id)
        {
            EMSEntities datacontext = new EMSEntities();
            try
            {
                var query = from lbs in datacontext.Leavebalance_sheet
                            where lbs.leavetype_id == id && lbs.employee_id == emp_id
                            select lbs.no_of_days;
                return query.FirstOrDefault();
            }
            catch (Exception e)
            {
                Debug.WriteLine(e.Message);
                Debug.WriteLine(e.GetBaseException());
                throw e;
            }
            finally
            {
                datacontext.Dispose();
            }
        }
        public static void AddLeaveHistory(Leave leave)
        {
            EMSEntities datacontext = new EMSEntities();
            try
            {
                leave.leave_statusid = Constants.LEAVE_STATUS_PENDING;
                datacontext.Leaves.Add(leave);
                datacontext.SaveChanges();
            }
            catch (Exception e)
            {
                Debug.WriteLine(e.Message);
                Debug.WriteLine(e.GetBaseException());
                throw e;
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
                throw e;
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
                throw e;
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
                throw e;
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
                throw e;
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
                throw e;
            }
            finally
            {
                datacontext.Dispose();
            }
        }
        public static void EditLeave(Leave leave)
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
                throw e;
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
                throw e;
            }
            finally
            {
                datacontext.Dispose();
            }
        }
        public static string GetLeaveTypeById(int id)
        {
            EMSEntities datacontext = new EMSEntities();
            try
            {
                var query = from lv in datacontext.Leave_type
                            where lv.id == id
                            select lv.type_name;
                return query.FirstOrDefault();
            }
            catch (Exception e)
            {
                Debug.WriteLine(e.Message);
                Debug.WriteLine(e.GetBaseException());
                throw e;
            }
            finally
            {
                datacontext.Dispose();
            }
        }
        public static List<LeavehistoryModel> GetLeaveHistoryById(int id)
        {
            EMSEntities datacontext = new EMSEntities();
            try
            {
                var query = from e in datacontext.Employees
                            join l in datacontext.Leaves
                            on e.id equals l.employee_id
                            join lt in datacontext.Leave_type
                            on l.leavetype_id equals lt.id
                            join ls in datacontext.Status_leave
                            on l.leave_statusid equals ls.id
                            orderby l.id descending
                            where l.employee_id == id//&& l.leavetype_id == 2
                            select new LeavehistoryModel
                            {
                                //id = e.id,
                                //first_name = e.first_name,
                                //last_name = e.last_name,
                                type_name = lt.type_name,
                                from_date = l.from_date,
                                to_date = l.to_date,
                                no_of_days = l.no_of_days,
                                leave_status = ls.leave_status
                            };
                return query.ToList();

            }
            catch (Exception e)
            {
                Debug.WriteLine(e.Message);
                Debug.WriteLine(e.GetBaseException());
                throw e;
            }
            finally
            {
                datacontext.Dispose();
            }
        }
        //public static List<Holiday_List> GetHolidayList()
        //{
        //    EMSEntities datacontext = new EMSEntities();
        //    try
        //    {
        //        var query = from hl in datacontext.Holiday_List
        //                    select hl;
        //        return query.ToList();
        //    }
        //    catch (Exception e)
        //    {
        //        Debug.WriteLine(e.Message);
        //        Debug.WriteLine(e.GetBaseException());
        //        throw e;
        //    }
        //    finally
        //    {
        //        datacontext.Dispose();
        //    }
        //}
        public static List<LeaveBalanceModel> GetLeaveBalanceById(int id)
        {
            EMSEntities datacontext = new EMSEntities();
            try
            {
                var query = from lbs in datacontext.Leavebalance_sheet
                            join lt in datacontext.Leave_type
                            on lbs.leavetype_id equals lt.id
                            where lbs.employee_id == id
                            select new LeaveBalanceModel
                            {
                                leavetype_id = lbs.leavetype_id,
                                type_name = lt.type_name,
                                no_of_days = (decimal)lbs.no_of_days
                            };
                return query.ToList();
            }
            catch (Exception e)
            {
                Debug.WriteLine(e.Message);
                Debug.WriteLine(e.GetBaseException());
                throw e;
            }
            finally
            {
                datacontext.Dispose();
            }
        }

        public static int GetUserIdById(int id)
        {
            EMSEntities datacontext = new EMSEntities();
            try
            {
                var query = from e in datacontext.Employees
                            where e.id == id
                            select e.user_id;
                return query.FirstOrDefault();

            }
            catch (Exception e)
            {
                Debug.WriteLine(e.Message);
                Debug.WriteLine(e.GetBaseException());
                throw e;
            }
            finally
            {
                datacontext.Dispose();
            }
        }
        public static User GetUserById(int id)
        {
            EMSEntities datacontext = new EMSEntities();
            try
            {
                var query = from u in datacontext.Users
                            where u.id == id
                            select u;
                return query.FirstOrDefault();
            }
            catch (Exception e)
            {
                Debug.WriteLine(e.Message);
                Debug.WriteLine(e.GetBaseException());
                throw e;
            }
            finally
            {
                datacontext.Dispose();
            }
        }
        public static void EditUserPassword(User user)
        {
            EMSEntities datacontext = new EMSEntities();
            try
            {
                datacontext.Entry(user).State = EntityState.Modified;
                datacontext.SaveChanges();
            }
            catch (Exception e)
            {
                Debug.WriteLine(e.Message);
                Debug.WriteLine(e.GetBaseException());
                throw e;
            }
            finally
            {
                datacontext.Dispose();
            }
        }
        public static Leavebalance_sheet LeaveBalanceById(int employee_id, int leavetype_id)
        {
            EMSEntities datacontext = new EMSEntities();
            try
            {
                var query = from lbs in datacontext.Leavebalance_sheet
                            where lbs.employee_id == employee_id && lbs.leavetype_id == leavetype_id
                            select lbs;
                return query.FirstOrDefault();
            }
            catch (Exception e)
            {
                Debug.WriteLine(e.Message);
                Debug.WriteLine(e.GetBaseException());
                throw e;
            }
            finally
            {
                datacontext.Dispose();
            }
        }
        public static decimal? GetNoofdaysByLeaveTypeId(int id)
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
                throw e;
            }
            finally
            {
                datacontext.Dispose();
            }
        }
        public static void UpdateLeaveBalanceSheet(Leavebalance_sheet leave_balance)
        {
            EMSEntities datacontext = new EMSEntities();
            try
            {
                datacontext.Entry(leave_balance).State = EntityState.Modified;
                datacontext.SaveChanges();
            }
            catch (Exception e)
            {
                Debug.WriteLine(e.Message);
                Debug.WriteLine(e.GetBaseException());
                throw e;
            }
            finally
            {
                datacontext.Dispose();
            }
        }
        public static List<EmployeeListByRoleModel> GetEmployeeListByRole(int id)
        {
            EMSEntities datacontext = new EMSEntities();
            try
            {
                var query = from pr in datacontext.Project_role
                            join e in datacontext.Employees on pr.employee_id equals e.id
                            join p in datacontext.Projects on pr.project_id equals p.id
                            join r in datacontext.Roles on pr.role_id equals r.id
                            where e.reporting_to == id && r.role_type == "Project Role"
                            select new EmployeeListByRoleModel
                            {
                                id = e.id,
                                first_name = e.first_name,
                                last_name = e.last_name,
                                email = e.email,
                                project_name = p.project_name,
                                project_role = r.role_name
                            };
                return query.ToList();
            }
            catch (Exception e)
            {
                Debug.WriteLine(e.Message);
                Debug.WriteLine(e.GetBaseException());
                throw e;
            }
            finally
            {
                datacontext.Dispose();
            }
        }

        //public static List<EmployeeListByRoleModel> emp_proj_details_byrole(EmployeeListByRoleModel details)
        //{
        //    EMSEntities datacontext = new EMSEntities();
        //    try
        //    {
        //        foreach(EmployeeListByRoleModel items in details)
        //        {
        //            var query = from 
        //        }
        //    }
        //}

        public static List<LeavehistoryModel> GetRequestByRoleId(int id)
        {
            EMSEntities datacontext = new EMSEntities();
            try
            {
                var query = from e in datacontext.Employees
                            join l in datacontext.Leaves
                            on e.id equals l.employee_id
                            join lt in datacontext.Leave_type
                            on l.leavetype_id equals lt.id
                            join ls in datacontext.Status_leave on l.leave_statusid equals ls.id
                            orderby l.id descending
                            where e.reporting_to == id && l.leave_statusid == Constants.LEAVE_STATUS_PENDING && l.from_date > DateTime.Now //&& l.leave_statusid == ls.id
                            select new LeavehistoryModel
                            {
                                id = e.id,
                                first_name = e.first_name,
                                last_name = e.last_name,
                                type_name = lt.type_name,
                                from_date = l.from_date,
                                to_date = l.to_date,
                                no_of_days = l.no_of_days,
                                //is_approved = l.is_approved,
                                leave_id = l.id,
                                leave_status = ls.leave_status,
                                reporting_to = e.reporting_to
                            };
                return query.ToList();
            }
            catch (Exception e)
            {
                Debug.WriteLine(e.Message);
                Debug.WriteLine(e.GetBaseException());
                throw e;
            }
            finally
            {
                datacontext.Dispose();
            }
        }
        public static List<LeavehistoryModel> GetPendingLeave()
        {
            EMSEntities datacontext = new EMSEntities();
            try
            {
                var query = from l in datacontext.Leaves
                            join e in datacontext.Employees
                            on l.employee_id equals e.id
                            join lt in datacontext.Leave_type
                            on l.leavetype_id equals lt.id
                            join st in datacontext.Status_leave on l.leave_statusid equals st.id
                            orderby l.id descending
                            where l.leave_statusid == Constants.LEAVE_STATUS_PENDING && l.from_date > DateTime.Now
                            select new LeavehistoryModel
                            {
                                id = e.id,
                                first_name = e.first_name,
                                last_name = e.last_name,
                                type_name = lt.type_name,
                                from_date = l.from_date,
                                to_date = l.to_date,
                                no_of_days = l.no_of_days,
                                reporting_to = e.reporting_to,
                                leave_status = st.leave_status,
                                leave_id = l.id
                            };
                return query.ToList();
            }
            catch (Exception e)
            {
                Debug.WriteLine(e.Message);
                Debug.WriteLine(e.GetBaseException());
                throw e;
            }
            finally
            {
                datacontext.Dispose();
            }
        }
        public static Leave GetLeaveInstanceById(int id)
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
                throw e;
            }
            finally
            {
                datacontext.Dispose();
            }
        }
        public static List<LeaveTypeListModel> GetLeaveTypeList()
        {
            EMSEntities datacontext = new EMSEntities();
            try
            {
                var query = from lt in datacontext.Leave_type
                                //join lt in datacontext.Leave_type on l.leavetype_id equals lt.id
                            select new LeaveTypeListModel
                            {
                                leavetype_id = lt.id,
                                type_name = lt.type_name
                            };
                return query.ToList();
            }
            catch (Exception e)
            {
                Debug.WriteLine(e.Message);
                Debug.WriteLine(e.GetBaseException());
                throw e;
            }
            finally
            {
                datacontext.Dispose();
            }
        }
        public static List<LeavehistoryModel> GetPendingApprovedLeave(int r_id)
        {
            EMSEntities datacontext = new EMSEntities();
            try
            {
                var predicate = LinqKit.PredicateBuilder.True<Employee>();
                if (r_id != 0)
                {
                    predicate = predicate.And(X => X.reporting_to == r_id);
                }
                var query = from l in datacontext.Leaves
                            join e in datacontext.Employees.AsExpandable().Where(predicate)
                            on l.employee_id equals e.id
                            join lt in datacontext.Leave_type
                            on l.leavetype_id equals lt.id
                            join st in datacontext.Status_leave on l.leave_statusid equals st.id
                            orderby l.id descending
                            where l.from_date > DateTime.Now
                            select new LeavehistoryModel
                            {
                                id = e.id,
                                first_name = e.first_name,
                                last_name = e.last_name,
                                type_name = lt.type_name,
                                from_date = l.from_date,
                                to_date = l.to_date,
                                no_of_days = l.no_of_days,
                                reporting_to = e.reporting_to,
                                leave_status = st.leave_status
                            };
                return query.ToList();
            }
            catch (Exception e)
            {
                Debug.WriteLine(e.Message);
                Debug.WriteLine(e.GetBaseException());
                throw e;
            }
            finally
            {
                datacontext.Dispose();
            }
        }
        public static List<LeavehistoryModel> GetLeaveHistory()
        {
            EMSEntities datacontext = new EMSEntities();
            try
            {
                var query = from l in datacontext.Leaves
                            join e in datacontext.Employees
                            on l.employee_id equals e.id
                            join lt in datacontext.Leave_type
                            on l.leavetype_id equals lt.id
                            join st in datacontext.Status_leave on l.leave_statusid equals st.id
                            orderby l.id descending
                            where l.from_date > DateTime.Now
                            select new LeavehistoryModel
                            {
                                id = e.id,
                                first_name = e.first_name,
                                last_name = e.last_name,
                                type_name = lt.type_name,
                                from_date = l.from_date,
                                to_date = l.to_date,
                                no_of_days = l.no_of_days,
                                reporting_to = e.reporting_to,
                                leave_status = st.leave_status
                            };
                return query.ToList();
            }
            catch (Exception e)
            {
                Debug.WriteLine(e.Message);
                Debug.WriteLine(e.GetBaseException());
                throw e;
            }
            finally
            {
                datacontext.Dispose();
            }
        }
        public static List<Leave> GetActiveLeaveListByEmpId(int employee_id)
        {
            EMSEntities datacontext = new EMSEntities();
            try
            {
                var query = from l in datacontext.Leaves
                            where l.employee_id == employee_id && (l.leave_statusid == Constants.LEAVE_STATUS_APPROVED || l.leave_statusid == Constants.LEAVE_STATUS_PENDING) && l.from_date >= DateTime.Now
                            select l;
                return query.ToList();
            }
            catch (Exception e)
            {
                Debug.WriteLine(e.Message);
                Debug.WriteLine(e.GetBaseException());
                throw e;
            }
            finally
            {
                datacontext.Dispose();
            }
        }
    }
}