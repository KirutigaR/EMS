using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using EMS.Repository;
using EMS.Utility;

namespace EMS.Controllers
{
    public class EmployeeController : ApiController
    {

        [HttpPost]
        [Route("api/employee/create")]
        public HttpResponseMessage CreateNewEmployee(Employee Details)
        {
            HttpResponseMessage Response = null;
            
            try
            {
                if(Details!=null)
                {
                    User user_details = new User();
                    user_details.user_name = Details.email;
                    user_details.password = Details.first_name+"jaishu"; 
                    EmployeeRepo.CreateNewUser(user_details);
                    Details.user_id = user_details.id;
                    EmployeeRepo.CreateNewEmployee(Details);
                    Response = Request.CreateResponse(HttpStatusCode.OK, new EMSResponseMessage("EMS_001", "Success", "Employee added Successfully"));
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

        [Route("api/employee/list")]
        public HttpResponseMessage GetEmployeeList()
        {
            HttpResponseMessage Response = null;
            try
            {
                List<Employee> Emp_List = EmployeeRepo.GetEmployeeList();
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

        [Route("api/employee/{e_id?}")]
        public HttpResponseMessage GetEmployeeById(int e_id)
        {
            HttpResponseMessage Response = null;
            try
            {
                if(e_id!=0)
                {
                    Employee employee = EmployeeRepo.GetEmployeeById(e_id);
                    Response = Request.CreateResponse(HttpStatusCode.OK, new EMSResponseMessage("EMS_001", "Success", employee));
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

        [Route("api/employee/edit")]
        public HttpResponseMessage EditEmployee(int e_id)
        {
            HttpResponseMessage Response = null;
            try
            {
                if(e_id!=0)
                {
                    Employee employee_details = EmployeeRepo.GetEmployeeById(e_id);
                    EmployeeRepo.EditEmployee(employee_details);
                    Response = Request.CreateResponse(HttpStatusCode.OK, new EMSResponseMessage("EMS_001", "Success", "Employee details Updated successfully!"));
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
    }
}
