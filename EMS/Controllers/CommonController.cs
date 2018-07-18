using EMS.Models;
using EMS.Repository;
using EMS.Utility;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Net;
using System.Net.Http;
using System.Web.Http;

namespace EMS.Controllers
{
	public class CommonController : ApiController
    {
        [Route("api/v2/login")]
        [HttpPost]
        public HttpResponseMessage Login(User user)
        {
            HttpResponseMessage response = null;            
            try
            {
                user.password = EncryptPassword.CalculateHash(user.password);
                Dictionary<string, object> resultSet = new Dictionary<string, object>();
                if (!CommonRepo.Login(user))
                {
                    response = Request.CreateResponse(HttpStatusCode.OK, new EMSResponseMessage("EMS_301", "Invalid Username or Password", "Invalid Username or Password"));
                }
                else
                {
                    int user_id = CommonRepo.GetUserIDByUserName(user);
                    Role role = CommonRepo.GetUserRole(user_id);
                    EmployeeModel employee = EmployeeRepo.GetEmployeeDetailsByUserId(user_id);
                    resultSet.Add("employee_id", employee.id);
                    resultSet.Add("user_id", user_id);
                    resultSet.Add("UserName", employee.first_name + employee.last_name);
                    resultSet.Add("role_name", role.role_name);
                    resultSet.Add("role_id", role.id);
                    resultSet.Add("gender", employee.gender);
	                resultSet.Add("token", JwtValidation.CreateToken(user.user_name));
					response = Request.CreateResponse(HttpStatusCode.OK, new EMSResponseMessage("EMS_001", "Success", resultSet));
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

        [Route("api/v2/forgotpassword/request")]
        [HttpPost]
        public HttpResponseMessage ForgotPasswordRequest(ChangePasswordModel forgotpassword)
        {
            HttpResponseMessage response = null;
            try
            {
                Employee employee = CommonRepo.GetEmployeeIdByMailid(forgotpassword.employee_email);
                if(employee!=null)
                {
                    User user = CommonRepo.GetuserByUserId(employee.user_id);
                    if (user.is_active == 1)
                    {
                        Password_Token Token_instance = new Password_Token();
                        Token_instance.User_Id = user.id;
                        Token_instance.Token = Guid.NewGuid().ToString();
                        Token_instance.Generated_on = DateTime.Now;
                        if (CommonRepo.AddUserToken(Token_instance))
                        {
                            MailHandler.ForgotPassword(employee.first_name, employee.email, Token_instance.Token);
                            response = Request.CreateResponse(HttpStatusCode.OK, new EMSResponseMessage("EMS_001", "Reset password mail has been sent to your email address", "Reset password mail has been sent to your email address"));
                        }
                        else
                        {
                            response = Request.CreateResponse(HttpStatusCode.OK, new EMSResponseMessage("EMS_101", "Application Error", "Error while creating token"));
                        }
                    }
                    else
                    {
                        response = Request.CreateResponse(HttpStatusCode.OK, new EMSResponseMessage("EMS_303", "Invalid User", "Invalid User"));
                    }
                }
                else
                {
                    response = Request.CreateResponse(HttpStatusCode.OK, new EMSResponseMessage("EMS_310", "Invalid MailID", "Invalid MailID"));
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
        [Route("api/v2/forgotpassword/validation")]
        public HttpResponseMessage ForgotPasswordValidation(ChangePasswordModel ForgotPassword)
        {
            HttpResponseMessage response = null;
            try
            {
                Password_Token TokenObject = CommonRepo.GetActiveTokenObjectByToken(ForgotPassword.token);
                if(TokenObject != null && TokenObject.Generated_on <= DateTime.Now && TokenObject.Generated_on.AddDays(1) >= DateTime.Now)
                {
                    response = Request.CreateResponse(HttpStatusCode.OK, new EMSResponseMessage("EMS_001", "Success", "Valid Token"));
                }
                else
                {
                    response = Request.CreateResponse(HttpStatusCode.OK, new EMSResponseMessage("EMS_309", "Token Expired", "Token Expired"));
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
        [Route("api/v2/forgotpassword/update")]
        public HttpResponseMessage ForgotPasswordUpdate(ChangePasswordModel ForgotPassword)
        {
            HttpResponseMessage response = null;
            try
            {
                if(ForgotPassword.token!= null && (ForgotPassword.new_password != null || ForgotPassword.new_password != "") && ForgotPassword.new_password == ForgotPassword.confirm_password)
                {
                    Password_Token TokenObject = CommonRepo.GetActiveTokenObjectByToken(ForgotPassword.token);
                    if (TokenObject != null && TokenObject.Generated_on <= DateTime.Now && TokenObject.Generated_on.AddDays(1) >= DateTime.Now)
                    {
                        User user_instance = CommonRepo.GetuserByUserId(TokenObject.User_Id);
                        if (user_instance.is_active == 1 && TokenObject.Generated_on <= DateTime.Now && TokenObject.Generated_on.AddDays(1) >= DateTime.Now)
                        {
                            user_instance.password = EncryptPassword.CalculateHash(ForgotPassword.new_password);
                            if (CommonRepo.EditUserPassword(user_instance))
                            {
                                CommonRepo.DeletePasswordToken(user_instance.id);
                                EmployeeModel Employee_Insatnce = EmployeeRepo.GetEmployeeDetailsByUserId(user_instance.id);
                                MailHandler.ChangePasswordIntimation(Employee_Insatnce.first_name, Employee_Insatnce.email);
                                response = Request.CreateResponse(HttpStatusCode.OK, new EMSResponseMessage("EMS_001", "Password changed sucessfully", "Password changed sucessfully"));
                            }
                        }
                        else
                        {
                            response = Request.CreateResponse(HttpStatusCode.OK, new EMSResponseMessage("EMS_303", "Invalid User", "Invalid User"));
                        }
                    }
                    else
                    {
                        response = Request.CreateResponse(HttpStatusCode.OK, new EMSResponseMessage("EMS_309", "Token Expired", "Token Expired"));
                    }
                }
                else
                {
                    response = Request.CreateResponse(HttpStatusCode.OK, new EMSResponseMessage("EMS_190", "Invalid Input", "Invalid Input"));
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
