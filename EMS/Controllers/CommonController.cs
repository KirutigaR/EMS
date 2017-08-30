using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using System.Diagnostics;
using EMS.Repository;
using EMS.Utility;
using EMS.Models;

namespace EMS.Controllers
{
    public class CommonController : ApiController
    {
        [Route("api/login")]
        [HttpPost]
        public HttpResponseMessage Login(User user)
        {
            HttpResponseMessage response = null;            
            try
            {
                user.password = EncryptPassword.CalculateHash(user.password);
                Dictionary<string, object> resultSet = new Dictionary<string, object>(); 
                bool b = CommonRepo.Login(user);
                
                if (!b)
                {
                    response = Request.CreateResponse(HttpStatusCode.OK, new EMSResponseMessage("EMS_120", "Username or password didn't match", "login failed"));
                }
                else
                {

                    int user_id = CommonRepo.GetUserID(user);
                    string user_role = CommonRepo.GetUserRole(user_id);
                    EmployeeModel employee = EmployeeRepo.GetEmployeeDetailsByUserId(user_id);
                    resultSet.Add("employee_id", employee.id);
                    resultSet.Add("user_id", user_id);
                    resultSet.Add("UserName", employee.first_name + employee.last_name);
                    resultSet.Add("role", user_role);
                    response = Request.CreateResponse(HttpStatusCode.OK, new EMSResponseMessage("EMS_001", "Success", resultSet));
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
