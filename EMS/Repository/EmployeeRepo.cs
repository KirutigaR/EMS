using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Diagnostics;
using System.Linq;
using System.Web;

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
            catch(Exception e)
            {
                Debug.WriteLine(e.Message);
                Debug.WriteLine(e.GetBaseException());
                throw e;
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
            catch(Exception e)
            {
                Debug.WriteLine(e.Message);
                Debug.WriteLine(e.GetBaseException());
            }
            finally
            {
                datacontent.Dispose();
            }
        }
        public static List<Employee> GetEmployeeList()
        {
            EMSEntities datacontent = new EMSEntities();
            try
            {
                var query = from x in datacontent.Employees
                            select x;
                return query.ToList();
            }
            catch(Exception e)
            {
                Debug.WriteLine(e.Message);
                Debug.WriteLine(e.GetBaseException());
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
            catch(Exception e)
            {
                Debug.WriteLine(e.Message);
                Debug.WriteLine(e.GetBaseException());
                return null;
            }
            finally
            {
                datacontent.Dispose();
            }
        }
    }
}