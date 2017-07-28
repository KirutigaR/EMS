using EMS.Repository;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;

namespace EMS.Controllers
{
    public class LeaveController : ApiController
    {
        [Route("api/holidaylist")]
        public HttpResponseMessage HolidayList()
        {
            HttpResponseMessage response = null;
            try
            {
                List<Holiday_List> holidayList = LeaveRepo.GetHoliday();
                response = Request.CreateResponse(HttpStatusCode.OK, holidayList);
            }
            catch (Exception exception)
            {
                Debug.WriteLine(exception.Message);
                Debug.WriteLine(exception.GetBaseException());
                response = Request.CreateResponse(HttpStatusCode.OK, exception.Message);
            }
            return response;
        }
        [Route("api/ApplyLeave")]
        public HttpResponseMessage ApplyLeave()
        {
            HttpResponseMessage response = null;
            try
            {
                Leave_type leave_type = new Leave_type();

            }
        }
    }
}
