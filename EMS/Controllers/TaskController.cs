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
    public class TaskController : ApiController
    {
        [Route("api/task/list/{p_id?}")]
        public HttpResponseMessage GetTaskList(int p_id=0)
        {
            HttpResponseMessage response = null;
            try
            {
                if(p_id!=0)
                {
                    List<TaskModel> tasklist = TaskRepo.GetTaskList(p_id);
                    response = Request.CreateResponse(HttpStatusCode.OK, new EMSResponseMessage("EMS_001", "Success", tasklist));
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

        [Route("api/task/add")]
        public HttpResponseMessage AssignProjectTask(List<Task> task)
        {
            HttpResponseMessage response = null;
            try
            {
                if (task != null)
                {
                    foreach(Task items in task)
                    {
                        Project proj_details = ProjectRepo.GetProjectById(items.project_id);
                        if (proj_details != null)
                        {
                            TaskRepo.AssignProjectTask(items);
                            response = Request.CreateResponse(HttpStatusCode.OK, new EMSResponseMessage("EMS_001", "Success", "Project tasks assigned successfully"));
                        }
                        else
                        {
                            response = Request.CreateResponse(HttpStatusCode.OK, new EMSResponseMessage("EMS_104", "Failure", "Project ID doesnot Exists"));
                        }
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

        [Route("api/task/edit")]
        public HttpResponseMessage EditTask(Task task)
        {
            HttpResponseMessage response = null;
            try
            {
                if (task != null)
                {
                    if (TaskRepo.GetTaskById(task.id) != null)
                    {
                        TaskRepo.EditTask(task);
                        response = Request.CreateResponse(HttpStatusCode.OK, new EMSResponseMessage("EMS_001", "Success", "Project tasks updated successfully"));
                    }
                    else
                    {
                        response = Request.CreateResponse(HttpStatusCode.OK, new EMSResponseMessage("EMS_104", "Failure", "Task doesnot Exists"));
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

        [Route("api/get/task/{t_id?}")]
        public HttpResponseMessage GetTaskById(int t_id)
        {
            HttpResponseMessage response = null;
            try
            {
                if(t_id!=0)
                {
                    TaskModel instance = TaskRepo.GetTaskDetailsById(t_id);
                    if (instance != null)
                    {
                        response = Request.CreateResponse(HttpStatusCode.OK, new EMSResponseMessage("EMS_001", "Success", instance));
                    }
                    else
                    {
                        response = Request.CreateResponse(HttpStatusCode.OK, new EMSResponseMessage("EMS_104", "Error", "Task doesnot exists"));
                    }
                }
                else
                {
                    response = Request.CreateResponse(HttpStatusCode.OK, new EMSResponseMessage("EMS_106", "URL Error", "Please check the input and datatype"));
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
        [Route("api/task/delete/{t_id}")]
        public HttpResponseMessage DeleteTask(int t_id)
        {
            HttpResponseMessage response = null;
            try
            {
                if(t_id!=0)
                {
                    Task instance = TaskRepo.GetTaskById(t_id);
                    if (instance != null)
                    {
                        TaskRepo.DeleteTaskById(instance);
                        response = Request.CreateResponse(HttpStatusCode.OK, new EMSResponseMessage("EMS_001", "Success", "Task deleted!"));
                    }
                    else
                    {
                        response = Request.CreateResponse(HttpStatusCode.OK, new EMSResponseMessage("EMS_104", "Error", "Task doesnot exists"));
                    }
                }
                else
                {
                    response = Request.CreateResponse(HttpStatusCode.OK, new EMSResponseMessage("EMS_106", "URL Error", "Please check the input and datatype"));
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
