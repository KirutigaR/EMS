using EMS.Models;
using EMS.Repository;
using EMS.Utility;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using System.Data.Entity.Validation;
using System.Collections;

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
                if (holiday_list != null)
                {
                    //holiday.id = holiday_list.id;
                    LeaveRepo.EditHolidayList(holiday);
                    response = Request.CreateResponse(HttpStatusCode.OK, new EMSResponseMessage("EMS_001", "Success", "Holiday List successfully updated"));

                }
                else
                {
                    response = Request.CreateResponse(HttpStatusCode.OK, new EMSResponseMessage("EMS_501", "Invalid holiday id", "updated failed"));
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
        [Route("api/holiday/delete/{holiday_id?}")]
        [HttpGet]
        public HttpResponseMessage DeleteHolidayList(int holiday_id)
        {
            HttpResponseMessage response = null;
            try
            {
                Holiday_List holiday = LeaveRepo.GetHolidayById(holiday_id);
                if (holiday != null)
                {
                    LeaveRepo.DeleteHolidayList(holiday);
                    response = Request.CreateResponse(HttpStatusCode.OK, new EMSResponseMessage("EMS_001", "Success", "Holiday successfully deleted"));
                }
                else
                {
                    response = Request.CreateResponse(HttpStatusCode.OK, new EMSResponseMessage("EMS_501", "Invalid holiday id", "updated failed"));
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
            catch (DbEntityValidationException DBexception)
            {
                Debug.WriteLine(DBexception.Message);
                Debug.WriteLine(DBexception.GetBaseException());
                response = Request.CreateResponse(HttpStatusCode.OK, new EMSResponseMessage("EMS_190", "Mandatory fields missing", DBexception.Message));
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
        [Route("api/applyleave")]
        public HttpResponseMessage ApplyLeave(Leave leave)
        {
            HttpResponseMessage response = null;
            try
            {
                Employee employee_instance = EmployeeRepo.GetEmployeeById(leave.employee_id);
                string gender = employee_instance.gender;

                if (leave.from_date < DateTime.Now || leave.to_date < DateTime.Now || leave.from_date > leave.to_date)
                {
                    response = Request.CreateResponse(HttpStatusCode.OK, new EMSResponseMessage("EMS_502", "invalid date", "so please select valid Date"));
                }
                //else if(leave.to_date == DateTime.MinValue)
                //{
                //    response = Request.CreateResponse(HttpStatusCode.OK, new EMSResponseMessage("EMS_121", "", "Your date is past date or . so please select valid Date"));
                //}
                else
                {
                    Leavebalance_sheet leave_balance = new Leavebalance_sheet();
                    string leave_type1 = LeaveRepo.GetLeaveTypeById(leave.leavetype_id);
                    List<Leave> leavelist = LeaveRepo.GetActiveLeaveListByEmpId(leave.employee_id);
                    foreach (Leave leaveinstance in leavelist)
                    {
                        if (leave.from_date <= leaveinstance.from_date && leave.to_date >= leaveinstance.to_date)
                        {
                            response = Request.CreateResponse(HttpStatusCode.OK, new EMSResponseMessage("EMS_516", "leave application is already registered inbetween these days", "Error in leave application, check the dates"));
                            return response;
                        }
                        else if ((leaveinstance.from_date <= leave.from_date && leave.from_date <= leaveinstance.to_date) || (leaveinstance.from_date <= leave.to_date && leave.to_date <= leaveinstance.to_date))
                        {
                            response = Request.CreateResponse(HttpStatusCode.OK, new EMSResponseMessage("EMS_517", "Already a leave is applied in these days", "Error in leave application, check the dates"));
                            return response;
                        }
                    }
                    if (gender == "female" && leave_type1 == "ML")
                    {
                        //if (leave_type1 == "ML")
                        //{ 
                        //int leave_type_id = LeaveRepo.GetLeavetypeIdByLeavetype(leave_type1);
                        decimal Ml_leaves = LeaveRepo.GetNoofDaysById(leave.leavetype_id, leave.employee_id);
                        if (Ml_leaves != 0)
                        {
                            DateTime MLnoofdays = ((DateTime)leave.from_date).AddDays(182);
                            //leave.maternity_leave = 0;
                            leave.no_of_days = 182;
                            leave.to_date = MLnoofdays;
                            LeaveRepo.AddLeaveHistory(leave);
                            //LMSRepo.EditEmployeeLeaveByApplyLeave(leave.employee_id, LeaveType, FromDate, MLnoofdays, 182);
                            //ViewData["MlMessage"] = "Leave successfully Applied";
                            ReportingTo reporting_to = EmployeeRepo.GetReportingtoByEmpId(leave.employee_id);
                             
                            MailHandler.LeaveMailing(leave.from_date, leave.to_date, employee_instance.first_name, Constants.LEAVE_STATUS_PENDING, employee_instance.email, reporting_to.mailid, null);
                            response = Request.CreateResponse(HttpStatusCode.OK, new EMSResponseMessage("EMS_001", "Success", "Leave successfully Applied"));
                        }
                        else
                        {
                            //ViewData["MLError"] = "you already applied Maternity Leave";
                            response = Request.CreateResponse(HttpStatusCode.OK, new EMSResponseMessage("EMS_503", "Already applied", "you already applied Maternity Leave"));
                        }
                        //}
                    }
                    else if(gender == "male" && leave_type1 == "ML")
                    {
                        response = Request.CreateResponse(HttpStatusCode.OK, new EMSResponseMessage("EMS_504", "invalid leave type", "Invalid leave type"));
                    }
                    else if (leave.leavetype_id != 0 && leave.from_date != null && leave.to_date != null && leave.to_date != DateTime.MinValue && (leave.to_date > DateTime.Now) && leave.to_date >= leave.from_date)
                    {
                        List<DateTime> holiday = LeaveRepo.GetDateFromHoliday();
                        decimal noofdays = (decimal)Utils.DaysLeft(leave.from_date, leave.to_date, true, holiday);
                        //string leave_type = LeaveRepo.GetLeaveTypeById(leave.leavetype_id);
                        Leave leave_instance = new Leave();
                        if (noofdays == 0)
                        {
                            response = Request.CreateResponse(HttpStatusCode.OK, new EMSResponseMessage("EMS_505", "selected date falls on holiday", "selected date falls on holiday"));
                        }

                        
                        else if (leave_type1 == "CL")
                        {
                            //int leave_type_id = LeaveRepo.GetLeavetypeIdByLeavetype(leave_type1);
                            decimal? Cl_leave_type = LeaveRepo.GetNoofDaysById(leave.leavetype_id, leave.employee_id);
                            if (noofdays > 3)
                            {

                                response = Request.CreateResponse(HttpStatusCode.OK, new EMSResponseMessage("EMS_506", "CL can't be more than three days", "CL can't be more than three days"));
                            }
                            else if (noofdays <= 3)
                            {
                                if (Cl_leave_type < noofdays)
                                {
                                    ReportingTo reporting_to = EmployeeRepo.GetReportingtoByEmpId(leave.employee_id);
                                    MailHandler.LeaveMailing(leave.from_date, leave.to_date, employee_instance.first_name, Constants.LEAVE_STATUS_PENDING, employee_instance.email, reporting_to.mailid, null);
                                    response = Request.CreateResponse(HttpStatusCode.OK, new EMSResponseMessage("EMS_507", "you dont have enough CL leavebalance", "you dont have enough CL leavebalance"));
                                }
                                else if (Cl_leave_type >= noofdays)
                                {
                                    Cl_leave_type = Cl_leave_type - noofdays;
                                    //leave.LOP = 0;
                                    leave.no_of_days = (int)noofdays;
                                    LeaveRepo.AddLeaveHistory(leave);
                                    if (noofdays != 0)
                                    {
                                        ReportingTo reporting_to = EmployeeRepo.GetReportingtoByEmpId(leave.employee_id);

                                        MailHandler.LeaveMailing(leave.from_date, leave.to_date, employee_instance.first_name, Constants.LEAVE_STATUS_PENDING, employee_instance.email, reporting_to.mailid, null);
                                        response = Request.CreateResponse(HttpStatusCode.OK, new EMSResponseMessage("EMS_001", "Success", "Leave successfully Applied"));
                                    }
                                }
                            }
                        }
                        else if (leave_type1 == "EL")
                        {
                            //int leave_type_id = LeaveRepo.GetLeavetypeIdByLeavetype(leave_type1);
                            int LOP_leave_type_id = LeaveRepo.GetLeavetypeIdByLeavetype("LOP");
                            decimal Lop_leave_type = LeaveRepo.GetNoofDaysById(LOP_leave_type_id, leave.employee_id);
                            decimal? El_leave_type = LeaveRepo.GetNoofDaysById(leave.leavetype_id, leave.employee_id);
                            if (El_leave_type < noofdays)
                            {
                                El_leave_type = noofdays - El_leave_type;
                                Lop_leave_type = Math.Abs(Lop_leave_type + (decimal)El_leave_type);
                                El_leave_type = 0;
                                leave.no_of_days = (int)noofdays;
                                LeaveRepo.AddLeaveHistory(leave);
                                if (noofdays != 0)
                                {
                                    ReportingTo reporting_to = EmployeeRepo.GetReportingtoByEmpId(leave.employee_id);
                                    MailHandler.LeaveMailing(leave.from_date, leave.to_date, employee_instance.first_name, Constants.LEAVE_STATUS_PENDING, employee_instance.email, reporting_to.mailid, null);
                                    response = Request.CreateResponse(HttpStatusCode.OK, new EMSResponseMessage("EMS_001", "Success", "Leave successfully Applied"));
                                }
                            }
                            else if (El_leave_type >= noofdays)
                            {
                                El_leave_type = El_leave_type - noofdays;
                                LOP_leave_type_id = 0;
                                leave.no_of_days = (int)noofdays;
                                LeaveRepo.AddLeaveHistory(leave);
                                if (noofdays != 0)
                                {
                                    ReportingTo reporting_to = EmployeeRepo.GetReportingtoByEmpId(leave.employee_id);

                                    MailHandler.LeaveMailing(leave.from_date, leave.to_date, employee_instance.first_name, Constants.LEAVE_STATUS_PENDING, employee_instance.email, reporting_to.mailid, null);
                                    response = Request.CreateResponse(HttpStatusCode.OK, new EMSResponseMessage("EMS_001", "Success", "Leave successfully Applied"));
                                }


                            }
                        }
                        else if (leave_type1 == "LOP")
                        {
                            //int leave_type_id = LeaveRepo.GetLeavetypeIdByLeavetype(leave_type1);
                            //int LOP_leave_type_id = LeaveRepo.GetLeavetypeIdByLeavetype("LOP");
                            decimal Lop_leave_type = LeaveRepo.GetNoofDaysById(leave.leavetype_id, leave.employee_id);
                            int Cl_leave_id = LeaveRepo.GetLeavetypeIdByLeavetype("CL");
                            int El_leave_id = LeaveRepo.GetLeavetypeIdByLeavetype("EL");
                            decimal Cl_leave_type = LeaveRepo.GetNoofDaysById(Cl_leave_id, leave.employee_id);
                            decimal El_leave_type = LeaveRepo.GetNoofDaysById(El_leave_id, leave.employee_id);
                            if (Cl_leave_type == 0 && El_leave_type == 0)
                            {
                                Lop_leave_type = Lop_leave_type + noofdays;
                                leave.no_of_days = (int)noofdays;
                                LeaveRepo.AddLeaveHistory(leave);
                                if (noofdays != 0)
                                {
                                    ReportingTo reporting_to = EmployeeRepo.GetReportingtoByEmpId(leave.employee_id);

                                    MailHandler.LeaveMailing(leave.from_date, leave.to_date, employee_instance.first_name, Constants.LEAVE_STATUS_PENDING, employee_instance.email, reporting_to.mailid, null);
                                    response = Request.CreateResponse(HttpStatusCode.OK, new EMSResponseMessage("EMS_001", "Success", "Leave successfully Applied"));
                                }
                            }
                            else if (Cl_leave_type == 0 && El_leave_type > 0)
                            {

                                response = Request.CreateResponse(HttpStatusCode.OK, new EMSResponseMessage("EMS_508", "you can apply the Leave in 'EL'", "you can apply the Leave in 'EL'"));
                            }
                            else if (El_leave_type == 0 && Cl_leave_type > 0)
                            {
                                response = Request.CreateResponse(HttpStatusCode.OK, new EMSResponseMessage("EMS_509", "you can apply the Leave in 'CL'", "you can apply the Leave in 'CL'"));
                            }
                            else if (Cl_leave_type > 0 && El_leave_type > 0)
                            {
                                response = Request.CreateResponse(HttpStatusCode.OK, new EMSResponseMessage("EMS_510", "You have CL, EL leave balance", "You have CL, EL leave balance"));
                            }
                        }
                        else if (leave_type1 == "WFH")
                        {
                            //int leave_type_id = LeaveRepo.GetLeavetypeIdByLeavetype(leave_type1);
                            decimal? WFH_leave_type = LeaveRepo.GetNoofDaysById(leave.leavetype_id, leave.employee_id);
                            if (noofdays <= 2)
                            {
                                leave.no_of_days = (int)noofdays;
                                LeaveRepo.AddLeaveHistory(leave);
                                if (noofdays != 0)
                                {
                                    ReportingTo reporting_to = EmployeeRepo.GetReportingtoByEmpId(leave.employee_id);
                                    MailHandler.LeaveMailing(leave.from_date, leave.to_date, employee_instance.first_name, Constants.LEAVE_STATUS_PENDING, employee_instance.email, reporting_to.mailid, null);
                                    response = Request.CreateResponse(HttpStatusCode.OK, new EMSResponseMessage("EMS_001", "Success", "Leave successfully Applied"));
                                }
                            }
                            else if (noofdays > 2)
                            {
                                response = Request.CreateResponse(HttpStatusCode.OK, new EMSResponseMessage("EMS_511", "work from home cant be applied more than two days", "work from home cant be applied more than two dayss"));
                            }
                        }
                    }
                    else
                    {
                        response = Request.CreateResponse(HttpStatusCode.OK, new EMSResponseMessage("EMS_512", "dont leave the fields", "dont leave the fields"));
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
        [Route("api/leavehistory/id/{employee_id?}")] // display Leavehistory in leavepage by id
        public HttpResponseMessage GetLeaveHistoryById(int employee_id)
        {
            HttpResponseMessage response = null;
            try
            {
                List<LeavehistoryModel> leave_history_model = LeaveRepo.GetLeaveHistoryById(employee_id);
                response = Request.CreateResponse(HttpStatusCode.OK, new EMSResponseMessage("EMS_513", "approved", leave_history_model));
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
        [Route("api/balanceleave/{employee_id?}")] //balance CL EL Ml Lop leave in applyleave page
        public HttpResponseMessage GetLeavePageBalanceLeave(int employee_id)
        {
            HttpResponseMessage response = null;
            try
            {
                List<LeaveBalanceModel> leavebalance = LeaveRepo.GetLeaveBalanceById(employee_id);
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
        public HttpResponseMessage ChangePassword(ChangePasswordModel changepassword)
        {
            HttpResponseMessage response = null;
            try
            {
                int user_id = LeaveRepo.GetUserIdById(changepassword.id);
                User user_instance = LeaveRepo.GetUserById(user_id);
                if (changepassword.new_password == changepassword.confirm_password)
                {
                    if (changepassword.oldpassword == user_instance.password)
                    {
                        user_instance.password = changepassword.new_password;
                        LeaveRepo.EditUserPassword(user_instance);
                        response = Request.CreateResponse(HttpStatusCode.OK, new EMSResponseMessage("EMS_001", "Success", "Password sucessfully changed"));
                    }
                    else
                    {
                        response = Request.CreateResponse(HttpStatusCode.OK, new EMSResponseMessage("EMS_514", "password not matched", "password mismatch"));
                    }

                }
                else
                {
                    response = Request.CreateResponse(HttpStatusCode.OK, new EMSResponseMessage("EMS_302", "confirm password not match", "new password and confirm password doesnot match"));
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
        [Route("api/approval/{leave_id?}/{is_approved?}/{remarks?}")]
        public HttpResponseMessage GetApproval(int leave_id, int is_approved, string remarks)
        {
            HttpResponseMessage response = null;
            try
            {
                if (is_approved == 2)
                {
                    //LeaveRepo.GetLeaveById(leave.employee_id, leave.id);
                    //Leavebalance_sheet leave_balance = new Leavebalance_sheet();
                    Leave leave1 = LeaveRepo.GetLeaveById(leave_id);
                    leave1.leave_statusid = Constants.LEAVE_STATUS_APPROVED;
                    LeaveRepo.EditLeave(leave1);
                    Leavebalance_sheet leave_balance_instance = LeaveRepo.LeaveBalanceById(leave1.employee_id, leave1.leavetype_id);

                    decimal? no_of_days = LeaveRepo.GetNoofdaysByLeaveTypeId(leave1.leavetype_id);
                    leave_balance_instance.no_of_days = (decimal)no_of_days - leave1.no_of_days;
                    LeaveRepo.UpdateLeaveBalanceSheet(leave_balance_instance);
                    Employee employee = EmployeeRepo.GetEmployeeById(leave1.employee_id);
                    ReportingTo reporting_to = EmployeeRepo.GetReportingtoByEmpId(leave1.employee_id);

                    MailHandler.LeaveMailing(leave1.from_date, leave1.to_date, employee.first_name, Constants.LEAVE_STATUS_APPROVED, employee.email, reporting_to.mailid, remarks);
                    response = Request.CreateResponse(HttpStatusCode.OK, new EMSResponseMessage("EMS_001", "Success", "Leave Approved"));
                }
                else if (is_approved == 3)
                {
                    Leave leave1 = LeaveRepo.GetLeaveById(leave_id);
                    leave1.leave_statusid = Constants.LEAVE_STATUS_REJECTED;
                    LeaveRepo.EditLeave(leave1);
                    ReportingTo reporting_to = EmployeeRepo.GetReportingtoByEmpId(leave1.employee_id);
                    Employee employee = EmployeeRepo.GetEmployeeById(leave1.employee_id);
                    MailHandler.LeaveMailing(leave1.from_date, leave1.to_date, employee.first_name, Constants.LEAVE_STATUS_REJECTED, employee.email, reporting_to.mailid, remarks);
                    response = Request.CreateResponse(HttpStatusCode.OK, new EMSResponseMessage("EMS_515", "Leave not approved", "Leave cancel"));
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
        [Route("api/employeelist/byrole/{employee_id?}")]// Get employee list by hr/project manager id
        [HttpGet]
        public HttpResponseMessage EmployeeListByRole(int employee_id)
        {
            HttpResponseMessage response = null;
            try
            {
                //Project_role project_role = ProjectRepo.GetProjectIdRoleId(e_id);
                //string project_name = ProjectRepo.GetProjectName(project_role.project_id);
                //string role_name = ProjectRepo.GetProjectRole(project_role.role_id);
                List<EmployeeListByRoleModel> employee_list_byrole = LeaveRepo.GetEmployeeListByRole(employee_id);
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
        [HttpGet]
        [Route("api/leave/pending/{employee_id?}")] //leave request project manager page
        public HttpResponseMessage GetLeaveRequestByRole(int employee_id)
        {
            HttpResponseMessage response = null;
            try
            {
                List<LeavehistoryModel> leave_history = LeaveRepo.GetRequestByRoleId(employee_id);
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
        [Route("api/entire/leave/history/pending")] // pending approval in hr manager
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
        [Route("api/typesofleaves/list")] //for leave type list dropdown
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
        [Route("api/leave/fullhistoryby/{reportingto_id?}")] // pending approval in hr manager
        public HttpResponseMessage GetPendingApprovedList(int reportingto_id = 0)
        {
            HttpResponseMessage response = null;
            try
            {
                //Project_role project_role = ProjectRepo.GetProjectIdRoleId(employee.id);
                //string role_name = ProjectRepo.GetRoleNameById(project_role.role_id);
                //if (role_name == "HR")
                //{
                List<LeavehistoryModel> leave_history = LeaveRepo.GetPendingApprovedLeave(reportingto_id);
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
        [Route("api/entire/leave/history")] // leave history of all employee (HR only can view this history list)
        public HttpResponseMessage GetLeaveHistoryList()
        {
            HttpResponseMessage response = null;
            try
            {
                //Project_role project_role = ProjectRepo.GetProjectIdRoleId(employee.id);
                //string role_name = ProjectRepo.GetRoleNameById(project_role.role_id);
                //if (role_name == "HR")
                //{
                List<LeavehistoryModel> leave_history = LeaveRepo.GetLeaveHistory();
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
    }
}
