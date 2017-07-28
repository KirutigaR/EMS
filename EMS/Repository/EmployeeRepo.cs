using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Diagnostics;
using System.Linq;
using System.Web;
using EMS.Models;

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
        public static List<EmployeeModel> GetEmployeeList()
        {
            EMSEntities datacontent = new EMSEntities();
            try
            {
                var query = from x in datacontent.Employees
                            select new EmployeeModel
                            {
                                id = x.id,
                                first_name = x.first_name,
                                last_name = x.last_name,
                                email = x.email,
                                date_of_birth = x.date_of_birth,
                                date_of_joining = x.date_of_joining,
                                contact_no = x.contact_no,
                                user_id = x.user_id, 
                                reporting_to = x.reporting_to,
                                Year_of_experence = x.Year_of_experence
                            };
                return query.ToList();
            }
            catch(Exception exception)
            {
                Debug.WriteLine(exception.Message);
                Debug.WriteLine(exception.GetBaseException());
                return null;
            }
            finally
            {
                datacontent.Dispose();
            }
        }
        public static EmployeeModel GetEmployeeDetailsById(int e_id)
        {
            EMSEntities datacontent = new EMSEntities();
            try
            {
                var query = from x in datacontent.Employees
                            where x.id == e_id
                            select new EmployeeModel
                            {
                                id = x.id,
                                first_name = x.first_name,
                                last_name = x.last_name,
                                email = x.email,
                                date_of_birth = x.date_of_birth,
                                date_of_joining = x.date_of_joining,
                                contact_no = x.contact_no,
                                user_id = x.user_id, 
                                reporting_to = x.reporting_to,
                                Year_of_experence = x.Year_of_experence
                            };
                return query.FirstOrDefault();
            }
            catch(Exception exception)
            {
                Debug.WriteLine(exception.Message);
                Debug.WriteLine(exception.GetBaseException());
                return null;
            }
            finally
            {
                datacontent.Dispose();
            }
        }

        public static Employee GetEmployeeById(int e_id)
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
                return null;
            }
            finally
            {
                datacontent.Dispose();
            }
        }

    }
}