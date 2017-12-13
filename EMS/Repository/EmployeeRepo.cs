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

        public static List<EmployeeModel> GetEmployeeList(int reportingto_id, int designation_id) 
        {
            EMSEntities datacontent = new EMSEntities();
            try
            {
                var predicate = LinqKit.PredicateBuilder.True<Employee>();
                if (reportingto_id != 0)
                {
                    predicate = predicate.And(i => i.reporting_to == reportingto_id);       
                }
                if (designation_id != 0)
                {
                    predicate = predicate.And(i => i.designation == designation_id);      
                }
              
                    var query = from employee in datacontent.Employees.AsExpandable().Where(predicate)
                                join user in datacontent.Users
                                on employee.user_id equals user.id
                                join designation in datacontent.Designations
                                on employee.designation equals designation.id
                                join employee1 in datacontent.Employees
                                on employee.reporting_to equals employee1.id
                                orderby employee.created_on descending
                                where (user.is_active == 1)
                                select new EmployeeModel
                                {
                                    id = employee.id,
                                    first_name = employee.first_name,
                                    last_name = employee.last_name,
                                    //email = employee.email,
                                    //date_of_birth = employee.date_of_birth,
                                    gender = employee.gender,
                                    date_of_joining = employee.date_of_joining,
                                    //contact_no = employee.contact_no,
                                    //user_id = employee.user_id,
                                    //reporting_to = employee.reporting_to,
                                    reportingto_name = employee1.first_name + " " + employee1.last_name,
                                    //Year_of_experience = employee.year_of_experience,
                                    //pan_no = employee.pan_no,
                                    //bank_account_no =employee.bank_account_no,
                                    blood_group = employee.blood_group,
                                    //designation_id = designation.id,
                                    designation = designation.name,
                                    emergency_contact_no = employee.emergency_contact_no,
                                    emergency_contact_person = employee.emergency_contact_person,
                                    medical_insurance_no = employee.medical_insurance_no,
                                    //PF_no = employee.PF_no,
                                    //created_on = (DateTime)employee.created_on
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

        public static List<EmployeeModel> GetAvailableEmployeeList(int reportingto_id, int designation_id) //r_id reportingto id, d_id designation id of the employee
        {
            EMSEntities datacontent = new EMSEntities();
            try
            {
                var predicate = LinqKit.PredicateBuilder.True<Employee>();
                if (reportingto_id != 0)
                {
                    predicate = predicate.And(i => i.reporting_to == reportingto_id);
                }
                if (designation_id != 0)
                {
                    predicate = predicate.And(i => i.designation == designation_id);
                }

                var query = from employee in datacontent.Employees.AsExpandable().Where(predicate)
                            join user in datacontent.Users
                            on employee.user_id equals user.id
                            join designation in datacontent.Designations
                            on employee.designation equals designation.id
                            join project_role in datacontent.Project_role
                            on employee.id equals project_role.employee_id
                            join employee1 in datacontent.Employees
                            on employee.reporting_to equals employee1.id
                            where (user.is_active == 1) && (project_role.project_id == Constants.PROJECT_BENCH_ID)//available employees are the employees who are all in bench(project id 1 = bench ) 
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
                                //user_id = employee.user_id,
                                //reporting_to = employee.reporting_to,
                                reportingto_name = employee1.first_name +" "+ employee1.last_name,
                                Year_of_experience = (decimal)employee.year_of_experience,
                                pan_no = employee.pan_no,
                                bank_account_no = employee.bank_account_no,
                                blood_group = employee.blood_group,
                                //designation_id = designation.id,
                                designation = designation.name,
                                emergency_contact_no = employee.emergency_contact_no,
                                emergency_contact_person = employee.emergency_contact_person,
                                medical_insurance_no = employee.medical_insurance_no,
                                PF_no = employee.PF_no
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
                datacontent.Dispose();
            }
        }

        public static EmployeeModel GetEmployeeDetailsByUserId(int user_id)//u_id user_id 
        {
            EMSEntities datacontent = new EMSEntities();
            try
            {
                var query = from employee in datacontent.Employees
                            join userrole in datacontent.User_role
                            on employee.user_id equals userrole.user_id
                            join salary in datacontent.Salary_Structure
                            on employee.id equals salary.emp_id
                            join designation in datacontent.Designations
                            on employee.designation equals designation.id
                            join employee1 in datacontent.Employees
                            on employee.reporting_to equals employee1.id
                            where employee.user_id == user_id
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
                                role_id = userrole.role_id,
                                reporting_to = employee.reporting_to,
                                reportingto_name = employee1.first_name + " " + employee1.last_name,
                                Year_of_experience = (decimal)employee.year_of_experience,
                                pan_no = employee.pan_no,
                                bank_account_no = employee.bank_account_no,
                                blood_group = employee.blood_group,
                                designation_id = designation.id,
                                designation = designation.name,
                                emergency_contact_no = employee.emergency_contact_no,
                                emergency_contact_person = employee.emergency_contact_person,
                                medical_insurance_no = employee.medical_insurance_no,
                                PF_no = employee.PF_no,
                                ctc = salary.ctc
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
                datacontent.Dispose();
            }
        }

        public static EmployeeModel GetEmployeeDetailsById(int employee_id)//e_id employee_id 
        {
            EMSEntities datacontent = new EMSEntities();
            try
            {
                var query = from employee in datacontent.Employees
                            join userrole in datacontent.User_role
                            on employee.user_id equals userrole.user_id
                            join salary in datacontent.Salary_Structure
                            on employee.id equals salary.emp_id
                            join designation in datacontent.Designations
                            on employee.designation equals designation.id
                            join employee1 in datacontent.Employees
                            on employee.reporting_to equals employee1.id
                            join role in datacontent.Roles on userrole.role_id equals role.id
                            where employee.id == employee_id && salary.is_active == 1
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
                                //user_id = employee.user_id,
                                role_id = userrole.role_id,
                                reporting_to = employee.reporting_to,
                                reportingto_name = employee1.first_name + " " + employee1.last_name,
                                Year_of_experience = (decimal)employee.year_of_experience,
                                pan_no = employee.pan_no,
                                bank_account_no = employee.bank_account_no,
                                blood_group = employee.blood_group,
                                designation_id = designation.id,
                                designation = designation.name,
                                emergency_contact_no = employee.emergency_contact_no,
                                emergency_contact_person = employee.emergency_contact_person,
                                medical_insurance_no = employee.medical_insurance_no,
                                PF_no = employee.PF_no,
                                ctc = salary.ctc,
                                role_name = role.role_name
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

        public static Employee GetEmployeeById(int employee_id)//e_id employee_id
        {
            EMSEntities datacontent = new EMSEntities();
            try
            {
                var query = from x in datacontent.Employees
                            where x.id == employee_id
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
                    leave_balance.actual_balance = (decimal)Utils.LeaveCalculationBasedDOJ(employee.date_of_joining, arr[i]);
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

        public static int GetEmployeeStatusById(int employee_id)//e_id employee_id
        {
            EMSEntities datacontext = new EMSEntities();
            try
            {
                var query = from empl in datacontext.Employees
                            join user in datacontext.Users on empl.user_id equals user.id
                            where empl.id == employee_id
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
                var query = from employee in datacontext.Employees
                            join userrole in datacontext.User_role on employee.user_id equals userrole.user_id
                            join user in datacontext.Users on employee.user_id equals user.id
                            where (user.is_active == 1) && (userrole.role_id == Constants.ROLE_HR || userrole.role_id == Constants.SYSTEMROLE_MANAGER || userrole.role_id == Constants.SYSTEMROLE_TEAMLEAD)
                            select new ReportingTo
                            {
                                emp_name = employee.first_name + " " +employee.last_name ,
                                emp_id = employee.id
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

        public static ReportingTo GetReportingtoByEmpId(int employee_id)
        {
            EMSEntities datacontext = new EMSEntities();
            try
            {
                var query = from employee in datacontext.Employees
                            join employee2 in datacontext.Employees
                            on employee.reporting_to equals employee2.id
                            where employee.id == employee_id
                            select new ReportingTo
                            {
                               emp_name = employee2.first_name,
                               emp_id = employee2.id,
                               mailid = employee2.email
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

        public static User_role GetUserRoleByUserid(int userid)
        {
            EMSEntities datacontext = new EMSEntities();
            try
            {
                var query = from userrole in datacontext.User_role
                            where userrole.user_id == userid
                            select userrole;
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

        public static void UpdateUserRole(User_role userrole)
        {
            EMSEntities datacontext = new EMSEntities();
            try
            {
                datacontext.Entry(userrole).State = EntityState.Modified;
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

        public static List<Employee> GetEmployeeByMailId(string mailid)
        {
            EMSEntities datacontext = new EMSEntities();
            try
            {
                var query = from employee in datacontext.Employees
                            where employee.email == mailid
                            select employee;
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

        public static List<EmployeeModel> SearchEmployee(int employee_id, string Employee_name)
        {
            EMSEntities datacontent = new EMSEntities();
            try
            {
                var predicate = LinqKit.PredicateBuilder.True<Employee>();
                if (employee_id != 0)
                {
                    predicate = predicate.And(i => i.id == employee_id);
                }
                if (Employee_name != null)
                {
                    predicate = predicate.And(i => i.first_name == Employee_name).Or(i=>i.last_name == Employee_name);
                }
                var query = from employee in datacontent.Employees.AsExpandable().Where(predicate)
                            join userrole in datacontent.User_role on employee.user_id equals userrole.user_id
                            join designation in datacontent.Designations on employee.designation equals designation.id
                            join employee1 in datacontent.Employees on employee.reporting_to equals employee1.id
                            select new EmployeeModel
                            {
                                id = employee.id,
                                first_name = employee.first_name,
                                last_name = employee.last_name,
                                email = employee.email,
                                //date_of_birth = employee.date_of_birth,
                                //gender = employee.gender,
                                //date_of_joining = employee.date_of_joining,
                                //contact_no = employee.contact_no,
                                //user_id = employee.user_id,
                                //role_id = userrole.role_id,
                                //reporting_to = employee.reporting_to,
                                //reportingto_name = employee1.first_name + " " + employee1.last_name,
                                //Year_of_experience = (decimal)employee.year_of_experience,
                                //pan_no = employee.pan_no,
                                //bank_account_no = employee.bank_account_no,
                                blood_group = employee.blood_group,
                                //designation_id = designation.id,
                                //designation = designation.name,
                                emergency_contact_no = employee.emergency_contact_no,
                                emergency_contact_person = employee.emergency_contact_person,
                                medical_insurance_no = employee.medical_insurance_no,
                                //PF_no = employee.PF_no,
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
                datacontent.Dispose();
            }
        }

        public static int GetLastAddedEmployee()
        {
            EMSEntities datacontext = new EMSEntities();
            try
            {
                var query = from emp in datacontext.Employees 
                            join user in datacontext.Users on emp.user_id equals user.id 
                            where user.is_active == 1 orderby emp.created_on descending
                            select emp.id;
                var id = query.FirstOrDefault();
                return id;
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