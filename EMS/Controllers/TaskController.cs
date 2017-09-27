using System;
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
    public class TaskController : ApiController
    {
        [Route("api/task/list/{project_id?}")]
        public HttpResponseMessage GetTaskList(int project_id = 0)//p_id project_id
        {
            HttpResponseMessage response = null;
            try
            {
                List<TaskModel> tasklist = TaskRepo.GetTaskList(project_id);
                response = Request.CreateResponse(HttpStatusCode.OK, new EMSResponseMessage("EMS_001", "Success", tasklist));
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
                            response = Request.CreateResponse(HttpStatusCode.OK, new EMSResponseMessage("EMS_001", "Success : Project tasks assigned successfully", "Project tasks assigned successfully"));
                        }
                        else
                        {
                            response = Request.CreateResponse(HttpStatusCode.OK, new EMSResponseMessage("EMS_204", "Failure : Project ID doesnot Exists", "Project ID doesnot Exists"));
                        }
                    }
                }
                else
                {
                    response = Request.CreateResponse(HttpStatusCode.OK, new EMSResponseMessage("EMS_102", "Invalid Input", "Please check input Json"));
                }
            }
            catch (DbEntityValidationException DBexception)
            {
                Debug.WriteLine(DBexception.Message);
                Debug.WriteLine(DBexception.GetBaseException());
                response = Request.CreateResponse(HttpStatusCode.OK, new EMSResponseMessage("EMS_190", "Mandatory fields missing", DBexception.Message));
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
                        response = Request.CreateResponse(HttpStatusCode.OK, new EMSResponseMessage("EMS_801", "Failure : Task doesnot Exists", "Task doesnot Exists"));
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

        [Route("api/get/task/{task_id?}")]
        public HttpResponseMessage GetTaskById(int task_id)//t_id task_id
        {
            HttpResponseMessage response = null;
            try
            {
                if(task_id != 0)
                {
                    TaskModel instance = TaskRepo.GetTaskDetailsById(task_id);
                    if (instance != null)
                    {
                        response = Request.CreateResponse(HttpStatusCode.OK, new EMSResponseMessage("EMS_001", "Success", instance));
                    }
                    else
                    {
                        response = Request.CreateResponse(HttpStatusCode.OK, new EMSResponseMessage("EMS_801", "Error : Task doesnot exists", "Task doesnot exists"));
                    }
                }
                else
                {
                    response = Request.CreateResponse(HttpStatusCode.OK, new EMSResponseMessage("EMS_802", "URL Error : Please check the input and datatype", "Please check the input and datatype"));
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
        [Route("api/task/delete/{task_id}")]
        public HttpResponseMessage DeleteTask(int task_id)//t_id task_id
        {
            HttpResponseMessage response = null;
            try
            {
                if(task_id != 0)
                {
                    Task instance = TaskRepo.GetTaskById(task_id);
                    if (instance != null)
                    {
                        TaskRepo.DeleteTaskById(instance);
                        response = Request.CreateResponse(HttpStatusCode.OK, new EMSResponseMessage("EMS_001", "Success", "Task deleted!"));
                    }
                    else
                    {
                        response = Request.CreateResponse(HttpStatusCode.OK, new EMSResponseMessage("EMS_801", "Error : Task doesnot exists", "Task doesnot exists"));
                    }
                }
                else
                {
                    response = Request.CreateResponse(HttpStatusCode.OK, new EMSResponseMessage("EMS_802", "URL Error : Please check the input and datatype", "Please check the input and datatype"));
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
