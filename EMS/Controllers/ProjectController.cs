using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using EMS.Repository;
using EMS.Utility;

namespace EMS.Controllers
{
    public class ProjectController : ApiController
    {
        [Route("api/create/client")]
        public HttpResponseMessage CreateNewClient(Client client)
        {
            HttpResponseMessage Response = null;
            try
            {
                if(client != null)
                {
                    ProjectRepo.CreateNewClient(client);
                    Response = Request.CreateResponse(HttpStatusCode.OK, new EMSResponseMessage("EMS_001", "Success", "Client added successfully"));
                }
                else
                {
                    Response = Request.CreateResponse(HttpStatusCode.OK, new EMSResponseMessage("EMS_102", "Failure", "Please check the Json input"));
                }
            }
            catch (Exception e)
            {
                Debug.WriteLine(e.Message);
                Debug.WriteLine(e.GetBaseException());
            }
            return Response;
        }
        [Route("api/create/project")]
        public HttpResponseMessage CreateNewProject(Project project)
        {
            HttpResponseMessage Response = null;
            try
            {
                if (project != null)
                {
                    ProjectRepo.CreateNewProject(project);
                    Response = Request.CreateResponse(HttpStatusCode.OK, new EMSResponseMessage("EMS_001", "Success", "Project added successfully"));
                }
                else
                {
                    Response = Request.CreateResponse(HttpStatusCode.OK, new EMSResponseMessage("EMS_102", "Failure", "Please check the Json input"));
                }
            }
            catch (Exception e)
            {
                Debug.WriteLine(e.Message);
                Debug.WriteLine(e.GetBaseException());
            }
            return Response;
        }
    }
}
