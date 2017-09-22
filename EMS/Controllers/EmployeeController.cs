using System;
using System.Collections.Generic;
using System.Data.Entity.Validation;
using System.Diagnostics;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using System.Web.Security;
using EMS.Models;
using EMS.Repository;
using EMS.Utility;
using LinqKit;

namespace EMS.Controllers
{
    public class EmployeeController : ApiController
    {
        [HttpPost]
        [Route("api/employee/create")]
        public HttpResponseMessage CreateNewEmployee(EmployeeModel employee_details)
        {
            HttpResponseMessage Response = null;

            try
            {
                if (employee_details != null && employee_details.role_id !=0 && employee_details.ctc != 0)
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
                    employee.year_of_experience = employee_details.Year_of_experience;
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

                    Employee existingInstance = EmployeeRepo.GetEmployeeById(employee.id);
                    if (existingInstance == null)
                    {
                        User user = new User();
                        user.user_name = employee.email;
                        //user.password = employee.first_name + "jaishu";
                        string Temp_password = PasswordGenerator.GeneratePassword();
                        Debug.WriteLine(Temp_password);
                        user.password = EncryptPassword.CalculateHash(Temp_password);
                        //user.password = passwod;
                        Debug.WriteLine(user.password);
                        user.is_active = 1;
                        EmployeeRepo.CreateNewUser(user);
                        employee.user_id = user.id;
                        EmployeeRepo.CreateNewEmployee(employee);
                        User_role user_role = new User_role();
                        user_role.user_id = user.id;
                        user_role.role_id = employee_details.role_id;
                        EmployeeRepo.AssignEmployeeRole(user_role);
                        if (employee.gender == "male")
                        {
                            EmployeeRepo.InsertLeaveBalance(employee, Constants.male_leave_type);
                        }
                        else
                        {
                            EmployeeRepo.InsertLeaveBalance(employee, Constants.female_leave_type);
                        }

                        Salary_Structure salary = new Salary_Structure();
                        salary = SalaryCalculation.CalculateSalaryStructure(employee_details.ctc);
                        salary.emp_id = employee_details.id;
                        salary.is_active = 1;
                        salary.from_date = DateTime.Now;
                        salary.to_date = null;
                        SalaryRepo.CreateSalaryStructure(salary);

                        Payslip payslip = new Payslip();
                        payslip = SalaryCalculation.FirstMonthSalary(employee_details.date_of_joining, salary);
                        if (payslip != null)
                        {
                            PayslipRepo.AddPayslip(payslip);
                            string username = employee.first_name + " " + employee.last_name;
                            MailHandler.PasswordMailingFunction(username, employee.email, Temp_password);
                            Response = Request.CreateResponse(HttpStatusCode.OK, new EMSResponseMessage("EMS_001", "Success", "Employee added Successfully"));
                        }
                        else
                        {
                            Response = Request.CreateResponse(HttpStatusCode.OK, new EMSResponseMessage("EMS_500", "Active Salary structure not available", "Error in salary structure generation or payslip generation"));
                        }
                    }
                    else
                    {
                        Response = Request.CreateResponse(HttpStatusCode.OK, new EMSResponseMessage("EMS_104", "Employee ID already exists", "Employee ID already exists"));
                    }
                }
                else
                {
                    Response = Request.CreateResponse(HttpStatusCode.OK, new EMSResponseMessage("EMS_102", "Invalid Input", "Please check input Json some mandatory fields are missing"));
                }
            }
            catch (DbEntityValidationException DBexception)
            {
                Debug.WriteLine(DBexception.Message);
                Debug.WriteLine(DBexception.GetBaseException());
                Response = Request.CreateResponse(HttpStatusCode.OK, new EMSResponseMessage("EMS_190", "Mandatory fields missing", DBexception.Message));
            }
            catch (Exception exception)
            {
                Debug.WriteLine(exception.Message);
                Debug.WriteLine(exception.GetBaseException());
                Response = Request.CreateResponse(HttpStatusCode.OK, new EMSResponseMessage("EMS_101", "Application Error", exception.Message));
            }
            return Response;
        }

        [Route("api/employee/list/{reportingto_id?}/{designation_id?}")]//active employee list 
        public HttpResponseMessage GetEmployeeList(int reportingto_id = 0, int designation_id = 0)//r_id reportingto_id, d_id designation_id
        {
            HttpResponseMessage Response = null;
            try
            {
                List<EmployeeModel> Emp_List = EmployeeRepo.GetEmployeeList(reportingto_id, designation_id);
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

        [Route("api/employee/available/list/{reportingto_id?}/{designation_id?}")] //(available employees = employees assigned in bench[bench project id = 1])
        public HttpResponseMessage GetAvailableEmployeeList(int reportingto_id = 0, int designation_id = 0)//r_id reportingto_id, d_id designation_id 
        {
            Stopwatch stopwatch = new Stopwatch();
            stopwatch.Start();

            HttpResponseMessage Response = null;
            try
            {
                List<EmployeeModel> Emp_List = EmployeeRepo.GetAvailableEmployeeList(reportingto_id, designation_id);
                Response = Request.CreateResponse(HttpStatusCode.OK, new EMSResponseMessage("EMS_001", "Success", Emp_List));
            }
            catch (Exception exception)
            {
                Debug.WriteLine(exception.Message);
                Debug.WriteLine(exception.GetBaseException());
                Response = Request.CreateResponse(HttpStatusCode.OK, new EMSResponseMessage("EMS_101", "Application Error", exception.Message));
            }
            stopwatch.Stop();
            Debug.WriteLine("\n\n\n"+stopwatch.Elapsed+"\n\n\n");
            return Response;
        }

        [Route("api/get/employee/{employee_id?}")]
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

        [Route("api/get/employee/byuserid/{user_id?}")]
        public HttpResponseMessage GetEmployeeByUserId(int user_id)//u_id user_id
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

        [HttpGet]
        [Route("api/employee/inactive/{employee_id?}")]
        public HttpResponseMessage InvalidEmployee(int employee_id)//e_id employee_id
        {
            HttpResponseMessage response = null;
            try
            {
                if (employee_id != 0)
                {
                    Employee existinginstance = EmployeeRepo.GetEmployeeById(employee_id);
                    if (existinginstance != null)
                    {
                        EmployeeRepo.InactiveEmployee(existinginstance);
                        response = Request.CreateResponse(HttpStatusCode.OK, new EMSResponseMessage("EMS_001", "Success", "Employee accounts closed!"));
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

        [Route("api/employee/designation/list")]
        public HttpResponseMessage GetEmpDesignationList()
        {
            HttpResponseMessage response = null;
            try
            {
                List<DesignationModel> d_list = EmployeeRepo.GetDesignationList();
                response = Request.CreateResponse(HttpStatusCode.OK, new EMSResponseMessage("EMS_001", "Success", d_list));
            }
            catch (Exception exception)
            {
                Debug.WriteLine(exception.Message);
                Debug.WriteLine(exception.GetBaseException());
                response = Request.CreateResponse(HttpStatusCode.OK, new EMSResponseMessage("EMS_101", "Application Error", exception.Message));
            }
            return response;
        }

        [Route("api/employee/reportingto/list")]
        //Manager , TeamLeader and HR list 
        public HttpResponseMessage GetReportingtoList()
        {
            HttpResponseMessage response = null;
            try
            {
                List<ReportingTo> reporting_to_list = EmployeeRepo.GetReportingtoList();
                response = Request.CreateResponse(HttpStatusCode.OK, new EMSResponseMessage("EMS_001", "Success", reporting_to_list));
            }
            catch (Exception exception)
            {
                Debug.WriteLine(exception.Message);
                Debug.WriteLine(exception.GetBaseException());
                response = Request.CreateResponse(HttpStatusCode.OK, new EMSResponseMessage("EMS_101", "Application Error", exception.Message));
            }
            return response;
        }

        [Route("api/employee/reportingto/{employee_id}")]
        public HttpResponseMessage GetReportingtoByEmpId(int employee_id)
        {
            HttpResponseMessage response = null;
            try
            {
                ReportingTo reportingto = EmployeeRepo.GetReportingtoByEmpId(employee_id);
                response = Request.CreateResponse(HttpStatusCode.OK, new EMSResponseMessage("EMS_001", "Success", reportingto));
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
        [Route("api/employee/update")]
        public HttpResponseMessage EmployeeAddandUpdate(EmployeeModel employee_details)
        {
            HttpResponseMessage Response = null;
            try
            {
                if (employee_details != null)
                {
                    Employee existingInstance = EmployeeRepo.GetEmployeeById(employee_details.id);
                    Employee employee = new Employee();
                    employee.id = employee_details.id;
                    employee.first_name = employee_details.first_name;
                    employee.last_name = employee_details.last_name;
                    employee.email = employee_details.email;
                    employee.date_of_birth = employee_details.date_of_birth;
                    employee.date_of_joining = employee_details.date_of_joining;
                    employee.contact_no = employee_details.contact_no;
                    employee.reporting_to = employee_details.reporting_to;
                    employee.year_of_experience = employee_details.Year_of_experience;
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

                    if (existingInstance == null)
                    {
                        Response = Request.CreateResponse(HttpStatusCode.OK, new EMSResponseMessage("EMS_101", "Failure", "Employee record doesnot exists!"));
                    }
                    else//(if existingInstance != null)
                    {
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
                        Response = Request.CreateResponse(HttpStatusCode.OK, new EMSResponseMessage("EMS_001", "success", "Employee record updated!"));
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

    }
}
