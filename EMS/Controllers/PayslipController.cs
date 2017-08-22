using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using EMS.Utility;

namespace EMS.Controllers
{
    public class PayslipController : ApiController
    {
        public HttpResponseMessage GeneratePayslip(int e_id)//e_id employee_id
        {
            HttpResponseMessage response = null;
            try
            {
                if(e_id != 0 )
                {

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
