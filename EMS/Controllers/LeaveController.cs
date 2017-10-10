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

        [Route("api/sorted/holiday/list")]
        public HttpResponseMessage GetsortedHolidayList()
        {
            HttpResponseMessage response = null;
            try
            {
                List<Holiday_List> holidayList = LeaveRepo.GetsortedHoliday();
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
                string leave_type = LeaveRepo.GetLeaveTypeById(leave.leavetype_id);

                leave.from_date = leave.from_date.Date;
                leave.to_date = leave.to_date.Date;
                DateTime timeNow = DateTime.Now.Date;

                int applied_from_year = leave.from_date.Year;
                int applied_to_year = leave.to_date.Year;

                if(applied_from_year>DateTime.Now.Year || applied_to_year>DateTime.Now.Year && leave_type!="ML")
                {
                    response = Request.CreateResponse(HttpStatusCode.OK, new EMSResponseMessage("EMS_502", "Leave cannot be applied for the future year", "Leave cannot be applied for the future year"));
                    return response;
                }

                if (leave_type == "ML" && leave.from_date >= timeNow)
                {
                    leave.to_date = leave.from_date.AddDays(182);
                }

                if (leave.from_date < timeNow || leave.to_date < timeNow || leave.from_date > leave.to_date)
                {
                    response = Request.CreateResponse(HttpStatusCode.OK, new EMSResponseMessage("EMS_502", "Please select valid date ", "Please select valid date"));
                }

                else
                {
                    List<Leave> leavelist = LeaveRepo.GetActiveLeaveListByEmpId(leave.employee_id);// to get the pending and approved leave list 
                    foreach (Leave leaveinstance in leavelist)//to check if leave is already applied for the given days 
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

                    #region ML_Leave 

                    if (gender == "female" && leave_type == "ML")
                    {
                        Leavebalance_sheet leave_balance_instance = LeaveRepo.LeaveBalanceById(leave.employee_id, leave.leavetype_id);
                        if (leave_balance_instance.no_of_days != 0 && leave_balance_instance.no_of_days == 182)
                        {
                            leave.no_of_days = 182;

                            leave_balance_instance.no_of_days = leave_balance_instance.no_of_days - leave.no_of_days;
                            LeaveRepo.UpdateLeaveBalanceSheet(leave_balance_instance);

                            LeaveRepo.AddLeaveHistory(leave);
                            ReportingTo reporting_to = EmployeeRepo.GetReportingtoByEmpId(leave.employee_id);
                            MailHandler.LeaveMailing(leave.from_date, leave.to_date, employee_instance.first_name, Constants.LEAVE_STATUS_PENDING, employee_instance.email, reporting_to.mailid, null);
                            response = Request.CreateResponse(HttpStatusCode.OK, new EMSResponseMessage("EMS_001", "Success", "Leave successfully Applied"));
                        }
                        else
                        {
                            response = Request.CreateResponse(HttpStatusCode.OK, new EMSResponseMessage("EMS_503", "Already applied", "you already applied Maternity Leave"));
                        }
                    }
                    else if(gender == "male" && leave_type == "ML")
                    {
                        response = Request.CreateResponse(HttpStatusCode.OK, new EMSResponseMessage("EMS_504", "invalid leave type", "Invalid leave type"));
                    }

                    #endregion ML_Leave

                    else if (leave.leavetype_id != 0 && leave.from_date != DateTime.MinValue && leave.to_date != DateTime.MinValue && (leave.to_date >= timeNow) && leave.to_date >= leave.from_date)
                    {
                        List<DateTime> holiday = LeaveRepo.GetDateFromHoliday();
                        decimal noofdays = (decimal)Utils.DaysLeft(leave.from_date, leave.to_date, true, holiday);
                        if (noofdays == 0)
                        {
                            response = Request.CreateResponse(HttpStatusCode.OK, new EMSResponseMessage("EMS_505", "selected date falls on holiday", "selected date falls on holiday"));
                        }

                        #region CL_Leave 
                        else if (leave_type == "CL")
                        {
                            //int leave_type_id = LeaveRepo.GetLeavetypeIdByLeavetype(leave_type1);
                            Leavebalance_sheet leave_balance_instance = LeaveRepo.LeaveBalanceById(leave.employee_id, leave.leavetype_id);
                            if (noofdays > 3)
                            {
                                response = Request.CreateResponse(HttpStatusCode.OK, new EMSResponseMessage("EMS_506", "CL can't be more than three days", "CL can't be more than three days"));
                            }
                            else if (noofdays <= 3)
                            {
                                if (leave_balance_instance.no_of_days < noofdays)
                                {
                                    ReportingTo reporting_to = EmployeeRepo.GetReportingtoByEmpId(leave.employee_id);
                                    MailHandler.LeaveMailing(leave.from_date, leave.to_date, employee_instance.first_name, Constants.LEAVE_STATUS_PENDING, employee_instance.email, reporting_to.mailid, null);
                                    response = Request.CreateResponse(HttpStatusCode.OK, new EMSResponseMessage("EMS_507", "you dont have enough CL leavebalance", "you dont have enough CL leavebalance"));
                                }
                                else if (leave_balance_instance.no_of_days >= noofdays)
                                {
                                    leave.no_of_days = (int)noofdays;

                                    leave_balance_instance.no_of_days = leave_balance_instance.no_of_days - leave.no_of_days;
                                    LeaveRepo.UpdateLeaveBalanceSheet(leave_balance_instance);

                                    LeaveRepo.AddLeaveHistory(leave);

                                    ReportingTo reporting_to = EmployeeRepo.GetReportingtoByEmpId(leave.employee_id);
                                    MailHandler.LeaveMailing(leave.from_date, leave.to_date, employee_instance.first_name, Constants.LEAVE_STATUS_PENDING, employee_instance.email, reporting_to.mailid, null);
                                    response = Request.CreateResponse(HttpStatusCode.OK, new EMSResponseMessage("EMS_001", "Success", "Leave successfully Applied"));
                                }
                            }
                        }
                        #endregion CL_Leave

                        #region EL_Leave 
                        else if (leave_type == "EL")
                        {
                            int LOP_leave_type_id = LeaveRepo.GetLeavetypeIdByLeavetype("LOP");
                            decimal Lop_leave_balance = LeaveRepo.GetNoofDaysById(LOP_leave_type_id, leave.employee_id);
                            decimal El_leave_balance = LeaveRepo.GetNoofDaysById(leave.leavetype_id, leave.employee_id);
                            if (El_leave_balance < noofdays)
                            {
                                El_leave_balance = noofdays - El_leave_balance;
                                Lop_leave_balance = Math.Abs(Lop_leave_balance + (decimal)El_leave_balance);
                                El_leave_balance = 0;
                                leave.no_of_days = (int)noofdays;

                                Leavebalance_sheet EL_balance_instance = LeaveRepo.LeaveBalanceById(leave.employee_id, leave.leavetype_id);
                                leave.EL_flag = (int)EL_balance_instance.no_of_days;
                                EL_balance_instance.no_of_days = El_leave_balance;
                                LeaveRepo.UpdateLeaveBalanceSheet(EL_balance_instance);

                                Leavebalance_sheet lop_balance_instance = LeaveRepo.LeaveBalanceById(leave.employee_id, LOP_leave_type_id);
                                lop_balance_instance.no_of_days = Lop_leave_balance;
                                LeaveRepo.UpdateLeaveBalanceSheet(lop_balance_instance);
                                
                                LeaveRepo.AddLeaveHistory(leave);

                                ReportingTo reporting_to = EmployeeRepo.GetReportingtoByEmpId(leave.employee_id);
                                MailHandler.LeaveMailing(leave.from_date, leave.to_date, employee_instance.first_name, Constants.LEAVE_STATUS_PENDING, employee_instance.email, reporting_to.mailid, null);
                                response = Request.CreateResponse(HttpStatusCode.OK, new EMSResponseMessage("EMS_001", "Success", "Leave successfully Applied"));

                            }
                            else if (El_leave_balance >= noofdays)
                            {
                                El_leave_balance = El_leave_balance - noofdays;
                                leave.no_of_days = (int)noofdays;

                                Leavebalance_sheet leave_balance_instance = LeaveRepo.LeaveBalanceById(leave.employee_id, leave.leavetype_id);
                                leave_balance_instance.no_of_days = leave_balance_instance.no_of_days - leave.no_of_days;
                                LeaveRepo.UpdateLeaveBalanceSheet(leave_balance_instance);

                                LeaveRepo.AddLeaveHistory(leave);

                                ReportingTo reporting_to = EmployeeRepo.GetReportingtoByEmpId(leave.employee_id);
                                MailHandler.LeaveMailing(leave.from_date, leave.to_date, employee_instance.first_name, Constants.LEAVE_STATUS_PENDING, employee_instance.email, reporting_to.mailid, null);
                                response = Request.CreateResponse(HttpStatusCode.OK, new EMSResponseMessage("EMS_001", "Success", "Leave successfully Applied"));
                            }
                        }
                        #endregion EL_Leave

                        #region LOP
                        else if (leave_type == "LOP")
                        {
                            decimal Lop_leave_balance = LeaveRepo.GetNoofDaysById(leave.leavetype_id, leave.employee_id);
                            int Cl_leave_id = LeaveRepo.GetLeavetypeIdByLeavetype("CL");
                            int El_leave_id = LeaveRepo.GetLeavetypeIdByLeavetype("EL");
                            decimal Cl_leave_type = LeaveRepo.GetNoofDaysById(Cl_leave_id, leave.employee_id);
                            decimal El_leave_type = LeaveRepo.GetNoofDaysById(El_leave_id, leave.employee_id);
                            if (Cl_leave_type == 0 && El_leave_type == 0)
                            {
                                Lop_leave_balance = Lop_leave_balance + noofdays;
                                leave.no_of_days = (int)noofdays;

                                Leavebalance_sheet leave_balance_instance = LeaveRepo.LeaveBalanceById(leave.employee_id, leave.leavetype_id);
                                leave_balance_instance.no_of_days = leave_balance_instance.no_of_days + leave.no_of_days;
                                LeaveRepo.UpdateLeaveBalanceSheet(leave_balance_instance);

                                LeaveRepo.AddLeaveHistory(leave);

                                ReportingTo reporting_to = EmployeeRepo.GetReportingtoByEmpId(leave.employee_id);
                                MailHandler.LeaveMailing(leave.from_date, leave.to_date, employee_instance.first_name, Constants.LEAVE_STATUS_PENDING, employee_instance.email, reporting_to.mailid, null);
                                response = Request.CreateResponse(HttpStatusCode.OK, new EMSResponseMessage("EMS_001", "Success", "Leave successfully Applied"));
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
                        #endregion LOP

                        #region Wrok_From _Home
                        else if (leave_type == "WFH")
                        {
                            decimal? WFH_leave_balance = LeaveRepo.GetNoofDaysById(leave.leavetype_id, leave.employee_id);
                            if (noofdays <= 2)
                            {
                                leave.no_of_days = (int)noofdays;

                                Leavebalance_sheet leave_balance_instance = LeaveRepo.LeaveBalanceById(leave.employee_id, leave.leavetype_id);
                                leave_balance_instance.no_of_days = leave_balance_instance.no_of_days + leave.no_of_days;
                                LeaveRepo.UpdateLeaveBalanceSheet(leave_balance_instance);

                                LeaveRepo.AddLeaveHistory(leave);

                                ReportingTo reporting_to = EmployeeRepo.GetReportingtoByEmpId(leave.employee_id);
                                MailHandler.LeaveMailing(leave.from_date, leave.to_date, employee_instance.first_name, Constants.LEAVE_STATUS_PENDING, employee_instance.email, reporting_to.mailid, null);
                                response = Request.CreateResponse(HttpStatusCode.OK, new EMSResponseMessage("EMS_001", "Success", "Leave successfully Applied"));
                            }
                            else if (noofdays > 2)
                            {
                                response = Request.CreateResponse(HttpStatusCode.OK, new EMSResponseMessage("EMS_511", "work from home cant be applied more than two days", "work from home cant be applied more than two dayss"));
                            }
                        }
                        #endregion
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

        

        //[HttpPost]
        [Route("api/approval/{leave_id?}/{is_approved?}/{remarks?}")]
        public HttpResponseMessage GetApproval(int leave_id, int is_approved, string remarks)
        {
            HttpResponseMessage response = null;
            Leave leave = LeaveRepo.GetLeaveById(leave_id);
            try
            {
                if (is_approved == Constants.LEAVE_STATUS_APPROVED)
                {
                    leave.leave_statusid = Constants.LEAVE_STATUS_APPROVED;
                    LeaveRepo.EditLeave(leave);

                    response = Request.CreateResponse(HttpStatusCode.OK, new EMSResponseMessage("EMS_001", "Success", "Leave Approved"));
                }
                else if (is_approved == Constants.LEAVE_STATUS_REJECTED)
                {
                    Leavebalance_sheet leave_balance_instance = LeaveRepo.LeaveBalanceById(leave.employee_id, leave.leavetype_id);
                    string leave_type_name = LeaveRepo.GetLeaveTypeById(leave.leavetype_id);

                    #region CL and ML
                    if(leave_type_name == "CL" || leave_type_name =="ML")
                    {
                        leave_balance_instance.no_of_days = leave_balance_instance.no_of_days + leave.no_of_days;
                    }
                    #endregion

                    #region EL
                    else if(leave_type_name == "EL")
                    {
                        if (leave.EL_flag > 0)
                        {
                            int LOP_leave_type_id = LeaveRepo.GetLeavetypeIdByLeavetype("LOP");
                            Leavebalance_sheet lop_balance_instance = LeaveRepo.LeaveBalanceById(leave.employee_id, LOP_leave_type_id);
                            leave_balance_instance.no_of_days = (decimal)leave.EL_flag;
                            lop_balance_instance.no_of_days =(decimal)(lop_balance_instance.no_of_days - (decimal)(leave.no_of_days - leave.EL_flag));
                            LeaveRepo.UpdateLeaveBalanceSheet(lop_balance_instance);
                        }
                        else
                        {
                            leave_balance_instance.no_of_days = leave_balance_instance.no_of_days + leave.no_of_days;
                        }
                    }
                    #endregion

                    #region LOP and WFH
                    else if(leave_type_name =="LOP" || leave_type_name =="WFH")
                    {
                        leave_balance_instance.no_of_days = leave_balance_instance.no_of_days - leave.no_of_days;
                    }
                    #endregion

                    LeaveRepo.UpdateLeaveBalanceSheet(leave_balance_instance);

                    leave.leave_statusid = Constants.LEAVE_STATUS_REJECTED;
                    LeaveRepo.EditLeave(leave);

                    response = Request.CreateResponse(HttpStatusCode.OK, new EMSResponseMessage("EMS_515", "Leave not approved", "Leave cancel"));
                }
                //ReportingTo reporting_to = EmployeeRepo.GetReportingtoByEmpId(leave.employee_id);
                //Employee employee = EmployeeRepo.GetEmployeeById(leave.employee_id);
                //MailHandler.LeaveMailing(leave.from_date, leave.to_date, employee.first_name, leave.leave_statusid, employee.email, reporting_to.mailid, remarks);
            }
            catch (Exception exception)
            {
                Debug.WriteLine(exception.Message);
                Debug.WriteLine(exception.GetBaseException());
                response = Request.CreateResponse(HttpStatusCode.OK, new EMSResponseMessage("EMS_101", "Application Error", exception.Message));
            }
            return response;
        }

        //[Route("api/employeelist/byrole/{employee_id?}")]// Get employee list by hr/project manager id
        //[HttpGet]
        //public HttpResponseMessage EmployeeListByRole(int employee_id)
        //{
        //    HttpResponseMessage response = null;
        //    try
        //    {
        //        //Project_role project_role = ProjectRepo.GetProjectIdRoleId(e_id);
        //        //string project_name = ProjectRepo.GetProjectName(project_role.project_id);
        //        //string role_name = ProjectRepo.GetProjectRole(project_role.role_id);
        //        List<EmployeeListByRoleModel> employee_list_byrole = LeaveRepo.GetEmployeeListByRole(employee_id);
        //        //List<EmployeeListByRoleModel> emp_proj_details_byrole = LeaveRepo.emp_proj_details_byrole(employee_list_byrole);
        //        response = Request.CreateResponse(HttpStatusCode.OK, new EMSResponseMessage("EMS_001", "Success", employee_list_byrole));
        //    }
        //    catch (Exception exception)
        //    {
        //        Debug.WriteLine(exception.Message);
        //        Debug.WriteLine(exception.GetBaseException());
        //        response = Request.CreateResponse(HttpStatusCode.OK, new EMSResponseMessage("EMS_101", "Application Error", exception.Message));
        //    }
        //    return response;
        //}

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
        [Route("api/get/leave/{leave_id?}")] 
        public HttpResponseMessage GetLeaveHistoryList(int leave_id)
        {
            HttpResponseMessage response = null;
            try
            {
                if(leave_id <= 0)
                {
                    response = Request.CreateResponse(HttpStatusCode.OK, new EMSResponseMessage("EMS_102", "Invalid Request", "Invalid Request"));
                }
                else
                {
                    LeavehistoryModel leavehistory = LeaveRepo.GetLeaveDetailsByLeaveId(leave_id);
                    response = Request.CreateResponse(HttpStatusCode.OK, new EMSResponseMessage("EMS_001", "Success", leavehistory));
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
