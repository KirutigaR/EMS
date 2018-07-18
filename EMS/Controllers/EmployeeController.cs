using EMS.Models;
using EMS.Repository;
using EMS.Utility;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.Entity.Validation;
using System.Data.OleDb;
using System.Data.SqlClient;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Web;
using System.Web.Http;
using EMS.Filters;

namespace EMS.Controllers
{
	[AuthenticationFilter]
	public class EmployeeController : ApiController
    {
        [HttpPost]
        [Route("api/v2/employee/create")]
        public HttpResponseMessage CreateNewEmployee(EmployeeModel employee_details)
        {
            HttpResponseMessage Response = null;

            try
            {
                if (employee_details != null && employee_details.role_id != 0 /*&& employee_details.ctc != 0 && employee_details.id != 0 */&& employee_details.reporting_to != 0 && employee_details.designation_id != 0)
                {
                    //Employee existingInstance = EmployeeRepo.GetEmployeeById(employee_details.id);
                    List<Employee> employeeByMailid = EmployeeRepo.GetEmployeeByMailId(employee_details.email);
                    //if (existingInstance != null)
                    //{
                    //    Response = Request.CreateResponse(HttpStatusCode.OK, new EMSResponseMessage("EMS_402", "Employee ID already exists", "Employee ID already exists"));
                    //    return Response;
                    //}
                    if (employeeByMailid.Count != 0)
                    {
                        Response = Request.CreateResponse(HttpStatusCode.OK, new EMSResponseMessage("EMS_402", "Mail ID already exists", "Mail ID already exists"));
                        return Response;
                    }
                    bool isEmail = Regex.IsMatch(employee_details.email,
                                    @"^([0-9a-zA-Z]" + //Start with a digit or alphabetical
                                    @"([\+\-_\.][0-9a-zA-Z]+)*" + // No continuous or ending +-_. chars in email
                                    @")+" +
                                    @"@(([0-9a-zA-Z][-\w]*[0-9a-zA-Z]*\.)+[a-zA-Z0-9]{2,17})$"
                                        , RegexOptions.IgnoreCase);
                    if (isEmail != true)
                    {
                        Response = Request.CreateResponse(HttpStatusCode.OK, new EMSResponseMessage("EMS_402", "Enter valid MailId", "Enter valid MailId"));
                        return Response;
                    }
                    if ((employee_details.date_of_birth.Year > (DateTime.Now.Year - 21)))
                    {
                        Response = Request.CreateResponse(HttpStatusCode.OK, new EMSResponseMessage("EMS_402", "Employee age is below 21 years", "Employee age is below 21 years"));
                        return Response;
                    }

                    Employee employee = new Employee();
                    //employee.id = employee_details.id;
                    employee.first_name = employee_details.first_name;
                    employee.last_name = employee_details.last_name;
                    employee.email = employee_details.email;
                    employee.date_of_birth = employee_details.date_of_birth;
                    employee.date_of_joining = employee_details.date_of_joining;
                    employee.contact_no = employee_details.contact_no;
                    employee.reporting_to = employee_details.reporting_to;
                    employee.year_of_experience = Decimal.Parse(employee_details.Year_of_experience);
                    employee.gender = employee_details.gender;
                    employee.pan_no = employee_details.pan_no;
                    employee.bank_account_no = employee_details.bank_account_no;
                    employee.emergency_contact_no = employee_details.emergency_contact_no;
                    employee.emergency_contact_person = employee_details.emergency_contact_person;
                    employee.PF_no = employee_details.PF_no;
                    employee.medical_insurance_no = employee_details.medical_insurance_no;
                    employee.blood_group = employee_details.blood_group;
                    employee.designation = employee_details.designation_id;
                    employee.created_on = DateTime.Now;

                        User user = new User();
                        user.user_name = employee.email;
                        string Temp_password = PasswordGenerator.GeneratePassword();
                        //Debug.WriteLine(Temp_password);
                        user.password = EncryptPassword.CalculateHash(Temp_password);
                        //Debug.WriteLine(user.password);
                        user.is_active = 1;
                        EmployeeRepo.CreateNewUser(user);
                        employee.user_id = user.id;
                        EmployeeRepo.CreateNewEmployee(employee);
                        User_role user_role = new User_role();
                        user_role.user_id = user.id;
                        user_role.role_id = employee_details.role_id;
                        EmployeeRepo.AssignEmployeeRole(user_role);
                        if (employee.gender.ToLower() == "male")
                        {
                            EmployeeRepo.InsertLeaveBalance(employee, Constants.MALE_LEAVE_TYPES);
                        }
                        else
                        {
                            EmployeeRepo.InsertLeaveBalance(employee, Constants.FEMALE_LEAVE_TYPES);
                        }

                        Salary_Structure salary = new Salary_Structure();
                        salary = SalaryCalculation.CalculateSalaryStructure(employee_details.ctc);
                        salary.emp_id = employee.id;
                        salary.is_active = 1;
                        salary.from_date = DateTime.Now;
                        salary.to_date = null;
                        SalaryRepo.CreateSalaryStructure(salary);

                        Payslip payslip = new Payslip();
                        payslip = SalaryCalculation.FirstMonthSalary(employee_details.date_of_joining, salary);
                        if (payslip != null)
                        {
                            PayslipRepo.AddPayslip(payslip);
                        }
                        else
                        {
                            Response = Request.CreateResponse(HttpStatusCode.OK, new EMSResponseMessage("EMS_401", "Error in salary structure generation or payslip generation", "Error in salary structure generation or payslip generation"));
                            return Response;
                        }
                        MailHandler.PasswordMailingFunction(employee.first_name, employee.email, Temp_password);
                        Response = Request.CreateResponse(HttpStatusCode.OK, new EMSResponseMessage("EMS_001", "Employee added Successfully", "Employee added Successfully"));
                }
                else
                {
                    Response = Request.CreateResponse(HttpStatusCode.OK, new EMSResponseMessage("EMS_190", "Invalid Input - check the fileds", "Invalid Input - check the fileds"));
                }
            }
            catch (DbEntityValidationException DBexception)
            {
                Debug.WriteLine(DBexception.Message);
                Debug.WriteLine(DBexception.GetBaseException());
                Response = Request.CreateResponse(HttpStatusCode.OK, new EMSResponseMessage("EMS_190", "Mandatory fields missing or Type mismatch", DBexception.Message));
            }
            catch (Exception exception)
            {
                Debug.WriteLine(exception.Message);
                Debug.WriteLine(exception.GetBaseException());
                Response = Request.CreateResponse(HttpStatusCode.OK, new EMSResponseMessage("EMS_101", "Application Error", exception.Message));
            }
            return Response;
        }

        [Route("api/v2/employee/list/{reportingto_id?}/{designation_id?}")]//active employee list 
        public HttpResponseMessage GetEmployeeList(int reportingto_id = 0, int designation_id = 0)
        {
            HttpResponseMessage Response = null;
            try
            {
                Response = Request.CreateResponse(HttpStatusCode.OK, new EMSResponseMessage("EMS_001", "Success", EmployeeRepo.GetEmployeeList(reportingto_id, designation_id)));
            }
            catch (Exception exception)
            {
                Debug.WriteLine(exception.Message);
                Debug.WriteLine(exception.GetBaseException());
                Response = Request.CreateResponse(HttpStatusCode.OK, new EMSResponseMessage("EMS_101", "Application Error", exception.Message));
            }
            return Response;
        }

        [Route("api/v2/employee/available/list/{reportingto_id?}/{designation_id?}")] //(available employees = employees assigned in bench[bench project id = 1])
        public HttpResponseMessage GetAvailableEmployeeList(int reportingto_id = 0, int designation_id = 0)//r_id reportingto_id, d_id designation_id 
        {
            HttpResponseMessage Response = null;
            try
            {
                Response = Request.CreateResponse(HttpStatusCode.OK, new EMSResponseMessage("EMS_001", "Success", EmployeeRepo.GetAvailableEmployeeList(reportingto_id, designation_id)));
            }
            catch (Exception exception)
            {
                Debug.WriteLine(exception.Message);
                Debug.WriteLine(exception.GetBaseException());
                Response = Request.CreateResponse(HttpStatusCode.OK, new EMSResponseMessage("EMS_101", "Application Error", exception.Message));
            }
            return Response;
        }

        
        [Route("api/v2/get/employee/{employee_id?}")]
        public HttpResponseMessage GetEmployeeById(int employee_id)//e_id employee_id
        {
            HttpResponseMessage Response = null;
            try
            {
                if (employee_id != 0)
                {
                    EmployeeModel existingInstance = EmployeeRepo.GetEmployeeDetailsById(employee_id);
                    if (existingInstance != null)
                    {
                        Response = Request.CreateResponse(HttpStatusCode.OK, new EMSResponseMessage("EMS_001", "Success", existingInstance));
                    }
                    else
                    {
                        Response = Request.CreateResponse(HttpStatusCode.OK, new EMSResponseMessage("EMS_403", "Invalid Employee ID", "Invalid Employee ID"));
                    }
                }
                else
                {
                    Response = Request.CreateResponse(HttpStatusCode.OK, new EMSResponseMessage("EMS_102", "Invalid Employee ID", "Please check input Json"));
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

        [Route("api/v2/get/employee/byuserid/{user_id?}")]
        public HttpResponseMessage GetEmployeeByUserId(int user_id)
        {
            HttpResponseMessage Response = null;
            try
            {
                if (user_id != 0)
                {
                    EmployeeModel existingInstance = EmployeeRepo.GetEmployeeDetailsByUserId(user_id);
                    if (existingInstance != null)
                    {
                        Response = Request.CreateResponse(HttpStatusCode.OK, new EMSResponseMessage("EMS_001", "Success", existingInstance));
                    }
                    else
                    {
                        Response = Request.CreateResponse(HttpStatusCode.OK, new EMSResponseMessage("EMS_403", "Invalid Employee ID", "Invalid Employee ID"));
                    }
                }
                else
                {
                    Response = Request.CreateResponse(HttpStatusCode.OK, new EMSResponseMessage("EMS_102", "Invalid Employee ID", "Please check input Json"));
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

        [HttpGet]
        [Route("api/v2/employee/inactive/{employee_id?}")]
        public HttpResponseMessage InvalidEmployee(int employee_id)
        {
            HttpResponseMessage response = null;
            try
            {
                if (employee_id != 0)
                {
                    Employee existinginstance = EmployeeRepo.GetEmployeeById(employee_id);
                    if (existinginstance != null)
                    {
                        EmployeeRepo.InactiveEmployee(existinginstance.user_id);
                        response = Request.CreateResponse(HttpStatusCode.OK, new EMSResponseMessage("EMS_001", "Success", "Employee accounts closed!"));
                    }
                    else
                    {
                        response = Request.CreateResponse(HttpStatusCode.OK, new EMSResponseMessage("EMS_403", "Invalid Employee ID", "Invalid Employee ID"));
                    }
                }
                else
                {
                    response = Request.CreateResponse(HttpStatusCode.OK, new EMSResponseMessage("EMS_102", "Invalid Employee ID", "Please check input Json"));
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

        [Route("api/v2/employee/designation/list")]
        public HttpResponseMessage GetEmpDesignationList()
        {
            HttpResponseMessage response = null;
            try
            {
                response = Request.CreateResponse(HttpStatusCode.OK, new EMSResponseMessage("EMS_001", "Success", EmployeeRepo.GetDesignationList()));
            }
            catch (Exception exception)
            {
                Debug.WriteLine(exception.Message);
                Debug.WriteLine(exception.GetBaseException());
                response = Request.CreateResponse(HttpStatusCode.OK, new EMSResponseMessage("EMS_101", "Application Error", exception.Message));
            }
            return response;
        }

        [Route("api/v2/employee/reportingto/list")]//Manager , TeamLeader and HR list 
        public HttpResponseMessage GetReportingtoList()
        {
            HttpResponseMessage response = null;
            try
            {
                List<ReportingTo> reporting_to_list = EmployeeRepo.GetReportingtoList();
                if(reporting_to_list != null)
                {
                    response = Request.CreateResponse(HttpStatusCode.OK, new EMSResponseMessage("EMS_001", "Success", reporting_to_list));
                }   
                else
                {
                    response = Request.CreateResponse(HttpStatusCode.OK, new EMSResponseMessage("EMS_405", "No active reporters found", "No active reporters found"));
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

        [Route("api/v2/employee/reportingto/{employee_id}")]
        public HttpResponseMessage GetReportingtoByEmpId(int employee_id)
        {
            HttpResponseMessage response = null;
            try
            {
                response = Request.CreateResponse(HttpStatusCode.OK, new EMSResponseMessage("EMS_001", "Success", EmployeeRepo.GetReportingtoByEmpId(employee_id)));
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
        [Route("api/v2/employee/update")]
        public HttpResponseMessage EmployeeUpdate(EmployeeModel employee_details)
        {
            HttpResponseMessage Response = null;
            try
            {
                if (employee_details != null)
                {
                    Employee existingInstance = EmployeeRepo.GetEmployeeById(employee_details.id);
                    if (existingInstance == null)
                    {
                        Response = Request.CreateResponse(HttpStatusCode.OK, new EMSResponseMessage("EMS_403", "Employee record doesnot exists!", "Employee record doesnot exists!"));
                    }
                    else//(if existingInstance != null)
                    {
                        Employee employee = new Employee();
                        employee.id = employee_details.id;
                        employee.first_name = employee_details.first_name;
                        employee.last_name = employee_details.last_name;
                        employee.email = employee_details.email;
                        employee.date_of_birth = employee_details.date_of_birth;
                        employee.date_of_joining = employee_details.date_of_joining;
                        employee.contact_no = employee_details.contact_no;
                        employee.reporting_to = employee_details.reporting_to;
                        employee.year_of_experience = Decimal.Parse(employee_details.Year_of_experience);
                        employee.gender = employee_details.gender;
                        employee.pan_no = employee_details.pan_no;
                        employee.bank_account_no = employee_details.bank_account_no;
                        employee.emergency_contact_no = employee_details.emergency_contact_no;
                        employee.emergency_contact_person = employee_details.emergency_contact_person;
                        employee.PF_no = employee_details.PF_no;
                        employee.medical_insurance_no = employee_details.medical_insurance_no;
                        employee.blood_group = employee_details.blood_group;
                        employee.designation = employee_details.designation_id;
                        employee.created_on = existingInstance.created_on;
                        
                        employee.user_id = existingInstance.user_id;

                        User_role userrole_instance = EmployeeRepo.GetUserRoleByUserid(employee.user_id);
                        userrole_instance.role_id = employee_details.role_id;
                        EmployeeRepo.UpdateUserRole(userrole_instance);

                        Salary_Structure active_sal_instance = SalaryRepo.GetSalaryStructureByEmpId(employee.id);

                        if (active_sal_instance != null && active_sal_instance.ctc != employee_details.ctc )
                        {
                                active_sal_instance.is_active = 0;
                                active_sal_instance.to_date = DateTime.Now;
                                SalaryRepo.UpdateSalaryStructure(active_sal_instance);

                                Salary_Structure new_sal_structure = new Salary_Structure();
                                new_sal_structure = SalaryCalculation.CalculateSalaryStructure(employee_details.ctc);
                                new_sal_structure.emp_id = employee.id;
                                new_sal_structure.is_active = 1;
                                new_sal_structure.from_date = DateTime.Now;
                                new_sal_structure.to_date = null;
                                SalaryRepo.CreateSalaryStructure(new_sal_structure);
                        }
                        EmployeeRepo.EditEmployee(employee);
                        Response = Request.CreateResponse(HttpStatusCode.OK, new EMSResponseMessage("EMS_001", "Employee details updated successfully!", "Employee details updated successfully!"));
                    }//(existingInstance != null) ELSE PART
                }//employee_details != null IF PART 
                else //IF employee_details == null
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

        [HttpGet]
        [Route("api/v2/get/last/employee/id")]
        public HttpResponseMessage GetLastEmployeeId(int employee_id = 0, string employee_name = null)
        {
            HttpResponseMessage Response = null;
            try
            {
                Response = Request.CreateResponse(new EMSResponseMessage("EMS_001", "Success", EmployeeRepo.GetLastAddedEmployee()));
            }
            catch (Exception exception)
            {
                Debug.WriteLine(exception.Message);
                Debug.WriteLine(exception.GetBaseException());
                Response = Request.CreateResponse(HttpStatusCode.OK, new EMSResponseMessage("EMS_101", "Application Error", exception.Message));
            }
            return Response;
        }

        [HttpGet]
        [Route("api/v2/get/common/employee/details")]
        public HttpResponseMessage GetCommonEmployeeDetails()
        {
            HttpResponseMessage Response = null;
            try
            {
                List<EmployeeModel> Employee_details = EmployeeRepo.GetEmployeeList(0, 0);
                if (Employee_details.Count != 0)
                {
                    //var itemToRemove = Employee_details.Single(r => r.id == /*employee id*/);
                    //Employee_details.Remove(itemToRemove);
                    Response = Request.CreateResponse(HttpStatusCode.OK, new EMSResponseMessage("EMS_001", "Success", Employee_details.OrderBy(x => x.first_name)));
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
				return Request.CreateResponse(HttpStatusCode.OK, new EMSResponseMessage("EMS_308", "Application Error", e.Message));
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
				if (user_instance.is_active == 1)
				{
					if (changepassword.new_password == changepassword.confirm_password)
					{
						if (changepassword.oldpassword == changepassword.new_password)
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
