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

        [Route("api/get/timesheet/{id?}")]
        public HttpResponseMessage GetSheetById(int id)
        {
            HttpResponseMessage response = null;
            try
            {
                if(id != 0)
                {
                    TimeSheetModel record = TimeSheetRepo.GetSheetById(id);
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


        [Route("api/get/employee/timesheet/{e_id?}")]
        public HttpResponseMessage GetSheetByEmpId(int e_id)
        {
            HttpResponseMessage response = null;
            try
            {
                if (e_id != 0)
                {
                    //int emp_status = EmployeeRepo.GetEmployeeStatusByID(e_id);
                    //if (emp_status == 1)
                    //{
                        List<TimeSheetModel> record_list = TimeSheetRepo.GetSheetByEmpId(e_id);
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

        [Route("api/get/project/timesheet/{p_id?}")]
        public HttpResponseMessage GetSheetByProjId(int p_id)
        {
            HttpResponseMessage response = null;
            try
            {
                if (p_id != 0)
                {
                    List<TimeSheetModel> record_list = TimeSheetRepo.GetSheetByProjId(p_id);
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
        [Route("api/timesheet/delete/{s_id}")]
        public HttpResponseMessage DeleteSheetById(int s_id)
        {
            HttpResponseMessage response = null;
            try
            {
                if(s_id!=0)
                {
                    if (TimeSheetRepo.GetSheetById(s_id) != null)
                    {
                        TimeSheetRepo.DeleteTimeSheetRecord(s_id);
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
