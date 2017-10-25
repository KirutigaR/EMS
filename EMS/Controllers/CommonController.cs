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
                    response = Request.CreateResponse(HttpStatusCode.OK, new EMSResponseMessage("EMS_301", "Username and password didn't match or Invalid user login", "login failed"));
                }
                else
                {

                    int user_id = CommonRepo.GetUserID(user);
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
                response = Request.CreateResponse(HttpStatusCode.OK, exception.Message);
            }
            return response;
        }
        [Route("api/forgotpassword")]
        [HttpPost]
        public HttpResponseMessage ForgotPassword(ChangePasswordModel forgotpassword)
        {
            HttpResponseMessage response = null;
            try
            {
                Employee employee = EmployeeRepo.GetEmployeeById(forgotpassword.employee_id);
                User user = CommonRepo.GetuserById(employee.user_id);
                if(user.is_active==1)
                {
                    if (forgotpassword.new_password == forgotpassword.confirm_password)
                    {
                        user.password = EncryptPassword.CalculateHash(forgotpassword.new_password);
                        CommonRepo.EditUserDetails(user);
                        string user_name = employee.first_name +" "+ employee.last_name;
                        MailHandler.ChangePasswordIntimation(user_name, employee.email);
                        response = Request.CreateResponse(HttpStatusCode.OK, new EMSResponseMessage("EMS_001", "your password has been changed successfully", "your password has been changed successfully"));
                    }
                    else
                    {
                        response = Request.CreateResponse(HttpStatusCode.OK, new EMSResponseMessage("EMS_302", "New Password and Confirm Password must be same", "New Password and Confirm Password must be same"));
                    }
                }
                else
                {
                    response = Request.CreateResponse(HttpStatusCode.OK, new EMSResponseMessage("EMS_303", "Invalid user login", "Invalid user login"));
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

        [HttpPost]
        [Route("api/bulkupload")]
        public async Task<HttpResponseMessage> PostFormData()
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

                    if (flag)
                    {
                        if (!CommonRepo.LoadDataFromTable())
                        {
                            return Request.CreateResponse(HttpStatusCode.OK, new EMSResponseMessage("EMS_305", "Some problem in Stored Procedure", "Some problem in Stored Procedure"));
                        }
                    }
                    return Request.CreateResponse(HttpStatusCode.OK, new EMSResponseMessage("EMS_306", "Leave Balance updated successfully", "Leave Balance updated successfully"));

                }
                else
                {
                    return Request.CreateResponse(HttpStatusCode.OK, new EMSResponseMessage("EMS_307", "Please upload valid excel file", "Please upload valid excel file"));
                }
            }
            catch (Exception e)
            {
                Debug.WriteLine(e.Message);
                Debug.WriteLine(e.GetBaseException());
                return Request.CreateResponse(HttpStatusCode.OK, new EMSResponseMessage("EMS_308", "Application Error",e.Message));
            }
        }
        [Route("api/changepassword")]
        [HttpPost]
        public HttpResponseMessage ChangePassword(ChangePasswordModel changepassword)
        {
            HttpResponseMessage response = null;
            try
            {
                Employee employee = EmployeeRepo.GetEmployeeById(changepassword.employee_id);
                User user_instance = LeaveRepo.GetUserById(employee.user_id);
                if(user_instance.is_active==1)
                {
                    if (changepassword.new_password == changepassword.confirm_password)
                    {
                        if (EncryptPassword.CalculateHash(changepassword.oldpassword) == user_instance.password)
                        {
                            user_instance.password = EncryptPassword.CalculateHash(changepassword.new_password);
                            LeaveRepo.EditUserPassword(user_instance);
                            string user_name = employee.first_name + " " + employee.last_name;
                            MailHandler.ChangePasswordIntimation(user_name, employee.email);
                            response = Request.CreateResponse(HttpStatusCode.OK, new EMSResponseMessage("EMS_001", "Password changed sucessfully", "Password changed sucessfully"));
                        }
                        else
                        {
                            response = Request.CreateResponse(HttpStatusCode.OK, new EMSResponseMessage("EMS_514", "Entered Old Password is WRONG", "Entered Old Password is WRONG"));
                        }
                    }
                    else
                    {
                        response = Request.CreateResponse(HttpStatusCode.OK, new EMSResponseMessage("EMS_302", "New Password and Confirm Password must be same", "New Password and Confirm Password must be same"));
                    }
                }
                else
                {
                    response = Request.CreateResponse(HttpStatusCode.OK, new EMSResponseMessage("EMS_303", "Invalid user login", "Invalid user login"));
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
