using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using EMS.Models;
using EMS.Repository;
using EMS.Utility;

namespace EMS.Controllers
{
    public class TimeSheetController : ApiController
    {
        [Route("api/timesheet/add")]
        public HttpResponseMessage AddTimeSheetRecord(List<Timesheet> sheetrecord)
        {
            HttpResponseMessage response = null;
            try
            {
                if (sheetrecord != null)
                {
                    TimeSheetRepo.AddTimeSheetRecord(sheetrecord);
                    response = Request.CreateResponse(HttpStatusCode.OK, new EMSResponseMessage("EMS_001", "Success", "TimeSheet updated Succesfully"));
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

        [Route("api/timesheet/edit")]
        public HttpResponseMessage EditTimeSheetRecord(Timesheet sheetdetails)
        {
            HttpResponseMessage response = null;
            try
            {
                if (sheetdetails != null)
                {
                    if (TimeSheetRepo.GetSheetById(sheetdetails.id) != null)
                    {
                        TimeSheetRepo.UpdateTimeSheetRecord(sheetdetails);
                        response = Request.CreateResponse(HttpStatusCode.OK, new EMSResponseMessage("EMS_001", "Success", "TimeSheet updated Succesfully"));
                    }
                    else
                    {
                        response = Request.CreateResponse(HttpStatusCode.OK, new EMSResponseMessage("EMS_103", "Invalid", "TimeSheet doesnot exists!"));
                    }
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

        [Route("api/get/timesheet/{timesheetid?}")]
        public HttpResponseMessage GetSheetById(int timesheetid)//timesheet id
        {
            HttpResponseMessage response = null;
            try
            {
                if(timesheetid != 0)
                {
                    TimeSheetModel record = TimeSheetRepo.GetSheetById(timesheetid);
                    if(record!= null)
                    {
                        response = Request.CreateResponse(HttpStatusCode.OK, new EMSResponseMessage("EMS_001", "Success", record));
                    }
                    else
                    {
                        response = Request.CreateResponse(HttpStatusCode.OK, new EMSResponseMessage("EMS_103", "Invalid", "TimeSheet doesnot exists!"));
                    }
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


        [Route("api/get/employee/timesheet/{employee_id?}")]
        public HttpResponseMessage GetSheetByEmpId(int employee_id)//e_id employee_id
        {
            HttpResponseMessage response = null;
            try
            {
                if (employee_id != 0)
                {
                    //int emp_status = EmployeeRepo.GetEmployeeStatusByID(e_id);
                    //if (emp_status == 1)
                    //{
                        List<TimeSheetModel> record_list = TimeSheetRepo.GetSheetByEmpId(employee_id);
                        response = Request.CreateResponse(HttpStatusCode.OK, new EMSResponseMessage("EMS_001", "Success", record_list));
                    //}
                    //else
                    //{
                    //    response = Request.CreateResponse(HttpStatusCode.OK, new EMSResponseMessage("EMS_103", "Invalid", "Invalid Employee ID"));
                    //}
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

        [Route("api/get/project/timesheet/{project_id?}")]
        public HttpResponseMessage GetSheetByProjId(int project_id)//p_id project_id
        {
            HttpResponseMessage response = null;
            try
            {
                if (project_id != 0)
                {
                    List<TimeSheetModel> record_list = TimeSheetRepo.GetSheetByProjId(project_id);
                    response = Request.CreateResponse(HttpStatusCode.OK, new EMSResponseMessage("EMS_001", "Success", record_list));
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

        [HttpGet]
        [Route("api/timesheet/delete/{timesheet_id}")]
        public HttpResponseMessage DeleteSheetById(int timesheet_id)//s_id timesheet_id
        {
            HttpResponseMessage response = null;
            try
            {
                if(timesheet_id != 0)
                {
                    if (TimeSheetRepo.GetSheetById(timesheet_id) != null)
                    {
                        TimeSheetRepo.DeleteTimeSheetRecord(timesheet_id);
                        response = Request.CreateResponse(HttpStatusCode.OK, new EMSResponseMessage("EMS_001", "Success", "Timesheet deleted successfully!"));
                    }
                    else
                    {
                        response = Request.CreateResponse(HttpStatusCode.OK, new EMSResponseMessage("EMS_103", "Invalid", "TimeSheet doesnot exists!"));
                    }
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
