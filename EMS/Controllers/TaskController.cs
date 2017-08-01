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
        [Route("api/task/list")]
        public HttpResponseMessage GetTaskList()
        {
            HttpResponseMessage response = null;
            try
            {
                List<TaskList> tasklist = TaskRepo.GetTaskList();
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
    }
}
