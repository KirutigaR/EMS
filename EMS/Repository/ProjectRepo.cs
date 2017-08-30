﻿using System;
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

        public static Project GetProjectById(int p_id)// p_id - project_id
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

        public static ProjectModel GetProjectDetailsById(int p_id)// p_id - project_id
        {
            EMSEntities datacontext = new EMSEntities();
            try
            {
                var query = from project in datacontext.Projects
                            join client in datacontext.Clients
                            on project.client_id equals client.id
                            where project.id == p_id
                            select new ProjectModel
                            {
                                project_id = project.id,
                                project_name = project.project_name,
                                start_date = project.start_date,
                                end_date = project.end_date,
                                status = project.status,
                                po = project.po,
                                project_description = project.project_description,
                                resources_req = project.resources_req,
                                client_id = (int)project.client_id,
                                client_name = client.client_name,
                                type_id = client.type_id
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

        public static List<ProjectModel> GetProjectList(int c_id, string status)//c_id Client Id , Status = Project status 
        {
            EMSEntities datacontext = new EMSEntities();
            try
            {
                var predicate = LinqKit.PredicateBuilder.True<Project>();
                if(c_id!=0)
                {
                    predicate = predicate.And(i => i.client_id == c_id);
                }
                if(status != null)
                {
                    predicate = predicate.And(i => i.status == status);
                }
              
                var query = from project in datacontext.Projects.AsExpandable().Where(predicate)
                            join client in datacontext.Clients
                            on project.client_id equals client.id
                            where client.is_active == 1 
                            select new ProjectModel
                            {
                                project_id = project.id,
                                project_name = project.project_name,
                                start_date = project.start_date,
                                end_date = project.end_date,
                                status = project.status,
                                po = project.po,
                                project_description = project.project_description,
                                client_id = project.client_id,
                                client_name = client.client_name,
                                type_id = client.type_id,
                                resources_req = project.resources_req
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

        public static List<ProjectModel> GetEntireProjectList(int c_id, string status)//c_id Client Id , Status = Project status 
        {
            EMSEntities datacontext = new EMSEntities();
            try
            {
                var predicate = LinqKit.PredicateBuilder.True<Project>();
                if (c_id != 0)
                {
                    predicate = predicate.And(i => i.client_id == c_id);
                }
                if (status != null)
                {
                    predicate = predicate.And(i => i.status == status);
                }

                var query = from project in datacontext.Projects.AsExpandable().Where(predicate)
                            join client in datacontext.Clients
                            on project.client_id equals client.id
                            select new ProjectModel
                            {
                                project_id = project.id,
                                project_name = project.project_name,
                                start_date = project.start_date,
                                end_date = project.end_date,
                                status = project.status,
                                po = project.po,
                                project_description = project.project_description,
                                client_id = project.client_id,
                                client_name = client.client_name,
                                type_id = client.type_id,
                                resources_req = project.resources_req
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

        public static List<Project_role_model> GetProjectListByEmployee(int e_id)//e_id employee_id
        {
            EMSEntities datacontext = new EMSEntities();
            try
            {
                var query = from x in datacontext.Project_role
                            join emp in datacontext.Employees
                            on x.employee_id equals emp.id
                            join proj in datacontext.Projects
                            on x.project_id equals proj.id
                            join role in datacontext.Roles
                            on x.role_id equals role.id
                            where x.employee_id == e_id
                            select new Project_role_model
                            {
                                id = x.id,
                                employee_name = emp.first_name,
                                project_name = proj.project_name,
                                role_name = role.role_name,
                                start_date = x.start_date,
                                end_date = x.end_date,
                                association = x.association
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
       
        public static void ProjectRoleAssignment(List<Project_role> project_roles)
        {
            EMSEntities datacontext = new EMSEntities();
            try
            {
                if (project_roles !=null)
                {
                    foreach (Project_role project_role in project_roles)
                    {
                        int empl_status = EmployeeRepo.GetEmployeeStatusById(project_role.employee_id);
                        ProjectModel proj = ProjectRepo.GetProjectDetailsById(project_role.project_id);
                        int actual_resource_count = proj.resources_req;
                        List<Project_role_model> assigned_resource_count = ProjectRepo.GetProjectRoleList(0, proj.project_id);
                        if ((empl_status == 1) && (proj != null) && (assigned_resource_count.Count < actual_resource_count) )
                        {
                            datacontext.Project_role.Add(project_role);
                            datacontext.SaveChanges();
                        }
                        else
                        {
                            throw new Exception("Check Project details , employee status and resource allocation count ");
                        }
                    }
                }
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
        public static string GetRoleNameById(int id)
        {
            EMSEntities datacontext = new EMSEntities();
            try
            {
                var query = from r in datacontext.Roles
                            where r.id == id && r.role_type == "Project Role"
                            select r.role_name;
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
        public static Project_role GetProjectIdRoleId(int id)
        {
            EMSEntities datacontext = new EMSEntities();
            try
            {
                var query = from pr in datacontext.Project_role
                            where pr.employee_id == id
                            select pr;
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
        public static string GetProjectName(int pro_id)
        {
            EMSEntities datacontext = new EMSEntities();
            try
            {
                var query = from p in datacontext.Projects
                            where p.id == pro_id
                            select p.project_name;
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
        public static string GetProjectRole(int role_id)
        {
            EMSEntities datacontext = new EMSEntities();
            try
            {
                var query = from r in datacontext.Roles
                            where r.id == role_id && r.role_type == "Project Role"
                            select r.role_name;
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
        public static List<Project_role_model> GetProjectRoleList(int e_id, int p_id)//e_id employee_id , p_id project_id
        {
            EMSEntities datacontext = new EMSEntities();
            try
            {
                var predicate = LinqKit.PredicateBuilder.True<Project_role>();
                if(e_id!=0)
                {
                    predicate = predicate.And(i => i.employee_id == e_id);
                }
                if (p_id != 0)
                {
                    predicate = predicate.And(i => i.project_id == p_id);
                }
                var query = from x in datacontext.Project_role.AsExpandable().Where(predicate)
                            join emp in datacontext.Employees
                            on x.employee_id equals emp.id
                            join proj in datacontext.Projects 
                            on x.project_id equals proj.id
                            join role in datacontext.Roles
                            on x.role_id equals role.id
                            select new Project_role_model
                            {
                                id = x.id,
                                employee_name = emp.first_name,
                                project_name = proj.project_name,
                                role_name = role.role_name,
                                start_date = x.start_date,
                                end_date = x.end_date,
                                association = x.association
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

        public static List<Project_role_model > EmpProjDetailsByManager (int m_id)//m_id manager_id
        {
            EMSEntities datacontext = new EMSEntities();
            try
            {
                var query = from x in datacontext.Employees
                            join prj_role in datacontext.Project_role
                            on x.id equals prj_role.employee_id
                            join proj in datacontext.Projects
                            on prj_role.project_id equals proj.id
                            join role in datacontext.Roles
                            on prj_role.role_id equals role.id
                            where x.reporting_to == m_id
                            select new Project_role_model
                            {
                                employee_name = x.first_name,
                                project_name = proj.project_name,
                                role_name = role.role_name,
                                association = prj_role.association,
                                start_date = prj_role.start_date,
                                end_date = prj_role.end_date,
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