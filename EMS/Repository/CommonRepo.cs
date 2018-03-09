using EMS.Models;
using System;
using System.Data.Entity;
using System.Diagnostics;
using System.Linq;

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
        public static int GetUserIDByUserName(User user)
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
        public static Employee GetEmployeeIdByMailid(string MailID)
        {
            EMSEntities datacontext = new EMSEntities();
            try
            {
                var query = from e in datacontext.Employees
                            where e.email == MailID
                            select e;
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
        
        public static User GetuserByUserId(int user_id)
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

        public static bool AddUserToken(Password_Token Forgot_Password)
        {
            EMSEntities datacontext = new EMSEntities();
            try
            {
                datacontext.Password_Token.Add(Forgot_Password);
                datacontext.SaveChanges();
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

        public static Password_Token GetActiveTokenObjectByToken (string Token)
        {
            EMSEntities datacontext = new EMSEntities();
            try
            {
                var query = from x in datacontext.Password_Token
                            where x.Token == Token /*&& x.Generated_on <= DateTime.Now && DbFunctions.AddDays(x.Generated_on, 1) >= DateTime.Now*/
                            select x;
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

        public static bool EditUserPassword(User user)
        {
            EMSEntities datacontext = new EMSEntities();
            try
            {
                datacontext.Entry(user).State = EntityState.Modified;
                datacontext.SaveChanges();
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

        public static void DeletePasswordToken(int User_id)
        {
            EMSEntities datacontext = new EMSEntities();
            try
            {
                datacontext.Password_Token.RemoveRange(datacontext.Password_Token.Where(x=>x.User_Id == User_id));
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
    }
}