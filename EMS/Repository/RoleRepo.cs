using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Web;
using EMS.Models;

namespace EMS.Repository
{
    public class RoleRepo
    {
        public static List<RoleModel> GetRoleList()
        {
            EMSEntities datacontext = new EMSEntities();
            try
            {
                var query = from roles in datacontext.Roles
                            select new RoleModel
                            {
                                id = roles.id,
                                role_name = roles.role_name,
                                role_description = roles.role_description,
                                role_type = roles.role_type
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
                datacontext.Dispose();
            }
        }
    }
}
