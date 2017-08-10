using System;
using System.Collections.Generic;
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
        [Route("api/project/create")]
        public HttpResponseMessage CreateNewProject(Project project)
        {
            HttpResponseMessage Response = null;
            try
            {
                if (project != null)
                {
                    Project existingInstance = ProjectRepo.GetProjectById(project.id);
                    if (existingInstance == null)
                    {
                        ProjectRepo.CreateNewProject(project);
                        Response = Request.CreateResponse(HttpStatusCode.OK, new EMSResponseMessage("EMS_001", "Success", "Project added successfully"));                    
                    }
                    else
                    {
                        Response = Request.CreateResponse(HttpStatusCode.OK, new EMSResponseMessage("EMS_104", "Project ID already exists", "Project ID already exists"));
                    }
                }
                else
                {
                    Response = Request.CreateResponse(HttpStatusCode.OK, new EMSResponseMessage("EMS_102", "Failure", "Please check the Json input"));
                }
            }
            catch (Exception exception)
            {
                Debug.WriteLine(exception.Message);
                Debug.WriteLine(exception.GetBaseException());
                Response = Request.CreateResponse(HttpStatusCode.OK, new EMSResponseMessage("EMS_101", "Application Error", exception.Message));
            }
            return Response;
        }

        [Route("api/project/list/{c_id?}/{status?}")]
        public HttpResponseMessage GetProjectList(int c_id = 0, string status = null)
        {
            HttpResponseMessage response = null;
            try
            {
                List<ProjectModel> Project_List = ProjectRepo.GetProjectList(c_id, status);
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

        [Route("api/project/list/{e_id?}")]
        public HttpResponseMessage GetProjectListByEmployee(int e_id)
        {
            HttpResponseMessage response = null;
            try
            {
                List<ProjectModel> Project_List = ProjectRepo.GetProjectListByEmployee(e_id);
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

        [Route("api/get/project/{p_id}")]
        public HttpResponseMessage GetProjectById(int p_id)
        {
            HttpResponseMessage response = null;
            try
            {
                if (p_id != 0)
                {
                    ProjectModel existinginstance = ProjectRepo.GetProjectDetailsById(p_id);
                    if (existinginstance != null)
                    {
                        response = Request.CreateResponse(HttpStatusCode.OK, new EMSResponseMessage("EMS_001", "Success", existinginstance));
                    }
                    else
                    {
                        response = Request.CreateResponse(HttpStatusCode.OK, new EMSResponseMessage("EMS_103", "Project ID doesnot exists", "Project ID doesnot exists"));
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
        [Route("api/project/edit")]
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
                        response = Request.CreateResponse(HttpStatusCode.OK, new EMSResponseMessage("EMS_001", "Success", "Project details Updated successfully!"));
                    }
                    else
                    {
                        response = Request.CreateResponse(HttpStatusCode.OK, new EMSResponseMessage("EMS_103", "Invalid Project ID", "Invalid Project ID"));
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
        [Route("api/assign/project/role")]
        public HttpResponseMessage AssignEmployeeProjectRole(List<Project_role> project_roles)
        {
            HttpResponseMessage response = null;
            try
            {
                if (project_roles != null)
                {
                    ProjectRepo.ProjectRoleAssignment(project_roles);
                    response = Request.CreateResponse(HttpStatusCode.OK, new EMSResponseMessage("EMS_001", "Success", "Project Role Assigned to the Employee Succesfully"));
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

        [Route("api/project/managerlist")]
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

        [Route("api/project_role/list/{e_id?}/{p_id?}")]
        public HttpResponseMessage GetProjectRoleList(int e_id = 0, int p_id = 0)
        {
            HttpResponseMessage response = null;
            try
            {
                List<Project_role_model> project_role_list = ProjectRepo.GetProjectRoleList(e_id , p_id);
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
    }
}
