using System;
using System.Collections.Generic;
using System.Data.Entity;
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

         public static List<RoleModel> GetSystemRoleList()
        {
            EMSEntities datacontext = new EMSEntities();
            try
            {
                var query = from roles in datacontext.Roles
                            where roles.role_type == "System Role"
                            select new RoleModel
                            {
                                id = roles.id,
                                role_name = roles.role_name,
                                role_description = roles.role_description,
                                role_type = roles.role_type
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

        public static List<RoleModel> GetProjectRoleList()
        {
            EMSEntities datacontext = new EMSEntities();
            try
            {
                var query = from roles in datacontext.Roles
                            where roles.role_type == "Project Role"
                            select new RoleModel
                            {
                                id = roles.id,
                                role_name = roles.role_name,
                                role_description = roles.role_description,
                                role_type = roles.role_type
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

        public static RoleModel GetRoleById(int role_id)
        {
            EMSEntities datacontext = new EMSEntities();
            try
            {
                var query = from role in datacontext.Roles
                            where role.id == role_id
                            select new RoleModel
                            {
                                id = role.id,
                                role_name = role.role_name,
                                role_type = role.role_type,
                                role_description = role.role_description
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

        public static RoleModel GetExistingRole(Role details)
        {
            EMSEntities datacontext = new EMSEntities();
            try
            {
                var query = from role in datacontext.Roles
                            where role.role_name == details.role_name && role.role_type == details.role_type
                            select new RoleModel
                            {
                                id = role.id,
                                role_name = role.role_name,
                                role_type = role.role_type,
                                role_description = role.role_description
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


        public static void CreateRole(Role details)
        {
            EMSEntities datacontext = new EMSEntities();
            try
            {
                datacontext.Roles.Add(details);
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

        public static void EditRole(Role details)
        {
            EMSEntities datacontext = new EMSEntities();
            try
            {
                datacontext.Entry(details).State = EntityState.Modified;
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

        public static void DeleteRoleById(int role_id)
        {
            EMSEntities datacontext = new EMSEntities();
            try
            {
                var query = from x in datacontext.Roles
                            where x.id == role_id
                            select x;

                datacontext.Entry(query.FirstOrDefault()).State = EntityState.Deleted;
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
    }
}
