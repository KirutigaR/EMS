using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Diagnostics;
using System.Linq;
using System.Web;
using EMS.Models;
using EMS.Utility;

namespace EMS.Repository
{
	public class ProjectRepo
	{
        public static void CreateNewProject(Project project)
        {
            EMSEntities datacontext = new EMSEntities();
            try
            {
                datacontext.Projects.Add(project);
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

        public static Project GetProjectById(int p_id)
        {
            EMSEntities datacontext = new EMSEntities();
            try
            {
                var query = from x in datacontext.Projects
                            where x.id == p_id
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
                datacontext.Dispose();
            }
        }

        public static ProjectModel GetProjectDetailsById(int p_id)
        {
            EMSEntities datacontext = new EMSEntities();
            try
            {
                var query = from project in datacontext.Projects
                            where project.id == p_id
                            select new ProjectModel
                            {
                                project_name = project.project_name,
                                start_date = project.start_date,
                                end_date = project.end_date,
                                status = project.status,
                                po = project.po,
                                project_description = project.project_description,
                                client_id = (int)project.client_id
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

        public static List<ProjectModel> GetProjectList()
        {
            EMSEntities datacontext = new EMSEntities();
            try
            {
                var query = from project in datacontext.Projects
                            select new ProjectModel
                            {
                                project_name = project.project_name,
                                start_date = project.start_date,
                                end_date = project.end_date,
                                status = project.status,
                                po = project.po,
                                project_description = project.project_description,
                                client_id = project.client_id
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

        public static void EditProject(Project project)
        {
            EMSEntities datacontext = new EMSEntities();
            try
            {
                datacontext.Entry(project).State = EntityState.Modified;
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
       
        public static void ProjectRoleAssignment(Project_role prj_role)
        {
            EMSEntities datacontext = new EMSEntities();
            try
            {
                datacontext.Project_role.Add(prj_role);
                datacontext.SaveChanges();
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


        public static List<ReportingTo> GetProjectManagerList()
        {
            EMSEntities datacontext = new EMSEntities();
            try
            {
                var query = from x in datacontext.Employees
                            join y in datacontext.User_role
                            on x.user_id equals y.user_id
                            where y.role_id == Constants.Systemrole_Manager || y.role_id == Constants.Systemrole_TeamLeader || y.role_id == Constants.Projectrole_Manager
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