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
using System.Data.Entity.Validation;
using System.Collections;

namespace EMS.Controllers
{
    public class LeaveController : ApiController
    {
        [Route("api/v1/holiday/list")]
        public HttpResponseMessage GetHolidayList()
        {
            HttpResponseMessage response = null;
            try
            {
                response = Request.CreateResponse(HttpStatusCode.OK, new EMSResponseMessage("EMS_001", "Success", LeaveRepo.GetHoliday()));
            }
            catch (Exception exception)
            {
                Debug.WriteLine(exception.Message);
                Debug.WriteLine(exception.GetBaseException());
                response = Request.CreateResponse(HttpStatusCode.OK, new EMSResponseMessage("EMS_101", "Application Error", exception.Message));
            }
            return response;
        }

        [Route("api/v1/sorted/holiday/list")]
        public HttpResponseMessage GetsortedHolidayList()
        {
            HttpResponseMessage response = null;
            try
            {
                response = Request.CreateResponse(HttpStatusCode.OK, new EMSResponseMessage("EMS_001", "Success", LeaveRepo.GetsortedHoliday()));
            }
            catch (Exception exception)
            {
                Debug.WriteLine(exception.Message);
                Debug.WriteLine(exception.GetBaseException());
                response = Request.CreateResponse(HttpStatusCode.OK, new EMSResponseMessage("EMS_101", "Application Error", exception.Message));
            }
            return response;
        }

        [Route("api/v1/editholidaylist")]
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

        [Route("api/v1/holiday/delete/{holiday_id?}")]
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
        [Route("api/v1/holiday/create")]
        public HttpResponseMessage CreateHoliday(Holiday_List holiday_list)
        {
            HttpResponseMessage response = null;
            try
            {
                if (holiday_list != null)
                {
                    LeaveRepo.CreateHoliday(holiday_list);
                    response = Request.CreateResponse(HttpStatusCode.OK, new EMSResponseMessage("EMS_001", "Holiday successfully created", "Holiday successfully created"));
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
        [Route("api/v1/applyleave")]
        public HttpResponseMessage ApplyLeave(Leave leave)
        {
            HttpResponseMessage response = null;
            try
            {
                if(leave.leavetype_id != 0 && leave.from_date != DateTime.MinValue && leave.to_date != DateTime.MinValue)
                {
                    string leave_type = LeaveRepo.GetLeaveTypeById(leave.leavetype_id);

                    leave.from_date = leave.from_date.Date;
                    leave.to_date = leave.to_date.Date;
                    DateTime timeNow = DateTime.Now.Date;

                    if (leave.from_date.Year > DateTime.Now.Year || leave.to_date.Year > DateTime.Now.Year && leave_type != Constants.LEAVE_TYPE_ML)
                    {
                        response = Request.CreateResponse(HttpStatusCode.OK, new EMSResponseMessage("EMS_502", "Leave should be applied in CURRENT YEAR", "Leave should be applied in CURRENT YEAR"));
                        return response;
                    }

                    if (leave_type == Constants.LEAVE_TYPE_ML && leave.from_date >= timeNow)
                    {
                        leave.to_date = leave.from_date.AddDays(182);
                    }

                    if (leave.from_date < timeNow || leave.to_date < timeNow || leave.from_date > leave.to_date)
                    {
                        response = Request.CreateResponse(HttpStatusCode.OK, new EMSResponseMessage("EMS_502", "Select valid date ", "Select valid date"));
                        return response;
                    }


                        List<Leave> leavelist = LeaveRepo.GetActiveLeaveListByEmployeeId(leave.employee_id);// to get the pending and approved leave list 
                        foreach (Leave leaveinstance in leavelist)//to check if leave is already applied for the given days 
                        {
                            if (leave.from_date <= leaveinstance.from_date && leave.to_date >= leaveinstance.to_date)
                            {
                                response = Request.CreateResponse(HttpStatusCode.OK, new EMSResponseMessage("EMS_516", "Already leaves has been applied inbetween selected days", "Already leaves has been applied inbetween selected days"));
                                return response;
                            }
                            else if ((leaveinstance.from_date <= leave.from_date && leave.from_date <= leaveinstance.to_date) || (leaveinstance.from_date <= leave.to_date && leave.to_date <= leaveinstance.to_date))
                            {
                                response = Request.CreateResponse(HttpStatusCode.OK, new EMSResponseMessage("EMS_516", "Already leaves has been applied inbetween selected days", "Already leaves has been applied inbetween selected days"));
                                return response;
                            }
                        }

                    Employee employee_instance = EmployeeRepo.GetEmployeeById(leave.employee_id);
                    string gender = employee_instance.gender;
                    #region ML_Leave 

                        if (gender.ToLower() == "female" && leave_type == Constants.LEAVE_TYPE_ML)
                        {
                            Leavebalance_sheet leave_balance_instance = LeaveRepo.LeaveBalanceById(leave.employee_id, leave.leavetype_id);
                            if (leave_balance_instance.no_of_days != 0 && leave_balance_instance.no_of_days == Constants.ML_LEAVE_BALANCE)
                            {
                                leave.no_of_days = Constants.ML_LEAVE_BALANCE;

                                leave_balance_instance.no_of_days = leave_balance_instance.no_of_days - leave.no_of_days;
                                LeaveRepo.UpdateLeaveBalanceSheet(leave_balance_instance);
                                LeaveRepo.AddLeaveHistory(leave);
                                ReportingTo reporting_to = EmployeeRepo.GetReportingtoByEmpId(leave.employee_id);
                                MailHandler.LeaveMailing(leave.from_date, leave.to_date, employee_instance.first_name, Constants.LEAVE_STATUS_PENDING, employee_instance.email, reporting_to.mailid, null, reporting_to.emp_name);
                                response = Request.CreateResponse(HttpStatusCode.OK, new EMSResponseMessage("EMS_001", "Leave Applied successfully", "Leave Applied successfully"));
                            }
                            else
                            {
                                response = Request.CreateResponse(HttpStatusCode.OK, new EMSResponseMessage("EMS_503", "Insufficient ML leave balance", "Insufficient ML leave balance"));
                                return response;
                            }
                        }
                        else if (gender.ToLower() == "male" && leave_type == Constants.LEAVE_TYPE_ML)
                        {
                            response = Request.CreateResponse(HttpStatusCode.OK, new EMSResponseMessage("EMS_504", "Invalid leave type", "Invalid leave type"));
                            return response;
                        }

                        #endregion ML_Leave

                        else
                        {
                            Leavebalance_sheet leave_balance_instance = LeaveRepo.LeaveBalanceById(leave.employee_id, leave.leavetype_id);

                            List<DateTime> holiday = LeaveRepo.GetDateFromHoliday();
                            decimal noofdays = (decimal)Utils.DaysLeft(leave.from_date, leave.to_date, true, holiday);
                            if (noofdays == 0)
                            {
                                response = Request.CreateResponse(HttpStatusCode.OK, new EMSResponseMessage("EMS_505", "Selected date falls on holiday", "Selected date falls on holiday"));
                                return response;
                            }

                            #region CL_Leave 
                            else if (leave_type == Constants.LEAVE_TYPE_CL)
                            {
                                //int leave_type_id = LeaveRepo.GetLeavetypeIdByLeavetype(leave_type1);
                                if (noofdays > 3)
                                {
                                    response = Request.CreateResponse(HttpStatusCode.OK, new EMSResponseMessage("EMS_506", "CL should be within three days", "CL should be within three days"));
                                    return response;
                                }
                                else if (noofdays <= 3)
                                {
                                    if (leave_balance_instance.no_of_days < noofdays)
                                    {
                                        response = Request.CreateResponse(HttpStatusCode.OK, new EMSResponseMessage("EMS_507", "Insufficient CL balance, Please select different leave type", "Insufficient CL balance, Please select different leave type"));
                                        return response;
                                    }
                                    else if (leave_balance_instance.no_of_days >= noofdays)
                                    {
                                        leave.no_of_days = (int)noofdays;

                                        leave_balance_instance.no_of_days = leave_balance_instance.no_of_days - leave.no_of_days;
                                        response = Request.CreateResponse(HttpStatusCode.OK, new EMSResponseMessage("EMS_001", "Leave Applied successfully", "Leave Applied successfully"));
                                    }
                                }
                            }
                            #endregion CL_Leave

                            #region EL_Leave 
                            else if (leave_type == Constants.LEAVE_TYPE_EL)
                            {
                                //int LOP_leave_type_id = LeaveRepo.GetLeavetypeIdByLeavetype(Constants.LEAVE_TYPE_LOP);
                                //decimal Lop_leave_balance = LeaveRepo.GetNoofDaysById(LOP_leave_type_id, leave.employee_id);
                                //decimal El_leave_balance = LeaveRepo.GetNoofDaysById(leave.leavetype_id, leave.employee_id);

                                if (leave_balance_instance.no_of_days < noofdays)
                                {
                                //El_leave_balance = noofdays - El_leave_balance;
                                //Lop_leave_balance = Math.Abs(Lop_leave_balance + (decimal)El_leave_balance);
                                //El_leave_balance = 0;
                                //leave.no_of_days = (int)noofdays;

                                //Leavebalance_sheet EL_balance_instance = LeaveRepo.LeaveBalanceById(leave.employee_id, leave.leavetype_id);
                                //leave.EL_flag = (decimal)EL_balance_instance.no_of_days;
                                //EL_balance_instance.no_of_days = El_leave_balance;
                                //LeaveRepo.UpdateLeaveBalanceSheet(EL_balance_instance);

                                //Leavebalance_sheet lop_balance_instance = LeaveRepo.LeaveBalanceById(leave.employee_id, LOP_leave_type_id);
                                //lop_balance_instance.no_of_days = Lop_leave_balance;
                                //LeaveRepo.UpdateLeaveBalanceSheet(lop_balance_instance);

                                //LeaveRepo.AddLeaveHistory(leave);

                                //ReportingTo reporting_to = EmployeeRepo.GetReportingtoByEmpId(leave.employee_id);
                                //MailHandler.LeaveMailing(leave.from_date, leave.to_date, employee_instance.first_name, Constants.LEAVE_STATUS_PENDING, employee_instance.email, reporting_to.mailid, null, reporting_to.emp_name);
                                //response = Request.CreateResponse(HttpStatusCode.OK, new EMSResponseMessage("EMS_001", "Leave Applied successfully", "Leave Applied successfully"));
                                response = Request.CreateResponse(HttpStatusCode.OK, new EMSResponseMessage("EMS_002", "Insufficient EL blanace, Please select different leave type", "Insufficient EL blanace, Please select different leave type"));
                                return response;
                                }
                                else if (leave_balance_instance.no_of_days >= noofdays)
                                {
                                    //El_leave_balance = El_leave_balance - noofdays;
                                    leave.no_of_days = (int)noofdays;

                                    leave_balance_instance.no_of_days = leave_balance_instance.no_of_days - leave.no_of_days;
                                    response = Request.CreateResponse(HttpStatusCode.OK, new EMSResponseMessage("EMS_001", "Leave Applied successfully", "Leave Applied successfully"));
                                }
                            }
                            #endregion EL_Leave

                            #region LOP
                            else if (leave_type == Constants.LEAVE_TYPE_LOP)
                            {
                                decimal Lop_leave_balance = LeaveRepo.GetNoofDaysById(leave.leavetype_id, leave.employee_id);
                                int Cl_leave_id = LeaveRepo.GetLeavetypeIdByLeavetype(Constants.LEAVE_TYPE_CL);
                                int El_leave_id = LeaveRepo.GetLeavetypeIdByLeavetype(Constants.LEAVE_TYPE_EL);
                                decimal Cl_leave_balance = LeaveRepo.GetNoofDaysById(Cl_leave_id, leave.employee_id);
                                decimal El_leave_balance = LeaveRepo.GetNoofDaysById(El_leave_id, leave.employee_id);
                                if (Cl_leave_balance == 0 && El_leave_balance == 0)
                                {
                                    Lop_leave_balance = Lop_leave_balance + noofdays;
                                    leave.no_of_days = (int)noofdays;
                                    leave_balance_instance.no_of_days = leave_balance_instance.no_of_days + leave.no_of_days;
                                    response = Request.CreateResponse(HttpStatusCode.OK, new EMSResponseMessage("EMS_001", "Leave Applied successfully", "Leave Applied successfully"));
                                }
                                else if (Cl_leave_balance == 0 && El_leave_balance > 0)
                                {
                                    response = Request.CreateResponse(HttpStatusCode.OK, new EMSResponseMessage("EMS_508", "You have EL leave balance. So you can apply from that", "You have EL leave balance. So you can apply from that"));
                                    return response;
                                }
                                else if (El_leave_balance == 0 && Cl_leave_balance > 0)
                                {
                                    response = Request.CreateResponse(HttpStatusCode.OK, new EMSResponseMessage("EMS_509", "You have CL leave balance. So you can apply from that", "You have CL leave balance. So you can apply from that"));
                                    return response;
                            }
                            else if (Cl_leave_balance > 0 && El_leave_balance > 0)
                                {
                                    response = Request.CreateResponse(HttpStatusCode.OK, new EMSResponseMessage("EMS_510", "You have CL, EL leave balance", "You have CL, EL leave balance"));
                                    return response;
                                }
                            }
                            #endregion LOP

                            #region Wrok_From _Home
                            else if (leave_type == Constants.LEAVE_TYPE_WFH)
                            {
                                decimal? WFH_leave_balance = LeaveRepo.GetNoofDaysById(leave.leavetype_id, leave.employee_id);
                                if (noofdays <= 2)
                                {
                                    leave.no_of_days = (int)noofdays;

                                    leave_balance_instance.no_of_days = leave_balance_instance.no_of_days + leave.no_of_days;
                                    response = Request.CreateResponse(HttpStatusCode.OK, new EMSResponseMessage("EMS_001", "Leave Applied successfully", "Leave Applied successfully"));
                                }
                                else if (noofdays > 2)
                                {
                                    response = Request.CreateResponse(HttpStatusCode.OK, new EMSResponseMessage("EMS_511", "Work from home should be within two days", "Work from home should be within two days"));
                                    return response;
                                }
                            }
                        #endregion

                        LeaveRepo.UpdateLeaveBalanceSheet(leave_balance_instance);
                        LeaveRepo.AddLeaveHistory(leave);
                        ReportingTo reporting_to = EmployeeRepo.GetReportingtoByEmpId(leave.employee_id);
                        MailHandler.LeaveMailing(leave.from_date, leave.to_date, employee_instance.first_name, Constants.LEAVE_STATUS_PENDING, employee_instance.email, reporting_to.mailid, null, reporting_to.emp_name);

                        }
                }
                else
                {
                    response = Request.CreateResponse(HttpStatusCode.OK, new EMSResponseMessage("EMS_512", "Mandatory fields are missing", "Mandatory fields are missing"));
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
        //Get own leave history of an employee by id 
        [Route("api/v1/leavehistory/id/{employee_id?}")] // display Leavehistory in leavepage by employee id
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

        [Route("api/v1/balanceleave/{employee_id?}")] //balance CL EL Ml Lop leave in applyleave page
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

        [HttpPost]
        [Route("api/v1/leave/status/update")] // leave approve and reject
        public HttpResponseMessage GetApproval(LeaveStatusModel leave_status)
        {
            HttpResponseMessage response = null;
            Leave leave = LeaveRepo.GetLeaveById(leave_status.leave_id);
            try
            {
                if (leave_status.is_approved == Constants.LEAVE_STATUS_APPROVED)
                {
                    leave.leave_statusid = Constants.LEAVE_STATUS_APPROVED;
                    LeaveRepo.EditLeave(leave);

                    response = Request.CreateResponse(HttpStatusCode.OK, new EMSResponseMessage("EMS_001", "Leave Approved", "Leave Approved"));
                }
                else if (leave_status.is_approved == Constants.LEAVE_STATUS_REJECTED || leave_status.is_approved == Constants.LEAVE_STATUS_CANCELLED)
                {
                    Leavebalance_sheet leave_balance_instance = LeaveRepo.LeaveBalanceById(leave.employee_id, leave.leavetype_id);
                    string leave_type_name = LeaveRepo.GetLeaveTypeById(leave.leavetype_id);

                    #region CL and ML
                    if(leave_type_name == Constants.LEAVE_TYPE_CL || leave_type_name == Constants.LEAVE_TYPE_ML)
                    {
                        leave_balance_instance.no_of_days = leave_balance_instance.no_of_days + leave.no_of_days;
                    }
                    #endregion

                    #region EL
                    else if(leave_type_name == Constants.LEAVE_TYPE_EL)
                    {
                        //if (leave.EL_flag > 0)
                        //{
                        //    int LOP_leave_type_id = LeaveRepo.GetLeavetypeIdByLeavetype(Constants.LEAVE_TYPE_LOP);
                        //    Leavebalance_sheet lop_balance_instance = LeaveRepo.LeaveBalanceById(leave.employee_id, LOP_leave_type_id);
                        //    leave_balance_instance.no_of_days = (decimal)leave.EL_flag;
                        //    lop_balance_instance.no_of_days =(decimal)(lop_balance_instance.no_of_days - (decimal)(leave.no_of_days - leave.EL_flag));
                        //    LeaveRepo.UpdateLeaveBalanceSheet(lop_balance_instance);
                        //}
                        //else
                        {
                            leave_balance_instance.no_of_days = leave_balance_instance.no_of_days + leave.no_of_days;
                        }
                    }
                    #endregion

                    #region LOP and WFH
                    else if(leave_type_name == Constants.LEAVE_TYPE_LOP || leave_type_name == Constants.LEAVE_TYPE_WFH)
                    {
                        leave_balance_instance.no_of_days = leave_balance_instance.no_of_days - leave.no_of_days;
                    }
                    #endregion

                    LeaveRepo.UpdateLeaveBalanceSheet(leave_balance_instance);

                    if(leave_status.is_approved == Constants.LEAVE_STATUS_CANCELLED)
                    {
                        leave.leave_statusid = Constants.LEAVE_STATUS_CANCELLED;
                        LeaveRepo.EditLeave(leave);
                        response = Request.CreateResponse(HttpStatusCode.OK, new EMSResponseMessage("EMS_515", "Leave Cancelled", "Leave Cancelled"));
                    }
                    if(leave_status.is_approved == Constants.LEAVE_STATUS_REJECTED)
                    {
                        leave.leave_statusid = Constants.LEAVE_STATUS_REJECTED;
                        LeaveRepo.EditLeave(leave);
                        response = Request.CreateResponse(HttpStatusCode.OK, new EMSResponseMessage("EMS_515", "Leave Rejected", "Leave Rejected"));
                    }
                }
                ReportingTo reporting_to = EmployeeRepo.GetReportingtoByEmpId(leave.employee_id);
                Employee employee = EmployeeRepo.GetEmployeeById(leave.employee_id);
                MailHandler.LeaveMailing(leave.from_date, leave.to_date, employee.first_name, leave.leave_statusid, employee.email, reporting_to.mailid, leave_status.remarks, reporting_to.emp_name);
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
        [Route("api/v1/leave/pending/{employee_id?}")] //leave request project manager page
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

        [Route("api/v1/entire/leave/history/pending")] // pending approval in hr manager
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

        [Route("api/v1/typesofleaves/list")] //for leave type list dropdown
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

        [Route("api/v1/leave/fullhistoryby/{reportingto_id?}")] // pending approval in hr manager
        public HttpResponseMessage GetPendingApprovedList(int reportingto_id = 0)
        {
            HttpResponseMessage response = null;
            try
            {
                //Project_role project_role = ProjectRepo.GetProjectIdRoleId(employee.id);
                //string role_name = ProjectRepo.GetRoleNameById(project_role.role_id);
                //if (role_name == "HR")
                //{
                List<LeavehistoryModel> leave_history = LeaveRepo.GetApprovedRejectedCancelledLeave(reportingto_id);
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

        [Route("api/v1/entire/leave/history")] // leave history of all employee (HR only can view this history list)
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
        [Route("api/v1/get/leave/{leave_id?}")] 
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

        [HttpGet]
        [Route("api/v1/get/employee/leave/count")]
        public HttpResponseMessage GetLeaveCount(int employeeid)
        {
            HttpResponseMessage response = null;
            try
            {

            }
            catch (Exception exception)
            {
                Debug.WriteLine(exception.Message);
                Debug.WriteLine(exception.GetBaseException());
                response = Request.CreateResponse(HttpStatusCode.OK, new EMSResponseMessage("EMS_101", "Application Error", exception.Message));
            }
            return response;
        }

        [Route("api/v1/get/dashboard/{employee_id}")]
        [HttpGet]
        public HttpResponseMessage GetDashboardDetails(int employee_id)
        {
            HttpResponseMessage response = null;
            try
            {
                if(employee_id != 0)
                {
                    Dictionary<string, object> result_set = new Dictionary<string, object>();
                    result_set.Add("Available_CL_EL", LeaveRepo.GetLeaveAvailableDashboard(employee_id));
                    result_set.Add("Reporties_Pending_Leave", LeaveRepo.GetRequestByRoleId(employee_id).Count);
                    result_set.Add("Approved_Leave_Application", LeaveRepo.GetApprovedLeaveDashboard(employee_id));
                    result_set.Add("Pending_Leave_Application", LeaveRepo.GetPendingLeaves(employee_id));
                    response = Request.CreateResponse(HttpStatusCode.OK, new EMSResponseMessage("EMS_001", "Success", result_set));
                }
                else
                {
                    response = Request.CreateResponse(HttpStatusCode.OK, new EMSResponseMessage("EMS_103", "Invalid Request", "Invalid id"));
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
