using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Web;

namespace EMS.Repository
{
    public class RoleRepo
    {
        public static List<Role> GetRoleList()
        {
            EMSEntities datacontext = new EMSEntities();
            try
            {
                var query = from roles in datacontext.Roles
                            select roles;
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
                datacontext.Dispose();
            }
        }
    }
}
