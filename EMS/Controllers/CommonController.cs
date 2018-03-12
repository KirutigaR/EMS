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
using System.Threading.Tasks;
using System.Web;
using System.Data;
using System.Data.SqlClient;
using System.Data.OleDb;
using System.IO;
using System.Configuration;

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
                    response = Request.CreateResponse(HttpStatusCode.OK, new EMSResponseMessage("EMS_310", "Invalid Mail ID", "Invalid Mail ID"));
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

        [HttpPost]
        [Route("api/v2/bulkupload")]
        public async Task<HttpResponseMessage> UpdateEmployeeLeaveBalance()
        {
            // Check if the request contains multipart/form-data.
            if (!Request.Content.IsMimeMultipartContent())
            {
                throw new HttpResponseException(HttpStatusCode.UnsupportedMediaType);
            }

            string root = HttpContext.Current.Server.MapPath("~/App_Data");
            var provider = new MultipartFormDataStreamProvider(root);

            DataSet ds = new DataSet();
            DataSet ospDS = new DataSet();
            SqlConnection con = null;
            OleDbDataAdapter oda = null;
            SqlBulkCopy objbulk = null;

            try
            {
                // Read the form data.
                await Request.Content.ReadAsMultipartAsync(provider);

                // This illustrates how to get the file names.
                foreach (MultipartFileData file in provider.FileData)
                {
                    Trace.WriteLine(file.Headers.ContentDisposition.FileName);
                    Trace.WriteLine("Server file path: " + file.LocalFileName);
                }
                MultipartFileData fileData = provider.FileData.First();
                string fileName = fileData.Headers.ContentDisposition.FileName;

                if (fileName.StartsWith("\"") && fileName.EndsWith("\""))
                {
                    fileName = fileName.Trim('"');
                }

                string destinationFileName = fileName + Guid.NewGuid().ToString() + Path.GetExtension(fileName.Replace(" ", "_"));
                //string destinationFileName = fileName;
                // Use the Path.Combine method to safely append the file name to the path.
                // Will not overwrite if the destination file already exists.
                File.Move(fileData.LocalFileName, Path.Combine(HttpContext.Current.Server.MapPath("~/App_Data/Uploads/"), destinationFileName));
                File.Delete(fileData.LocalFileName);

                string fileExtension = Path.GetExtension(destinationFileName);
                string fileLocation = HttpContext.Current.Server.MapPath("~/App_Data/Uploads/") + destinationFileName;

                if (fileExtension == ".xls" || fileExtension == ".xlsx")
                {

                    string excelConnectionString = string.Empty;
                    excelConnectionString = "Provider=Microsoft.ACE.OLEDB.12.0;Data Source=" + fileLocation + ";Extended Properties=\"Excel 12.0;HDR=Yes;IMEX=2\"";
                    //connection String for xls file format.
                    if (fileExtension == ".xls")
                    {
                        excelConnectionString = "Provider=Microsoft.Jet.OLEDB.4.0;Data Source=" + fileLocation + ";Extended Properties=\"Excel 8.0;HDR=Yes;IMEX=2\"";
                    }
                    //connection String for xlsx file format.
                    else if (fileExtension == ".xlsx")
                    {
                        excelConnectionString = "Provider=Microsoft.ACE.OLEDB.12.0;Data Source=" + fileLocation + ";Extended Properties=\"Excel 12.0;HDR=Yes;IMEX=2\"";
                    }
                    //Create Connection to Excel work book and add oledb namespace
                    OleDbConnection excelConnection = new OleDbConnection(excelConnectionString);
                    excelConnection.Open();
                    string[] sheets = Utility.GetSheet.GetSheetName(excelConnection);
                    bool flag = false;
                    if (sheets.Length == 0)
                    {
                        return Request.CreateResponse(HttpStatusCode.OK, new EMSResponseMessage("EMS_304", "No Sheets available in the excel file", "No Sheets available in the excel file"));
                    }
                    else
                    {
                        string query = string.Format("Select * FROM [{0}]", sheets.FirstOrDefault());
                        oda = new OleDbDataAdapter(query, excelConnection);
                        oda.Fill(ds);
                        DataTable Exceldt = ds.Tables[0];

                        string sqlconn = ConfigurationManager.ConnectionStrings["dbcon"].ConnectionString;
                        con = new SqlConnection(sqlconn);
                        //creating object of SqlBulkCopy    
                        objbulk = new SqlBulkCopy(con);
                        //assigning Destination table name    

                        objbulk.DestinationTableName = "EMS_Leave_Balance_Temp";
                        //Mapping Table column    
                        objbulk.ColumnMappings.Add("Employee Id", "Employee_id");
                        objbulk.ColumnMappings.Add("CL", "CL");
                        objbulk.ColumnMappings.Add("EL", "EL");
                        objbulk.ColumnMappings.Add("ML", "ML");
                        objbulk.ColumnMappings.Add("LOP", "LOP");
                        objbulk.ColumnMappings.Add("WFH", "WFH");

                        //inserting Datatable Records to DataBase    
                        con.Open();
                        objbulk.WriteToServer(Exceldt);
                        con.Close();
                        flag = true;
                    }

                    if (flag && CommonRepo.LoadDataFromTable())
                    {
                        return Request.CreateResponse(HttpStatusCode.OK, new EMSResponseMessage("EMS_306", "Leave Balance updated successfully", "Leave Balance updated successfully"));
                    }
                    else
                    {
                        return Request.CreateResponse(HttpStatusCode.OK, new EMSResponseMessage("EMS_305", "Some problem in Stored Procedure", "Some problem in Stored Procedure"));
                    }
                }
                else
                {
                    return Request.CreateResponse(HttpStatusCode.OK, new EMSResponseMessage("EMS_307", "Invalid File", "Invalid File"));
                }
            }
            catch (OleDbException e)
            {
                Debug.WriteLine(e.Message);
                Debug.WriteLine(e.GetBaseException());
                return Request.CreateResponse(HttpStatusCode.OK, new EMSResponseMessage("EMS_308", "Invalid Excel Sheet", e.Message));
            }
            catch (InvalidOperationException e)
            {
                Debug.WriteLine(e.Message);
                Debug.WriteLine(e.GetBaseException());
                return Request.CreateResponse(HttpStatusCode.OK, new EMSResponseMessage("EMS_308", "Mandatory columns are missing", e.Message));
            }
            catch (IOException IOException)
            {
                Debug.WriteLine(IOException.Message);
                Debug.WriteLine(IOException.GetBaseException());
                return Request.CreateResponse(HttpStatusCode.OK, new EMSResponseMessage("EMS_308", "Please select a file to upload", IOException.Message));
            }
            catch (Exception e)
            {
                Debug.WriteLine(e.Message);
                Debug.WriteLine(e.GetBaseException());
                return Request.CreateResponse(HttpStatusCode.OK, new EMSResponseMessage("EMS_308", "Application Error",e.Message));
            }
        }
        [Route("api/v2/changepassword")]
        [HttpPost]
        public HttpResponseMessage ChangePassword(ChangePasswordModel changepassword)
        {
            HttpResponseMessage response = null;
            try
            {
                Employee employee = EmployeeRepo.GetEmployeeById(changepassword.employee_id);
                User user_instance = CommonRepo.GetuserByUserId(employee.user_id);
                if(user_instance.is_active==1)
                {
                    if (changepassword.new_password == changepassword.confirm_password)
                    {
                        if(changepassword.oldpassword == changepassword.new_password)
                        {
                            response = Request.CreateResponse(HttpStatusCode.OK, new EMSResponseMessage("EMS_515", "Current Password and New password should be different", "Current Password and New password should be different"));
                        }
                        else if (EncryptPassword.CalculateHash(changepassword.oldpassword) == user_instance.password)
                        {
                            user_instance.password = EncryptPassword.CalculateHash(changepassword.new_password);
                            CommonRepo.EditUserPassword(user_instance);
                            MailHandler.ChangePasswordIntimation(employee.first_name, employee.email);
                            response = Request.CreateResponse(HttpStatusCode.OK, new EMSResponseMessage("EMS_001", "Password changed sucessfully", "Password changed sucessfully"));
                        }
                        else
                        {
                            response = Request.CreateResponse(HttpStatusCode.OK, new EMSResponseMessage("EMS_514", "Invalid Current Password", "Invalid Current Password"));
                        }
                    }
                    else
                    {
                        response = Request.CreateResponse(HttpStatusCode.OK, new EMSResponseMessage("EMS_302", "New Password and Confirm Password should be same", "New Password and Confirm Password should be same"));
                    }
                }
                else
                {
                    response = Request.CreateResponse(HttpStatusCode.OK, new EMSResponseMessage("EMS_303", "Access Denied", "Access Denied"));
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
