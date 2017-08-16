using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Diagnostics;
using System.Linq;
using System.Web;
using EMS.Models;
using EMS.Utility;
using LinqKit;

namespace EMS.Repository
{
    public class EmployeeRepo
    {
        public static void CreateNewUser(User user_details)
        {
            EMSEntities datacontent = new EMSEntities();
            try
            {
                datacontent.Users.Add(user_details);
                datacontent.SaveChanges();
            }
            catch(Exception exception)
            {
                Debug.WriteLine(exception.Message);
                Debug.WriteLine(exception.GetBaseException());
                throw exception;
            }
            finally
            {
                datacontent.Dispose();
            }
        }
        public static void CreateNewEmployee(Employee Details)
        {
            EMSEntities dataContent = new EMSEntities();
            try
            {
                dataContent.Employees.Add(Details);
                dataContent.SaveChanges();
            }
            catch(Exception exception)
            {
                Debug.WriteLine(exception.Message);
                Debug.WriteLine(exception.GetBaseException());
                throw exception;
            }
            finally
            {
                dataContent.Dispose();
            }
        }

        public static void EditEmployee(Employee Details)
        {
            EMSEntities datacontent = new EMSEntities();
            try
            {
                datacontent.Entry(Details).State = EntityState.Modified;
                datacontent.SaveChanges();
            }
            catch(Exception exception)
            {
                Debug.WriteLine(exception.Message);
                Debug.WriteLine(exception.GetBaseException());
                throw exception;
            }
            finally
            {
                datacontent.Dispose();
            }
        }
        public static List<EmployeeModel> GetEmployeeList(int r_id, int d_id) //r_id reportingto id, d_id designation id of the employee
        {
            EMSEntities datacontent = new EMSEntities();
            try
            {
                var predicate = LinqKit.PredicateBuilder.True<Employee>();
                if (r_id != 0)
                {
                    predicate = predicate.And(i => i.reporting_to == r_id);       
                }
                if (d_id != 0)
                {
                    predicate = predicate.And(i => i.designation == d_id);      
                }
              
                    var query = from employee in datacontent.Employees.AsExpandable().Where(predicate)
                                join user in datacontent.Users
                                on employee.user_id equals user.id
                                where (user.is_active == 1)
                                select new EmployeeModel
                                {
                                    id = employee.id,
                                    first_name = employee.first_name,
                                    last_name = employee.last_name,
                                    email = employee.email,
                                    date_of_birth = employee.date_of_birth,
                                    gender = employee.gender,
                                    date_of_joining = employee.date_of_joining,
                                    contact_no = employee.contact_no,
                                    user_id = employee.user_id,
                                    reporting_to = employee.reporting_to,
                                    Year_of_experence = employee.Year_of_experence,
                                    pan_no = employee.pan_no,
                                    bank_account_no =employee.bank_account_no,
                                    blood_group = employee.blood_group,
                                    designation = employee.designation,
                                    emergency_contact_no = employee.emergency_contact_no,
                                    emergency_contact_person = employee.emergency_contact_person,
                                    medical_insurance_no = employee.medical_insurance_no,
                                    PF_no = employee.PF_no
                                };
                return query.ToList();
       
            }
            catch(Exception exception)
            {
                Debug.WriteLine(exception.Message);
                Debug.WriteLine(exception.GetBaseException());
                throw exception;
            }
            finally
            {
                datacontent.Dispose();
            }
        }
        public static EmployeeModel GetEmployeeDetailsById(int e_id)//e_id employee_id 
        {
            EMSEntities datacontent = new EMSEntities();
            try
            {
                var query = from employee in datacontent.Employees
                            where employee.id == e_id
                            select new EmployeeModel
                            {
                                id = employee.id,
                                first_name = employee.first_name,
                                last_name = employee.last_name,
                                email = employee.email,
                                date_of_birth = employee.date_of_birth,
                                gender = employee.gender,
                                date_of_joining = employee.date_of_joining,
                                contact_no = employee.contact_no,
                                user_id = employee.user_id,
                                reporting_to = employee.reporting_to,
                                Year_of_experence = employee.Year_of_experence,
                                pan_no = employee.pan_no,
                                bank_account_no = employee.bank_account_no,
                                blood_group = employee.blood_group,
                                designation = employee.designation,
                                emergency_contact_no = employee.emergency_contact_no,
                                emergency_contact_person = employee.emergency_contact_person,
                                medical_insurance_no = employee.medical_insurance_no,
                                PF_no = employee.PF_no
                            };
                return query.FirstOrDefault();
            }
            catch(Exception exception)
            {
                Debug.WriteLine(exception.Message);
                Debug.WriteLine(exception.GetBaseException());
                throw exception;
            }
            finally
            {
                datacontent.Dispose();
            }
        }

        public static Employee GetEmployeeById(int e_id)//e_id employee_id
        {
            EMSEntities datacontent = new EMSEntities();
            try
            {
                var query = from x in datacontent.Employees
                            where x.id == e_id
                            select x;
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
                datacontent.Dispose();
            }
        }

        public static void InsertLeaveBalance(Employee employee, int[] arr)//arr constant array - contains manager , HR and TeamLead ID
        {
            EMSEntities datacontext = new EMSEntities();
            try
            {
                for (int i = 0; i < arr.Length; i++)
                {
                    Leavebalance_sheet leave_balance = new Leavebalance_sheet();
                    leave_balance.employee_id = employee.id;
                    leave_balance.leavetype_id = arr[i];
                    leave_balance.no_of_days = (decimal)Utils.LeaveCalculationBasedDOJ(employee.date_of_joining, arr[i]);
                    datacontext.Leavebalance_sheet.Add(leave_balance);
                }
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
        
        public static void InactiveEmployee(Employee existinginstance)
        {
            EMSEntities datacontext = new EMSEntities();
            try
            {
                //var query = from employee in datacontext.Employees
                //            join user in datacontext.Users
                //            on employee.user_id equals user.id
                //            where employee.id == e_id && user.is_active ==1
                //            select user;
                var query = from user in datacontext.Users
                            where existinginstance.user_id == user.id && user.is_active==1
                            select user;
                User user_details = query.FirstOrDefault();
                user_details.is_active = 0;
                datacontext.Entry(user_details).State = EntityState.Modified;
                datacontext.SaveChanges();
                //datacontext.Entry(query.FirstOrDefault()).State = EntityState.Deleted;
                //datacontext.SaveChanges();
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

        public static void AssignEmployeeRole(User_role user_role)
        {
            EMSEntities datacontext = new EMSEntities();
            try
            {
                datacontext.User_role.Add(user_role);
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

        public static int GetEmployeeStatusById(int e_id)//e_id employee_id
        {
            EMSEntities datacontext = new EMSEntities();
            try
            {
                var query = from empl in datacontext.Employees
                            join user in datacontext.Users
                            on empl.user_id equals user.id
                            select user.is_active;
                return query.FirstOrDefault();
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

        public static List<DesignationModel> GetDesignationList()
        {
            EMSEntities datacontext = new EMSEntities();
            try
            {
                var query = from d_list in datacontext.Designations
                            select new DesignationModel
                            {
                                id = d_list.id,
                                name = d_list.name,
                                description = d_list.description
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

        public static List<ReportingTo> GetReportingtoList()
        {
            EMSEntities datacontext = new EMSEntities();
            try
            {
                var query = from x in datacontext.Employees
                            join y in datacontext.User_role
                            on x.user_id equals y.user_id
                            where y.role_id == Constants.HR || y.role_id == Constants.Systemrole_Manager || y.role_id == Constants.Systemrole_TeamLeader
                            select new ReportingTo
                            {
                                emp_name = x.first_name,
                                emp_id = x.id
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
    }
}