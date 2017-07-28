using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using EMS.Repository;

namespace EMS.Controllers
{
    public class ProjectController : ApiController
    {
        [Route("api/CreateNewClient")]
        public HttpResponseMessage CreateNewClient()
        {
            HttpResponseMessage Response = null;
            try
            {
                ProjectRepo.CreateNewClient();
            }
            catch (Exception e)
            {
                Debug.WriteLine(e.Message);
                Debug.WriteLine(e.GetBaseException());
            }
            return Response;
        }
        [Route("api/CreateNewProject")]
        public HttpResponseMessage CreateNewProject()
        {
            HttpResponseMessage Response = null;
            try
            {
                ProjectRepo.CreateNewProject();
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
