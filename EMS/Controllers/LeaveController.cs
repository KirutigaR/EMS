﻿using EMS.Repository;
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
        [Route("api/holiday/list")]
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
        [Route("api/editholidaylist")]
        [HttpPost]
        public HttpResponseMessage EditHolidayList(Holiday_List holiday)
        {
            HttpResponseMessage response = null;
            try
            {
                Holiday_List holiday_list = LeaveRepo.GetHolidayById(holiday.id);
                if(holiday_list != null)
                {
                    //holiday.id = holiday_list.id;
                    LeaveRepo.EditHolidayList(holiday);
                    response = Request.CreateResponse(HttpStatusCode.OK, "Holiday List successfully updated");

                }
                else
                {
                    response = Request.CreateResponse(HttpStatusCode.OK, "updated failed");
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
        [Route("api/holiday/delete")]
        [HttpPost]
        public HttpResponseMessage DeleteHolidayList(Holiday_List holiday_list)
        {
            HttpResponseMessage response = null;
            try
            {
                Holiday_List holiday = LeaveRepo.GetHolidayById(holiday_list.id);
                if (holiday != null)
                {
                    LeaveRepo.DeleteHolidayList(holiday_list);
                    response = Request.CreateResponse(HttpStatusCode.OK, "Holiday successfully deleted");
                }
                else
                {
                    response = Request.CreateResponse(HttpStatusCode.OK, "deleted failed");
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
        [HttpPost]
        [Route("api/holiday/create")]
        public HttpResponseMessage CreateHoliday(Holiday_List holiday_list)
        {
            HttpResponseMessage response = null;
            try
            {               
                if (holiday_list != null)
                {
                    LeaveRepo.CreateHoliday(holiday_list);
                    response = Request.CreateResponse(HttpStatusCode.OK, "Holiday successfully added");
                }
                else
                {
                    response = Request.CreateResponse(HttpStatusCode.OK, "Create failed");
                }
            }
            catch(Exception exception)
            {
                Debug.WriteLine(exception.Message);
                Debug.WriteLine(exception.GetBaseException());
                response = Request.CreateResponse(HttpStatusCode.OK, exception.Message);
            }
            return response;
        }

        [HttpPost]
        [Route("api/apply/leave")]
        public HttpResponseMessage ApplyLeave(Leave leave)
        {
            HttpResponseMessage response = null;
            try
            {
                Employee employee_instance = EmployeeRepo.GetEmployeeById(leave.employee_id);
                string gender = employee_instance.gender;
                
                if (leave.from_date < DateTime.Now || (leave.to_date < DateTime.Now && leave.to_date != DateTime.MinValue) || (leave.to_date < leave.from_date && leave.to_date != DateTime.MinValue))
                {
                    response = Request.CreateResponse(HttpStatusCode.OK, "Your date is past date. so please select valid Date");
                }
                else 
                {
                    Leavebalance_sheet leave_balance = new Leavebalance_sheet();
                    string leave_type1 = LeaveRepo.GetLeaveTypeById(leave.leavetype_id);
                    if (leave_type1 == "ML")
                    {
                        int leave_type_id = LeaveRepo.GetLeavetypeIdByLeavetype(leave_type1);
                        decimal Ml_leave_type = LeaveRepo.GetNoofDaysById(leave_type_id);
                        if (Ml_leave_type != 0)
                        {
                            DateTime MLnoofdays = ((DateTime)leave.from_date).AddDays(182);
                            //leave.maternity_leave = 0;
                            leave.no_of_days = 182;
                            leave.to_date = MLnoofdays;
                            LeaveRepo.EditLeaveHistory(leave);
                            //LMSRepo.EditEmployeeLeaveByApplyLeave(leave.employee_id, LeaveType, FromDate, MLnoofdays, 182);
                            //ViewData["MlMessage"] = "Leave successfully Applied";
                            response = Request.CreateResponse(HttpStatusCode.OK, "Leave successfully Applied");
                        }
                        else
                        {
                            //ViewData["MLError"] = "you already applied Maternity Leave";
                            response = Request.CreateResponse(HttpStatusCode.OK, "you already applied Maternity Leave");
                        }
                    }
                    else if (leave.leavetype_id != 0 && leave.from_date != null && leave.to_date != null)
                    {
                        if (gender == "male")
                        {
                            List<DateTime> holiday = LeaveRepo.GetDateFromHoliday();
                            decimal noofdays = (decimal)Utils.DaysLeft(leave.from_date, leave.to_date, true, holiday);
                            string leave_type = LeaveRepo.GetLeaveTypeById(leave.leavetype_id);
                            Leave leave_instance = new Leave();
                            if (leave_type == "CL")
                            {
                                int leave_type_id = LeaveRepo.GetLeavetypeIdByLeavetype(leave_type);
                                decimal Cl_leave_type = LeaveRepo.GetNoofDaysById(leave_type_id);
                                if (noofdays > 3)
                                {

                                    response = Request.CreateResponse(HttpStatusCode.OK, "CL can't be more than three days");
                                }
                                else if (noofdays <= 3)
                                {
                                    if (Cl_leave_type < noofdays)
                                    {

                                        response = Request.CreateResponse(HttpStatusCode.OK, "Applied Leave more than Casual Leave. Choose Another LeaveType");
                                    }
                                    else if (Cl_leave_type >= noofdays)
                                    {
                                        Cl_leave_type = Cl_leave_type - noofdays;
                                        //leave.LOP = 0;
                                        leave.no_of_days = (int)noofdays;
                                        LeaveRepo.EditLeaveHistory(leave);
                                        if (noofdays != 0)
                                        {

                                            response = Request.CreateResponse(HttpStatusCode.OK, "Leave successfully Applied");
                                        }
                                    }
                                }
                            }
                            else if (leave_type == "EL")
                            {
                                int leave_type_id = LeaveRepo.GetLeavetypeIdByLeavetype(leave_type);
                                decimal LOP_leave_type_id = LeaveRepo.GetLeavetypeIdByLeavetype("LOP");
                                decimal El_leave_type = LeaveRepo.GetNoofDaysById(leave_type_id);
                                if (El_leave_type < noofdays)
                                {
                                    El_leave_type = noofdays - El_leave_type;
                                    LOP_leave_type_id = Math.Abs(LOP_leave_type_id + El_leave_type);
                                    El_leave_type = 0;
                                    leave.no_of_days = (int)noofdays;
                                    LeaveRepo.EditLeaveHistory(leave);
                                }
                                else if (El_leave_type >= noofdays)
                                {
                                    El_leave_type = El_leave_type - noofdays;
                                    LOP_leave_type_id = 0;
                                    leave.no_of_days = (int)noofdays;
                                    LeaveRepo.EditLeaveHistory(leave);
                                    if (noofdays != 0)
                                    {

                                        response = Request.CreateResponse(HttpStatusCode.OK, "Leave successfully Applied");
                                    }


                                }
                            }
                            else if (leave_type == "LOP")
                            {
                                int leave_type_id = LeaveRepo.GetLeavetypeIdByLeavetype(leave_type);
                                decimal LOP_leave_type_id = LeaveRepo.GetLeavetypeIdByLeavetype("LOP");
                                decimal Cl_leave_type = LeaveRepo.GetNoofDaysById(leave_type_id);
                                decimal El_leave_type = LeaveRepo.GetNoofDaysById(leave_type_id);
                                if (Cl_leave_type == 0 && El_leave_type == 0)
                                {
                                    LOP_leave_type_id = LOP_leave_type_id + noofdays;
                                    leave.no_of_days = (int)noofdays;
                                    LeaveRepo.EditLeaveHistory(leave);
                                    if (noofdays != 0)
                                    {

                                        response = Request.CreateResponse(HttpStatusCode.OK, "Leave successfully Applied");
                                    }
                                }
                                else if (Cl_leave_type == 0 && El_leave_type > 0)
                                {

                                    response = Request.CreateResponse(HttpStatusCode.OK, "you can apply the Leave in 'EL'");
                                }
                                else if (El_leave_type == 0 && Cl_leave_type > 0)
                                {

                                    response = Request.CreateResponse(HttpStatusCode.OK, "you can apply the Leave in 'CL'");
                                }
                                else if (Cl_leave_type > 0 && El_leave_type > 0)
                                {

                                    response = Request.CreateResponse(HttpStatusCode.OK, "You have CL, EL leave balance");
                                }
                            }
                            else if (leave_type == "WFH")
                            {
                                int leave_type_id = LeaveRepo.GetLeavetypeIdByLeavetype(leave_type);
                                decimal WFH_leave_type = LeaveRepo.GetNoofDaysById(leave_type_id);
                                if (noofdays <= 2)
                                {
                                    leave.no_of_days = (int)noofdays;
                                    LeaveRepo.EditLeaveHistory(leave);
                                    if (noofdays != 0)
                                    {

                                        response = Request.CreateResponse(HttpStatusCode.OK, "Leave successfully Applied");
                                    }
                                    else
                                    {

                                        response = Request.CreateResponse(HttpStatusCode.OK, "applied leave on holiday date");
                                    }
                                }
                                else if (noofdays > 2)
                                {

                                    response = Request.CreateResponse(HttpStatusCode.OK, "you can apply only two days");
                                }
                            }
                        }
                        else if (gender == "female")
                        {
                            List<DateTime> holiday = LeaveRepo.GetDateFromHoliday();
                            decimal noofdays = (decimal)Utils.DaysLeft(leave.from_date, leave.to_date, true, holiday);
                            string leave_type = LeaveRepo.GetLeaveTypeById(leave.leavetype_id);
                            Leave leave_instance = new Leave();
                            if (leave_type == "CL")
                            {
                                int leave_type_id = LeaveRepo.GetLeavetypeIdByLeavetype(leave_type);
                                decimal Cl_leave_type = LeaveRepo.GetNoofDaysById(leave_type_id);
                                if (noofdays > 3)
                                {

                                    response = Request.CreateResponse(HttpStatusCode.OK, "CL can't be more than three days");
                                }
                                else if (noofdays <= 3)
                                {
                                    if (Cl_leave_type < noofdays)
                                    {
                                        response = Request.CreateResponse(HttpStatusCode.OK, "Applied Leave more than Casual Leave. Choose Another LeaveType");
                                    }
                                    else if (Cl_leave_type >= noofdays)
                                    {
                                        Cl_leave_type = Cl_leave_type - noofdays;
                                        //leave.LOP = 0;
                                        leave.no_of_days = (int)noofdays;
                                        LeaveRepo.EditLeaveHistory(leave);
                                        if (noofdays != 0)
                                        {

                                            response = Request.CreateResponse(HttpStatusCode.OK, "Leave successfully Applied");
                                        }
                                    }
                                }
                            }
                            else if (leave_type == "EL")
                            {
                                int leave_type_id = LeaveRepo.GetLeavetypeIdByLeavetype(leave_type);
                                decimal LOP_leave_type_id = LeaveRepo.GetLeavetypeIdByLeavetype("LOP");
                                decimal El_leave_type = LeaveRepo.GetNoofDaysById(leave_type_id);
                                if (El_leave_type < noofdays)
                                {
                                    El_leave_type = noofdays - El_leave_type;
                                    LOP_leave_type_id = Math.Abs(LOP_leave_type_id + El_leave_type);
                                    El_leave_type = 0;
                                    leave.no_of_days = (int)noofdays;
                                    LeaveRepo.EditLeaveHistory(leave);
                                }
                                else if (El_leave_type >= noofdays)
                                {
                                    El_leave_type = El_leave_type - noofdays;
                                    LOP_leave_type_id = 0;
                                    leave.no_of_days = (int)noofdays;
                                    LeaveRepo.EditLeaveHistory(leave);
                                    if (noofdays != 0)
                                    {

                                        response = Request.CreateResponse(HttpStatusCode.OK, "Leave successfully Applied");
                                    }


                                }
                            }
                            else if (leave_type == "LOP")
                            {
                                int leave_type_id = LeaveRepo.GetLeavetypeIdByLeavetype(leave_type);
                                decimal LOP_leave_type_id = LeaveRepo.GetLeavetypeIdByLeavetype("LOP");
                                decimal Cl_leave_type = LeaveRepo.GetNoofDaysById(leave_type_id);
                                decimal El_leave_type = LeaveRepo.GetNoofDaysById(leave_type_id);
                                if (Cl_leave_type == 0 && El_leave_type == 0)
                                {
                                    LOP_leave_type_id = LOP_leave_type_id + noofdays;
                                    leave.no_of_days = (int)noofdays;
                                    LeaveRepo.EditLeaveHistory(leave);
                                    if (noofdays != 0)
                                    {

                                        response = Request.CreateResponse(HttpStatusCode.OK, "Leave successfully Applied");
                                    }
                                }
                                else if (Cl_leave_type == 0 && El_leave_type > 0)
                                {

                                    response = Request.CreateResponse(HttpStatusCode.OK, "you can apply the Leave in 'EL'");
                                }
                                else if (El_leave_type == 0 && Cl_leave_type > 0)
                                {

                                    response = Request.CreateResponse(HttpStatusCode.OK, "you can apply the Leave in 'CL'");
                                }
                                else if (Cl_leave_type > 0 && El_leave_type > 0)
                                {

                                    response = Request.CreateResponse(HttpStatusCode.OK, "You have CL, EL leave balance");
                                }
                            }
                            else if (leave_type == "WFH")
                            {
                                int leave_type_id = LeaveRepo.GetLeavetypeIdByLeavetype(leave_type);
                                decimal WFH_leave_type = LeaveRepo.GetNoofDaysById(leave_type_id);
                                if (noofdays <= 2)
                                {
                                    leave.no_of_days = (int)noofdays;
                                    LeaveRepo.EditLeaveHistory(leave);
                                    if (noofdays != 0)
                                    {

                                        response = Request.CreateResponse(HttpStatusCode.OK, "Leave successfully Applied" + leave);
                                    }
                                    else
                                    {

                                        response = Request.CreateResponse(HttpStatusCode.OK, "applied leave on holiday date");
                                    }
                                }
                                else if (noofdays > 2)
                                {

                                    response = Request.CreateResponse(HttpStatusCode.OK, "you can apply only two days");
                                }
                            }

                        }

                    }
                    else
                    {
                        response = Request.CreateResponse(HttpStatusCode.OK, "dont leave the fields");
                    }
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
        [Route("api/approval")]
        [HttpPost]
        public HttpResponseMessage Approval(bool b, int employee_id)
        {
            HttpResponseMessage response = null;
            try
            {
                if(b == true)
                {                   
                    Leave leave = LeaveRepo.GetLeaveById(employee_id);
                    leave.is_approved = 1;
                    LeaveRepo.ApproveLeave(leave);
                    response = Request.CreateResponse(HttpStatusCode.OK, "Leave Approved");
                }
                else
                {
                    response = Request.CreateResponse(HttpStatusCode.OK, "Leave cancel");
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
