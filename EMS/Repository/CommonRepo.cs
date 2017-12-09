using EMS.Models;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Diagnostics;
using System.Linq;
using System.Web;

namespace EMS.Repository
{
    public class CommonRepo
    {
        public static bool Login(User user)
        {
            EMSEntities datacontext = new EMSEntities();
            try
            {
                var query = from u in datacontext.Users
                            where u.user_name == user.user_name && u.password == user.password && u.is_active == 1
                            select u;
                if (query.ToList().Count > 0)
                    return true;
                else
                    return false;
            }
            catch (Exception e)
            {
                Debug.WriteLine(e.Message);
                Debug.WriteLine(e.GetBaseException());
                return false;
            }
            finally
            {
                datacontext.Dispose();
            }
        }
        public static Role GetUserRole(int id)
        {
            EMSEntities datacontext = new EMSEntities();
            try
            {
                var query = from r in datacontext.Roles
                            join ur in datacontext.User_role on r.id equals ur.role_id
                            join u in datacontext.Users on ur.user_id equals u.id
                            where u.id == id && r.role_type == "System Role"
                            select r;
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
        public static int GetUserID(User user)
        {
            EMSEntities datacontext = new EMSEntities();
            try
            {
                var query = from u in datacontext.Users
                            where u.user_name == user.user_name && u.password == user.password
                            select u.id;
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
        public static int GetEmployeeIdByUserid(int id)
        {
            EMSEntities datacontext = new EMSEntities();
            try
            {
                var query = from e in datacontext.Employees
                            where e.user_id == id
                            select e.id;
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
        
        public static User GetuserById(int user_id)
        {
            EMSEntities datacontext = new EMSEntities();
            try
            {
                var query = from u in datacontext.Users
                            where u.id == user_id
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
        public static void EditUserDetails(User user)
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
        public static bool LoadDataFromTable()
        {
            EMSEntities datacontext = new EMSEntities();
            try
            {
                var query = datacontext.UpdateLeaveBalance();
                return true;
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