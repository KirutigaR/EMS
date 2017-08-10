﻿using EMS.Models;
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
        [Route("api/holiday/list")]
        public HttpResponseMessage GetHolidayList()
        {
            HttpResponseMessage response = null;
            try
            {
                List<Holiday_List> holidayList = LeaveRepo.GetHoliday();
                response = Request.CreateResponse(HttpStatusCode.OK, new EMSResponseMessage("EMS_001", "Success", holidayList));
            }
            catch (Exception exception)
            {
                Debug.WriteLine(exception.Message);
                Debug.WriteLine(exception.GetBaseException());
                response = Request.CreateResponse(HttpStatusCode.OK, new EMSResponseMessage("EMS_101", "Application Error", exception.Message));
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
                    response = Request.CreateResponse(HttpStatusCode.OK, new EMSResponseMessage("EMS_001", "Success", "Holiday List successfully updated"));

                }
                else
                {
                    response = Request.CreateResponse(HttpStatusCode.OK, new EMSResponseMessage("EMS_105", "Invalid holiday id", "updated failed"));
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
        [Route("api/holiday/delete/{h_id?}")]
        [HttpGet]
        public HttpResponseMessage DeleteHolidayList(int h_id)
        {
            HttpResponseMessage response = null;
            try
            {
                Holiday_List holiday = LeaveRepo.GetHolidayById(h_id);
                if (holiday != null)
                {
                    LeaveRepo.DeleteHolidayList(holiday);
                    response = Request.CreateResponse(HttpStatusCode.OK, new EMSResponseMessage("EMS_001", "Success", "Holiday successfully deleted"));
                }
                else
                {
                    response = Request.CreateResponse(HttpStatusCode.OK, new EMSResponseMessage("EMS_105", "Invalid holiday id", "updated failed"));
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
                    response = Request.CreateResponse(HttpStatusCode.OK, new EMSResponseMessage("EMS_001", "Success", "Holiday successfully created"));
                }
                else
                {
                    response = Request.CreateResponse(HttpStatusCode.OK, new EMSResponseMessage("EMS_102", "Invalid Input", "Please check input Json"));
                }
            }
            catch(Exception exception)
            {
                Debug.WriteLine(exception.Message);
                Debug.WriteLine(exception.GetBaseException());
                response = Request.CreateResponse(HttpStatusCode.OK, new EMSResponseMessage("EMS_101", "Application Error", exception.Message));
            }
            return response;
        }

        [HttpPost]
        [Route("api/applyleave")]
        public HttpResponseMessage ApplyLeave(Leave leave)
        {
            HttpResponseMessage response = null;
            try
            {
                Employee employee_instance = EmployeeRepo.GetEmployeeById(leave.employee_id);
                string gender = employee_instance.gender;

                if (leave.from_date < DateTime.Now || (leave.to_date < DateTime.Now && leave.to_date != DateTime.MinValue) || (leave.to_date < leave.from_date && leave.to_date != DateTime.MinValue))
                {
                    response = Request.CreateResponse(HttpStatusCode.OK, new EMSResponseMessage("EMS_106", "invalid date", "Your date is past date. so please select valid Date"));
                }
                else
                {
                    Leavebalance_sheet leave_balance = new Leavebalance_sheet();
                    string leave_type1 = LeaveRepo.GetLeaveTypeById(leave.leavetype_id);
                    if (gender == "female" && leave_type1 == "ML")
                    {
                        //if (leave_type1 == "ML")
                        //{
                        int leave_type_id = LeaveRepo.GetLeavetypeIdByLeavetype(leave_type1);
                        decimal? Ml_leave_type = LeaveRepo.GetNoofDaysById(leave_type_id);
                        if (Ml_leave_type != 0)
                        {
                            DateTime MLnoofdays = ((DateTime)leave.from_date).AddDays(182);
                            //leave.maternity_leave = 0;
                            leave.no_of_days = 182;
                            leave.to_date = MLnoofdays;
                            LeaveRepo.EditLeaveHistory(leave);
                            //LMSRepo.EditEmployeeLeaveByApplyLeave(leave.employee_id, LeaveType, FromDate, MLnoofdays, 182);
                            //ViewData["MlMessage"] = "Leave successfully Applied";
                            response = Request.CreateResponse(HttpStatusCode.OK, new EMSResponseMessage("EMS_001", "Success", "Leave successfully Applied"));
                        }
                        else
                        {
                            //ViewData["MLError"] = "you already applied Maternity Leave";
                            response = Request.CreateResponse(HttpStatusCode.OK, new EMSResponseMessage("EMS_107", "Already applied", "you already applied Maternity Leave"));
                        }
                        //}
                    }
                    else if (leave.leavetype_id != 0 && leave.from_date != null && leave.to_date != null)
                    {
                        List<DateTime> holiday = LeaveRepo.GetDateFromHoliday();
                        decimal noofdays = (decimal)Utils.DaysLeft(leave.from_date, leave.to_date, true, holiday);
                        string leave_type = LeaveRepo.GetLeaveTypeById(leave.leavetype_id);
                        Leave leave_instance = new Leave();
                        if (leave_type == "CL")
                        {
                            int leave_type_id = LeaveRepo.GetLeavetypeIdByLeavetype(leave_type);
                            decimal? Cl_leave_type = LeaveRepo.GetNoofDaysById(leave_type_id);
                            if (noofdays > 3)
                            {

                                response = Request.CreateResponse(HttpStatusCode.OK, new EMSResponseMessage("EMS_108", "Can apply only 3 CL", "CL can't be more than three days"));
                            }
                            else if (noofdays <= 3)
                            {
                                if (Cl_leave_type < noofdays)
                                {

                                    response = Request.CreateResponse(HttpStatusCode.OK, new EMSResponseMessage("EMS_109", "casual leave less than applied leave", "Applied Leave more than Casual Leave. Choose Another LeaveType"));
                                }
                                else if (Cl_leave_type >= noofdays)
                                {
                                    Cl_leave_type = Cl_leave_type - noofdays;
                                    //leave.LOP = 0;
                                    leave.no_of_days = (int)noofdays;
                                    LeaveRepo.EditLeaveHistory(leave);
                                    if (noofdays != 0)
                                    {

                                        response = Request.CreateResponse(HttpStatusCode.OK, new EMSResponseMessage("EMS_001", "Success", "Leave successfully Applied"));
                                    }
                                }
                            }
                        }
                        else if (leave_type == "EL")
                        {
                            int leave_type_id = LeaveRepo.GetLeavetypeIdByLeavetype(leave_type);
                            decimal LOP_leave_type_id = LeaveRepo.GetLeavetypeIdByLeavetype("LOP");
                            decimal? El_leave_type = LeaveRepo.GetNoofDaysById(leave_type_id);
                            if (El_leave_type < noofdays)
                            {
                                El_leave_type = noofdays - El_leave_type;
                                LOP_leave_type_id = Math.Abs(LOP_leave_type_id + (decimal)El_leave_type);
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

                                    response = Request.CreateResponse(HttpStatusCode.OK, new EMSResponseMessage("EMS_001", "Success", "Leave successfully Applied"));
                                }


                            }
                        }
                        else if (leave_type == "LOP")
                        {
                            int leave_type_id = LeaveRepo.GetLeavetypeIdByLeavetype(leave_type);
                            decimal LOP_leave_type_id = LeaveRepo.GetLeavetypeIdByLeavetype("LOP");
                            decimal? Cl_leave_type = LeaveRepo.GetNoofDaysById(leave_type_id);
                            decimal? El_leave_type = LeaveRepo.GetNoofDaysById(leave_type_id);
                            if (Cl_leave_type == 0 && El_leave_type == 0)
                            {
                                LOP_leave_type_id = LOP_leave_type_id + noofdays;
                                leave.no_of_days = (int)noofdays;
                                LeaveRepo.EditLeaveHistory(leave);
                                if (noofdays != 0)
                                {

                                    response = Request.CreateResponse(HttpStatusCode.OK, new EMSResponseMessage("EMS_001", "Success", "Leave successfully Applied"));
                                }
                            }
                            else if (Cl_leave_type == 0 && El_leave_type > 0)
                            {

                                response = Request.CreateResponse(HttpStatusCode.OK, new EMSResponseMessage("EMS_110", "You have EL leave", "you can apply the Leave in 'EL'"));
                            }
                            else if (El_leave_type == 0 && Cl_leave_type > 0)
                            {

                                response = Request.CreateResponse(HttpStatusCode.OK, new EMSResponseMessage("EMS_111", "You have CL leave", "you can apply the Leave in 'CL'"));
                            }
                            else if (Cl_leave_type > 0 && El_leave_type > 0)
                            {

                                response = Request.CreateResponse(HttpStatusCode.OK, new EMSResponseMessage("EMS_111", "You have CL, EL leave", "You have CL, EL leave balance"));
                            }
                        }
                        else if (leave_type == "WFH")
                        {
                            int leave_type_id = LeaveRepo.GetLeavetypeIdByLeavetype(leave_type);
                            decimal? WFH_leave_type = LeaveRepo.GetNoofDaysById(leave_type_id);
                            if (noofdays <= 2)
                            {
                                leave.no_of_days = (int)noofdays;
                                LeaveRepo.EditLeaveHistory(leave);
                                if (noofdays != 0)
                                {

                                    response = Request.CreateResponse(HttpStatusCode.OK, new EMSResponseMessage("EMS_001", "Success", "Leave successfully Applied"));
                                }
                                else
                                {

                                    response = Request.CreateResponse(HttpStatusCode.OK, new EMSResponseMessage("EMS_112", "your leave date already applied", "applied leave on holiday date"));
                                }
                            }
                            else if (noofdays > 2)
                            {

                                response = Request.CreateResponse(HttpStatusCode.OK, new EMSResponseMessage("EMS_113", "allow only 2 days for WFH", "you can apply only two days"));
                            }
                        }


                    }
                    else
                    {
                        response = Request.CreateResponse(HttpStatusCode.OK, new EMSResponseMessage("EMS_114", "dont leave the fields", "dont leave the fields"));
                    }
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
        //[HttpPost]
        [Route("api/leavehistory/id/{e_id?}")] // display Leavehistory in leavepage by id
        public HttpResponseMessage GetLeaveHistoryById(int e_id)
        {
            HttpResponseMessage response = null;
            try
            {
                List<LeavehistoryModel> leave_history_model = LeaveRepo.GetLeaveHistoryById(e_id);

                response = Request.CreateResponse(HttpStatusCode.OK, new EMSResponseMessage("EMS_119", "approved", leave_history_model));


            }
            catch (Exception exception)
            {
                Debug.WriteLine(exception.Message);
                Debug.WriteLine(exception.GetBaseException());
                response = Request.CreateResponse(HttpStatusCode.OK, new EMSResponseMessage("EMS_101", "Application Error", exception.Message));
            }
            return response;
        }
        //[Route("api/leavepage/holidaylist")] //display holiday in apply leave page
        //public HttpResponseMessage GetHolidayListById()
        //{
        //    HttpResponseMessage response = null;
        //    try
        //    {
        //        List<Holiday_List> holiday_list = LeaveRepo.GetHolidayList();
        //        response = Request.CreateResponse(HttpStatusCode.OK, new EMSResponseMessage("EMS_001", "Success", holiday_list));

        //    }
        //    catch (Exception exception)
        //    {
        //        Debug.WriteLine(exception.Message);
        //        Debug.WriteLine(exception.GetBaseException());
        //        response = Request.CreateResponse(HttpStatusCode.OK, new EMSResponseMessage("EMS_101", "Application Error", exception.Message));
        //    }
        //    return response;
        //}
        //[HttpPost]
        [Route("api/balanceleave/{e_id?}")] //balance CL EL Ml Lop leave in applyleave page
        public HttpResponseMessage GetLeavePageBalanceLeave(int e_id)
        {
            HttpResponseMessage response = null;
            try
            {
                List<LeaveBalanceModel> leavebalance = LeaveRepo.GetLeaveBalanceById(e_id);
                //LeaveBalanceModel leavebalance = LeaveRepo.GetLeaveBalanceById(leave.employee_id);
                //Leave_type leave_type = new Leave_type();
                //leave_type.type_name = 
                //lbs.leavetype_id = LeaveRepo.GetLeaveBalanceByLeaveId()
                response = Request.CreateResponse(HttpStatusCode.OK, new EMSResponseMessage("EMS_001", "Success", leavebalance));
            }
            catch (Exception exception)
            {
                Debug.WriteLine(exception.Message);
                Debug.WriteLine(exception.GetBaseException());
                response = Request.CreateResponse(HttpStatusCode.OK, new EMSResponseMessage("EMS_101", "Application Error", exception.Message));
            }
            return response;
        }
        [Route("api/changepassword")]
        [HttpPost]
        public HttpResponseMessage ChangePassword(Employee employee, string oldapassword, string new_password, string confirm_password)
        {
            HttpResponseMessage response = null;
            try
            {
                int user_id = LeaveRepo.GetUserIdById(employee.id);
                User user_instance = LeaveRepo.GetUserById(user_id);
                if (new_password == confirm_password)
                {
                    if (oldapassword == user_instance.password)
                    {
                        user_instance.password = new_password;
                        LeaveRepo.EditUserPassword(user_instance);
                        response = Request.CreateResponse(HttpStatusCode.OK, new EMSResponseMessage("EMS_001", "Success", "Password sucessfully changed"));
                    }
                    else
                    {
                        response = Request.CreateResponse(HttpStatusCode.OK, new EMSResponseMessage("EMS_115", "password not matched", "password mismatch"));
                    }

                }
                else
                {
                    response = Request.CreateResponse(HttpStatusCode.OK, new EMSResponseMessage("EMS_116", "confirm password not match", "new password and confirm password doesnot match"));
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
        //[HttpPost]
        [Route("api/approval/{leave_id?}/{is_approved?}")]
        public HttpResponseMessage GetApproval(int leave_id, int is_approved)
        {
            HttpResponseMessage response = null;
            try
            {
                if (is_approved == 1)
                {
                    //LeaveRepo.GetLeaveById(leave.employee_id, leave.id);
                    //Leavebalance_sheet leave_balance = new Leavebalance_sheet();
                    Leave leave1 = LeaveRepo.GetLeaveById(leave_id);
                    leave1.leave_statusid = Constants.LEAVE_STATUS_APPROVED;
                    LeaveRepo.ApproveLeave(leave1);
                    Leavebalance_sheet leave_balance_instance = LeaveRepo.LeaveBalanceById(leave1.employee_id);

                    decimal? no_of_days = LeaveRepo.GetNoofdaysByLeaveTypeId(leave1.leavetype_id);
                    leave_balance_instance.no_of_days = (decimal)no_of_days - leave1.no_of_days;
                    LeaveRepo.UpdateLeaveBalanceSheet(leave_balance_instance);
                    response = Request.CreateResponse(HttpStatusCode.OK, new EMSResponseMessage("EMS_001", "Success", "Leave Approved"));
                }
                else if (is_approved == 0)
                {
                    response = Request.CreateResponse(HttpStatusCode.OK, new EMSResponseMessage("EMS_117", "Leave not approved", "Leave cancel"));
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
        [Route("api/employeelist/byrole/{e_id?}")]// Get employee list by hr/project manager id
        [HttpGet]
        public HttpResponseMessage EmployeeListByRole(int e_id)
        {
            HttpResponseMessage response = null;
            try
            {
                //Project_role project_role = ProjectRepo.GetProjectIdRoleId(e_id);
                //string project_name = ProjectRepo.GetProjectName(project_role.project_id);
                //string role_name = ProjectRepo.GetProjectRole(project_role.role_id);
                List<EmployeeListByRoleModel> employee_list_byrole = LeaveRepo.GetEmployeeListByRole(e_id);
                //List<EmployeeListByRoleModel> emp_proj_details_byrole = LeaveRepo.emp_proj_details_byrole(employee_list_byrole);
                response = Request.CreateResponse(HttpStatusCode.OK, new EMSResponseMessage("EMS_001", "Success", employee_list_byrole));
            }
            catch (Exception exception)
            {
                Debug.WriteLine(exception.Message);
                Debug.WriteLine(exception.GetBaseException());
                response = Request.CreateResponse(HttpStatusCode.OK, new EMSResponseMessage("EMS_101", "Application Error", exception.Message));
            }
            return response;
        }
        //[HttpPost]
        [Route("api/leave/pending{e_id?}")] //leave request project manager page
        public HttpResponseMessage GetLeaveRequestByRole(int e_id)
        {
            HttpResponseMessage response = null;
            try
            {
                List<LeavehistoryModel> leave_history = LeaveRepo.GetRequestByRoleId(e_id);
                response = Request.CreateResponse(HttpStatusCode.OK, new EMSResponseMessage("EMS_001", "Success", leave_history));
            }
            catch (Exception exception)
            {
                Debug.WriteLine(exception.Message);
                Debug.WriteLine(exception.GetBaseException());
                response = Request.CreateResponse(HttpStatusCode.OK, new EMSResponseMessage("EMS_101", "Application Error", exception.Message));
            }
            return response;
        }
        [Route("api/pendingapproval/hr")] // pending approval in hr manager
        public HttpResponseMessage GetPendingApproval()
        {
            HttpResponseMessage response = null;
            try
            {
                //Project_role project_role = ProjectRepo.GetProjectIdRoleId(employee.id);
                //string role_name = ProjectRepo.GetRoleNameById(project_role.role_id);
                //if (role_name == "HR")
                //{
                    List<LeavehistoryModel> leave_history = LeaveRepo.GetPendingLeave();
                    response = Request.CreateResponse(HttpStatusCode.OK, new EMSResponseMessage("EMS_001", "Success", leave_history));
                //}

            }
            catch (Exception exception)
            {
                Debug.WriteLine(exception.Message);
                Debug.WriteLine(exception.GetBaseException());
                response = Request.CreateResponse(HttpStatusCode.OK, new EMSResponseMessage("EMS_101", "Application Error", exception.Message));
            }
            return response;
        }
        [Route("api/typesofleaves/list")]
        public HttpResponseMessage GetLeaveTypesList()
        {
            HttpResponseMessage response = null;
            try
            {
                List<LeaveTypeListModel> leave_type_list = LeaveRepo.GetLeaveTypeList();
                response = Request.CreateResponse(HttpStatusCode.OK, new EMSResponseMessage("EMS_001", "Success", leave_type_list));
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
