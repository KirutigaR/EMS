using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using EMS.Repository;
using EMS.Utility;
using EMS.Models;

namespace EMS.Controllers
{
    public class SalaryController : ApiController
    {
        [HttpGet]
        [Route("api/salarystructure/update/{employee_id?}/{ctc?}")]
        public HttpResponseMessage UpdateSalaryStructure(int employee_id, decimal ctc)
        {
            HttpResponseMessage response = null;
            try
            {
                if(employee_id != 0 && EmployeeRepo.GetEmployeeStatusById(employee_id) !=0 && ctc!=0)
                {
                    Salary_Structure active_instance = SalaryRepo.GetSalaryStructureByEmpId(employee_id);
                    if (active_instance != null)
                    {
                        active_instance.is_active = 0;
                        active_instance.to_date = DateTime.Now;
                        SalaryRepo.UpdateSalaryStructure(active_instance);

                        Salary_Structure new_sal_structure = new Salary_Structure();
                        new_sal_structure = SalaryCalculation.CalculateSalaryStructure(ctc);
                        new_sal_structure.emp_id = employee_id;
                        new_sal_structure.is_active = 1;
                        new_sal_structure.from_date = DateTime.Now;
                        new_sal_structure.to_date = null;
                        SalaryRepo.CreateSalaryStructure(new_sal_structure);
                        response = Request.CreateResponse(HttpStatusCode.OK, new EMSResponseMessage("EMS_001", "success", "Salary structure successfully updated"));
                    }
                    else
                    {
                        response = Request.CreateResponse(HttpStatusCode.OK, new EMSResponseMessage("EMS_401", "No active Salary Structure", "No active Salary Structure - unable to find active record "));
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
        [Route("api/Get/salarystructure/list/{employee_id?}")]
        [HttpGet]
        public HttpResponseMessage GetSalaryStructure(int employee_id)
        {
            HttpResponseMessage response = null;
            try
            {
                if(employee_id != 0)
                {
                    List<SalaryStructureModel> salary_structure = SalaryRepo.GetSalaryStructureList(employee_id);
                    response = Request.CreateResponse(HttpStatusCode.OK, new EMSResponseMessage("EMS_001", "success", salary_structure));
                }
                else
                {
                    response = Request.CreateResponse(HttpStatusCode.OK, new EMSResponseMessage("EMS_102", "Invalid Input", "please check Input json"));
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
