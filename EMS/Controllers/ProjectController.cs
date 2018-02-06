using System;
using System.Collections;
using System.Collections.Generic;
using System.Data.Entity.Validation;
using System.Diagnostics;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using EMS.Models;
using EMS.Repository;
using EMS.Utility;

namespace EMS.Controllers
{
    public class ProjectController : ApiController
    {
        
        [HttpPost]
        [Route("api/v2/project/create")]
        public HttpResponseMessage CreateNewProject(Project project)
        {
            HttpResponseMessage Response = null;
            try
            {
                if (project != null)
                {
                    Project existingInstance = ProjectRepo.GetExistingProject(project.project_name, project.client_id);
                    if (existingInstance == null)
                    {
                        ProjectRepo.CreateNewProject(project);
                        Response = Request.CreateResponse(HttpStatusCode.OK, new EMSResponseMessage("EMS_001", "Project added successfully", "Project added successfully"));                    
                    }
                    else
                    {
                        Response = Request.CreateResponse(HttpStatusCode.OK, new EMSResponseMessage("EMS_203", "Project ID already exists", "Project ID already exists"));
                    }
                }
                else
                {
                    Response = Request.CreateResponse(HttpStatusCode.OK, new EMSResponseMessage("EMS_102", "Please check the Json input", "Please check the Json input"));
                }
            }
            catch (DbEntityValidationException DBexception)
            {
                Debug.WriteLine(DBexception.Message);
                Debug.WriteLine(DBexception.GetBaseException());
                Response = Request.CreateResponse(HttpStatusCode.OK, new EMSResponseMessage("EMS_190", "Some Mandatory fields are missing", DBexception.Message));
            }
            catch (Exception exception)
            {
                Debug.WriteLine(exception.Message);
                Debug.WriteLine(exception.GetBaseException());
                Response = Request.CreateResponse(HttpStatusCode.OK, new EMSResponseMessage("EMS_101", "Application Error", exception.Message));
            }
            return Response;
        }

        [Route("api/v2/project/list/{client_id?}/{status?}")]//project list of active client 
        public HttpResponseMessage GetProjectList(int client_id = 0, string status = null)//c_id client_id , status project_status
        {
            HttpResponseMessage response = null;
            try
            {
                List<ProjectModel> Project_List = ProjectRepo.GetProjectList(client_id, status);
                response = Request.CreateResponse(HttpStatusCode.OK, new EMSResponseMessage("EMS_001", "Success", Project_List));
            }
            catch (Exception exception)
            {
                Debug.WriteLine(exception.Message);
                Debug.WriteLine(exception.GetBaseException());
                response = Request.CreateResponse(HttpStatusCode.OK, new EMSResponseMessage("EMS_101", "Application Error", exception.Message));
            }
            return response;
        }

        [Route("api/v2/project/list/{employee_id?}")]//project list of active client 
        public HttpResponseMessage GetProjectListByEmployee(int employee_id)//c_id client_id , status project_status
        {
            HttpResponseMessage response = null;
            try
            {
                List<ProjectModel> Project_List = ProjectRepo.GetProjectListByEmployee(employee_id);
                response = Request.CreateResponse(HttpStatusCode.OK, new EMSResponseMessage("EMS_001", "Success", Project_List));
            }
            catch (Exception exception)
            {
                Debug.WriteLine(exception.Message);
                Debug.WriteLine(exception.GetBaseException());
                response = Request.CreateResponse(HttpStatusCode.OK, new EMSResponseMessage("EMS_101", "Application Error", exception.Message));
            }
            return response;
        }

        [Route("api/v2/entire/project/list/{client_id?}/{status?}")]//entire project list (include active and incative client projects )
        public HttpResponseMessage GetEntireProjectList(int client_id = 0, string status = null)//c_id client_id , status project_status
        {
            HttpResponseMessage response = null;
            try
            {
                List<ProjectModel> Project_List = ProjectRepo.GetEntireProjectList(client_id, status);
                response = Request.CreateResponse(HttpStatusCode.OK, new EMSResponseMessage("EMS_001", "Success", Project_List));
            }
            catch (Exception exception)
            {
                Debug.WriteLine(exception.Message);
                Debug.WriteLine(exception.GetBaseException());
                response = Request.CreateResponse(HttpStatusCode.OK, new EMSResponseMessage("EMS_101", "Application Error", exception.Message));
            }
            return response;
        }

        [Route("api/v2/get/project/{project_id}")]
        public HttpResponseMessage GetProjectById(int project_id)//p_id project_id
        {
            HttpResponseMessage response = null;
            try
            {
                if (project_id != 0)
                {
                    ProjectModel existinginstance = ProjectRepo.GetProjectDetailsById(project_id);
                    if (existinginstance != null)
                    {
                        response = Request.CreateResponse(HttpStatusCode.OK, new EMSResponseMessage("EMS_001", "Success", existinginstance));
                    }
                    else
                    {
                        response = Request.CreateResponse(HttpStatusCode.OK, new EMSResponseMessage("EMS_204", "Project ID doesnot exists", "Project ID doesnot exists"));
                    }
                }
                else
                {
                    response = Request.CreateResponse(HttpStatusCode.OK, new EMSResponseMessage("EMS_102", "Invalid Input", "Please check input Json"));
                }
            }
            catch (Exception exception)
            {
                Debug.WriteLine(exception.Message);
                Debug.WriteLine(exception.GetBaseException());
                response = Request.CreateResponse(HttpStatusCode.OK, new EMSResponseMessage("EMS_101", "Application Error", exception.Message));
            }
            return response;
        }

        [HttpPost]
        [Route("api/v2/project/edit")]
        public HttpResponseMessage EditProjectDetails(Project project)
        {
            HttpResponseMessage response = null;
            try
            {
                if (project != null)
                {
                    Project existingInstance = ProjectRepo.GetProjectById(project.id);
                    if (existingInstance != null)
                    {
                        ProjectRepo.EditProject(project);
                        response = Request.CreateResponse(HttpStatusCode.OK, new EMSResponseMessage("EMS_001", "Project details Updated successfully!", "Project details Updated successfully!"));
                    }
                    else
                    {
                        response = Request.CreateResponse(HttpStatusCode.OK, new EMSResponseMessage("EMS_204", "Project ID doesnot exists", "Invalid Project ID"));
                    }
                }
                else
                {
                    response = Request.CreateResponse(HttpStatusCode.OK, new EMSResponseMessage("EMS_102", "Invalid Input", "Please check input Json"));
                }
            }
            catch (Exception exception)
            {
                Debug.WriteLine(exception.Message);
                Debug.WriteLine(exception.GetBaseException());
                response = Request.CreateResponse(HttpStatusCode.OK, new EMSResponseMessage("EMS_101", "Application Error", exception.Message));
            }
            return response;
        }

        [HttpPost]
        [Route("api/v2/assign/project/role")]
        public HttpResponseMessage AssignEmployeeProjectRole(List<Project_role> project_roles)
        {
            HttpResponseMessage response = null;
            try
            {
                if (project_roles != null)
                {
                    ProjectRepo.ProjectRoleAssignment(project_roles);
                    response = Request.CreateResponse(HttpStatusCode.OK, new EMSResponseMessage("EMS_001", "Project Role Assigned to the Employee Succesfully", "Project Role Assigned to the Employee Succesfully"));
                }

                else
                {
                    response = Request.CreateResponse(HttpStatusCode.OK, new EMSResponseMessage("EMS_102", "Please check input Json, Project details and employee status", "Please check input Json, Project details and employee status"));
                }
            }
            catch (Exception exception)
            {
                Debug.WriteLine(exception.Message);
                Debug.WriteLine(exception.GetBaseException());
                response = Request.CreateResponse(HttpStatusCode.OK, new EMSResponseMessage("EMS_101", "Application Error", exception.Message));
            }
            return response;
        }

        [Route("api/v2/project/managerlist")]
        //project manager assignment , Team leader and manager list 
        public HttpResponseMessage GetProjectManagerList()
        {
            HttpResponseMessage response = null;
            try
            {
                List<ReportingTo> manager_list = ProjectRepo.GetProjectManagerList();
                response = Request.CreateResponse(HttpStatusCode.OK, new EMSResponseMessage("EMS_001", "Success", manager_list));
            }
            catch (Exception exception)
            {
                Debug.WriteLine(exception.Message);
                Debug.WriteLine(exception.GetBaseException());
                response = Request.CreateResponse(HttpStatusCode.OK, new EMSResponseMessage("EMS_101", "Application Error", exception.Message));
            }
            return response;
        }

        [Route("api/v2/assigned/project_role/list/{employee_id?}/{project_id?}/{reportingto_id?}")]
        public HttpResponseMessage GetAssignedProjectRoleList(int employee_id = 0, int project_id = 0, int reportingto_id = 0)//e_id employee_id , p_id project_id
        {
            HttpResponseMessage response = null;
            try
            {
                List<Project_role_model> project_role_list = ProjectRepo.GetAssignedProjectRoleList(employee_id, project_id, reportingto_id);
                response = Request.CreateResponse(HttpStatusCode.OK, new EMSResponseMessage("EMS_001", "Success", project_role_list));
            }
            catch (Exception exception)
            {
                Debug.WriteLine(exception.Message);
                Debug.WriteLine(exception.GetBaseException());
                response = Request.CreateResponse(HttpStatusCode.OK, new EMSResponseMessage("EMS_101", "Application Error", exception.Message));
            }
            return response;
        }

        [Route("api/v2/edit/employee/project/role")]
        public HttpResponseMessage EditEmployeeProjectRoleAssignmnet(Project_role project_roles)
        {
            HttpResponseMessage response = null;
            try
            {
                if (project_roles != null)
                {
                    ProjectRepo.EditEmployeeProjectRoleAssignmnet(project_roles);
                    response = Request.CreateResponse(HttpStatusCode.OK, new EMSResponseMessage("EMS_001", "Employee details assigned to the given project has been changed", "Employee details assigned to the given project has been changed"));
                }

                else
                {
                    response = Request.CreateResponse(HttpStatusCode.OK, new EMSResponseMessage("EMS_102", "Please check input Json, Project details and employee status", "Please check input Json, Project details and employee status"));
                }
            }
            catch (Exception exception)
            {
                Debug.WriteLine(exception.Message);
                Debug.WriteLine(exception.GetBaseException());
                response = Request.CreateResponse(HttpStatusCode.OK, new EMSResponseMessage("EMS_101", "Application Error", exception.Message));
            }
            return response;
        }

        [Route("api/v2/get/employee/project/role/{employee_id?}/{project_id?}/{role_id?}")]
        public HttpResponseMessage GetAssignedEmployeebyid(int employee_id, int project_id, int role_id)
        {
            HttpResponseMessage response = null;
            try
            {
                if (employee_id != 0 && project_id !=0 && role_id !=0)
                {
                    Project_role project_role = ProjectRepo.GetAssignedEmployeebyid(employee_id, project_id, role_id);
                    if(project_role!= null)
                    {
                        response = Request.CreateResponse(HttpStatusCode.OK, new EMSResponseMessage("EMS_001", "Success", project_role));
                    }
                    else
                    {
                        response = Request.CreateResponse(HttpStatusCode.OK, new EMSResponseMessage("EMS_001", "failure : Given employee is not specifically assigned to the role in that project", "Given employee is not specifically assigned to the role in that project"));
                    }
                }
                else
                {
                    response = Request.CreateResponse(HttpStatusCode.OK, new EMSResponseMessage("EMS_102", "Please check input Json, Project details and employee status", "Please check input Json, Project details and employee status"));
                }
            }
            catch (Exception exception)
            {
                Debug.WriteLine(exception.Message);
                Debug.WriteLine(exception.GetBaseException());
                response = Request.CreateResponse(HttpStatusCode.OK, new EMSResponseMessage("EMS_101", "Application Error", exception.Message));
            }
            return response;
        }

        [HttpGet]
        [Route("api/v2/delete/employee/project/role/{employee_id?}/{project_id?}/{role_id?}")]
        public HttpResponseMessage DeleteEmployeeProjectRoleAssignmnet(int employee_id, int project_id, int role_id)
        {
            HttpResponseMessage response = null;
            try
            {
                if(employee_id!=0 && project_id!=0 && role_id!=0)
                {
                    Project_role project_role = ProjectRepo.GetAssignedEmployeebyid(employee_id, project_id, role_id);
                    if (project_role != null)
                    {
                        ProjectRepo.DeleteEmployeeProjectRoleAssignmnet(project_role);
                        response = Request.CreateResponse(HttpStatusCode.OK, new EMSResponseMessage("EMS_001", "Employee deleted from the given project", "Employee deleted from the given project"));
                    }
                    else
                    {
                        response = Request.CreateResponse(HttpStatusCode.OK, new EMSResponseMessage("EMS_001", "Failure : check employe id, role id and project id", "Failure : check employe id, role id and project id"));
                    }
                }
                else
                {
                    response = Request.CreateResponse(HttpStatusCode.OK, new EMSResponseMessage("EMS_102", "Invalid Input", "Please check input Json, Project details and employee status"));
                }
            }
            catch (Exception exception)
            {
                Debug.WriteLine(exception.Message);
                Debug.WriteLine(exception.GetBaseException());
                response = Request.CreateResponse(HttpStatusCode.OK, new EMSResponseMessage("EMS_101", "Application Error", exception.Message));
            }
            return response;
        }

        [Route("api/v2/employee/project/list/{reporting_id?}")]//same as api/v2/project_role/list/{reportingto_id?}
        public HttpResponseMessage GetEmpProjDetailsByManager(int reporting_id)
        {
            HttpResponseMessage response = null;
            try
            {
                if(reporting_id!=0)
                {
                    if (EmployeeRepo.GetEmployeeById(reporting_id) != null)
                    {
                        ArrayList emp_prj_list = new ArrayList();
                        List<EmployeeModel> Emp_List = EmployeeRepo.GetEmployeeList(reporting_id, 0);
                        foreach (EmployeeModel items in Emp_List)
                        {
                            List<Project_role_model> proj_list = ProjectRepo.GetAssignedProjectRoleList(items.id,0,0);
                            emp_prj_list.Add(proj_list);
                        }
                        // List<Project_role_model> proj_list = ProjectRepo.EmpProjDetailsByManager(manager_id);
                        response = Request.CreateResponse(HttpStatusCode.OK, new EMSResponseMessage("EMS_001", "Success", emp_prj_list));
                    }
                    else
                    {
                        response = Request.CreateResponse(HttpStatusCode.OK, new EMSResponseMessage("EMS_403", "Invalid employee ID", "Invalid employee ID"));
                    }
                }
                else
                {
                    response = Request.CreateResponse(HttpStatusCode.OK, new EMSResponseMessage("EMS_102", "Invalid Input", "Please check input Json"));
                }
            }
            catch (Exception exception)
            {
                Debug.WriteLine(exception.Message);
                Debug.WriteLine(exception.GetBaseException());
                response = Request.CreateResponse(HttpStatusCode.OK, new EMSResponseMessage("EMS_101", "Application Error", exception.Message));
            }
            return response;
        }
    }
}
