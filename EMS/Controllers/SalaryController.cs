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
    public class SalaryController : ApiController
    {
        public HttpResponseMessage UpdateSalaryStructure(int e_id , decimal ctc)
        {
            HttpResponseMessage response = null;
            try
            {
                if(e_id !=0 && EmployeeRepo.GetEmployeeStatusById(e_id)!=0 && ctc!=0)
                {
                    Salary_Structure active_instance = SalaryRepo.GetSalaryStructureByEmpId(e_id);
                    if (active_instance != null)
                    {
                        active_instance.is_active = 0;
                        active_instance.to_date = DateTime.Now;
                        SalaryRepo.UpdateSalaryStructure(active_instance);

                        Salary_Structure new_sal_structure = new Salary_Structure();
                        new_sal_structure = SalaryCalculation.CalculateSalaryStructure(ctc);
                        new_sal_structure.emp_id = e_id;
                        new_sal_structure.is_active = 1;
                        new_sal_structure.from_date = DateTime.Now;
                        new_sal_structure.to_date = null;
                        SalaryRepo.CreateSalaryStructure(new_sal_structure);
                    }
                    else
                    {
                        response = Request.CreateResponse(HttpStatusCode.OK, new EMSResponseMessage("EMS_107", "No active Salary Structure", "No active Salary Structure - unable to find active record "));
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
        [Route("api/Get/salarystructure/{e_id?}")]
        [HttpGet]
        public HttpResponseMessage GetSalaryStructure(int e_id)
        {
            HttpResponseMessage response = null;
            try
            {
                if(e_id != 0)
                {
                    Salary_Structure salary_structure = SalaryRepo.GetSalaryStructureByEmpId(e_id);
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
