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
    public class EmployeeController : ApiController
    {

        [HttpPost]
        [Route("api/employee/create")]
        public HttpResponseMessage CreateNewEmployee(Employee employee)
        {
            HttpResponseMessage Response = null;
            
            try
            {
                if(employee!=null)
                {
                    Employee existingInstance = EmployeeRepo.GetEmployeeById(employee.id);
                    if (existingInstance == null)
                    {
                        User user = new User();
                        user.user_name = employee.email;
                        user.password = employee.first_name + "jaishu";
                        user.is_active = 1;
                        EmployeeRepo.CreateNewUser(user);
                        employee.user_id = user.id;
                        EmployeeRepo.CreateNewEmployee(employee);
                        if(employee.gender == "male" )
                        {
                            EmployeeRepo.InsertLeaveBalance(employee, Constants.male_leave_type);
                        }
                        else
                        {
                            EmployeeRepo.InsertLeaveBalance(employee, Constants.female_leave_type);
                        }
                        Response = Request.CreateResponse(HttpStatusCode.OK, new EMSResponseMessage("EMS_001", "Success", "Employee added Successfully"));
                    }
                    else
                    {
                        Response = Request.CreateResponse(HttpStatusCode.OK, new EMSResponseMessage("EMS_104", "Employee ID already exists", "Employee ID already exists"));
                    }
                }
                else
                {
                    Response = Request.CreateResponse(HttpStatusCode.OK, new EMSResponseMessage("EMS_102", "Invalid Input", "Please check input Json"));
                }
            }
            catch (Exception exception)
            {
                Debug.WriteLine(exception.Message);
                Debug.WriteLine(exception.GetBaseException());
                Response = Request.CreateResponse(HttpStatusCode.OK, new EMSResponseMessage("EMS_101", "Application Error", exception.Message));
            }
            return Response;
        }

        [Route("api/get/employee/list")]
        public HttpResponseMessage GetEmployeeList()
        {
            HttpResponseMessage Response = null;
            try
            {
                List<EmployeeModel> Emp_List = EmployeeRepo.GetEmployeeList();
                Response = Request.CreateResponse(HttpStatusCode.OK, new EMSResponseMessage("EMS_001", "Success", Emp_List));
            }
            catch (Exception exception)
            {
                Debug.WriteLine(exception.Message);
                Debug.WriteLine(exception.GetBaseException());
                Response = Request.CreateResponse(HttpStatusCode.OK, new EMSResponseMessage("EMS_101", "Application Error", exception.Message));
            }
            return Response;
        }

        [Route("api/get/employee/{e_id?}")]
        public HttpResponseMessage GetEmployeeById(int e_id)
        {
            HttpResponseMessage Response = null;
            try
            {
                if(e_id!=0)
                {
                    EmployeeModel existingInstance = EmployeeRepo.GetEmployeeDetailsById(e_id);
                    if (existingInstance != null)
                    {
                        Response = Request.CreateResponse(HttpStatusCode.OK, new EMSResponseMessage("EMS_001", "Success", existingInstance));
                    }
                    else
                    {
                        Response = Request.CreateResponse(HttpStatusCode.OK, new EMSResponseMessage("EMS_103", "Employee ID doesnot exists", "Employee ID doesnot exists"));
                    }
                }
                else
                {
                    Response = Request.CreateResponse(HttpStatusCode.OK, new EMSResponseMessage("EMS_102", "Invalid Input", "Please check input Json"));
                }
            }
            catch (Exception exception)
            {
                Debug.WriteLine(exception.Message);
                Debug.WriteLine(exception.GetBaseException());
                Response = Request.CreateResponse(HttpStatusCode.OK, new EMSResponseMessage("EMS_101", "Application Error", exception.Message));
            }
            return Response;
        }

        [HttpPost]
        [Route("api/employee/edit")]
        public HttpResponseMessage EditEmployee(Employee employee)
        {
            HttpResponseMessage response = null;
            try
            {
                if (employee != null )
                {
                    Employee existingInstance = EmployeeRepo.GetEmployeeById(employee.id);
                    if (existingInstance != null)
                    {
                        employee.user_id = existingInstance.user_id;
                        EmployeeRepo.EditEmployee(employee);
                        response = Request.CreateResponse(HttpStatusCode.OK, new EMSResponseMessage("EMS_001", "Success", "Employee details Updated successfully!"));
                    }
                    else
                    {
                        response = Request.CreateResponse(HttpStatusCode.OK, new EMSResponseMessage("EMS_103", "Invalid Employee ID", "Invalid Employee ID"));
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
