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
        //[Route("api/ApplyLeave")]
        //[HttpPost]
        //public HttpResponseMessage ApplyLeave(int id, string leave_type = "", DateTime? from_date = null, DateTime? to_date = null)
        //{
        //    HttpResponseMessage response = null;
        //    try
        //    {
        //        Employee employee = EmployeeRepo.GetEmployeeById(id);
        //        string gender = employee.gender;
        //        if (from_date < DateTime.Now || to_date < DateTime.Now || to_date < from_date)
        //        {
        //            //ViewData["Date"] = "Your date is past date. so please select valid Date";
        //            response = Request.CreateResponse(HttpStatusCode.OK, "Your date is past date. so please select valid Date");
        //        }
        //        else
        //        {
        //            if(gender == "male")
        //            {

        //            }
        //        }
        //    }
        //}
    }
}
