using EMS.Repository;
using EMS.Utility;
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
        [HttpPost]
        public HttpResponseMessage ApplyLeave(int id, string leave_type = "", DateTime? from_date = null, DateTime? to_date = null)
        {
            HttpResponseMessage response = null;
            try
            {
                Employee employee = EmployeeRepo.GetEmployeeById(id);
                string gender = employee.gender;
                Leavebalance_sheet leave_balance = new Leavebalance_sheet();
                if(leave_type != "" && from_date != null && to_date != null)
                {
                    if (from_date < DateTime.Now || to_date < DateTime.Now || to_date < from_date)
                    {
                        //ViewData["Date"] = "Your date is past date. so please select valid Date";
                        response = Request.CreateResponse(HttpStatusCode.OK, "Your date is past date. so please select valid Date");
                    }
                    else
                    {
                        if (gender == "male")
                        {
                            List<DateTime> holiday = LeaveRepo.GetDateFromHoliday();
                            decimal noofdays = (decimal)Utils.DaysLeft(from_date, to_date, true, holiday);
                            Leave leave = new Leave();
                            if (leave_type == "CL")
                            {
                                int leave_type_id = LeaveRepo.GetLeavetypeIdByLeavetype(leave_type);
                                decimal Cl_leave_type = LeaveRepo.GetNoofDaysById(leave_type_id);
                                if (noofdays > 3)
                                {
                                    //ViewData["CLmorethanthreedays"] = "CL can't be more than three days";
                                    response = Request.CreateResponse(HttpStatusCode.OK, "CL can't be more than three days");
                                }
                                else if (noofdays <= 3)
                                {
                                    if (Cl_leave_type < noofdays)
                                    {
                                        //ViewData["CLErrorMessage"] = "Applied Leave more than Casual Leave. Choose Another LeaveType";
                                        response = Request.CreateResponse(HttpStatusCode.OK, "Applied Leave more than Casual Leave. Choose Another LeaveType");
                                    }
                                    else if (Cl_leave_type >= noofdays)
                                    {
                                        Cl_leave_type = Cl_leave_type - noofdays;
                                        //leave.LOP = 0;
                                        LeaveRepo.EditLeaveHistory(leave);
                                        //if (noofdays != 0)
                                        //{
                                        //    LeaveRepo.EditEmployeeLeaveByApplyLeave(leave.employee_id, leave_type, from_date, to_date, noofdays);
                                        //    //ViewData["ClMessage"] = "Leave successfully Applied";
                                        //}                                        
                                    }
                                }
                            }
                            
                        }
                    }
                }
                else
                {
                    response = Request.CreateResponse(HttpStatusCode.OK, "Dont Leave the fields");
                }
                
            }
            catch (Exception exception)
            {
                Debug.WriteLine(exception.Message);
                Debug.WriteLine(exception.GetBaseException());
                response = Request.CreateResponse(HttpStatusCode.OK, exception.Message);
            }
            return response;
        }
    }
}
