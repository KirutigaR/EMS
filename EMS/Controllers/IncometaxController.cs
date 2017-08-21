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
    public class IncometaxController : ApiController
    {
        public HttpResponseMessage AddNewTaxDeclaration(Incometax incometax)
        {
            HttpResponseMessage response = null;
            try
            {
                if(incometax!=null)
                {
                    IncometaxRepo.AddNewTaxDeclaration(incometax);
                    response = Request.CreateResponse(HttpStatusCode.OK, new EMSResponseMessage("EMS_001", "Success", "Tax declaration added Successfully"));
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
