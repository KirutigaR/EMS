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
    public class PayslipController : ApiController
    {
        [Route("api/payslip/generate/{e_id?}")]
        public HttpResponseMessage GeneratePayslip(int e_id)//e_id employee_id
        {
            HttpResponseMessage response = null;
            try
            {
                if(e_id != 0 )
                {
                    Salary_Structure salary = SalaryRepo.GetSalaryStructureByEmpId(e_id);
                    Payslip payslip = SalaryCalculation.Calculatepayslip(salary);
                    Payslip existing_instance = PayslipRepo.GetExistingPayslip(payslip.emp_id, payslip.payslip_month);
                    if(payslip!= null && existing_instance == null)
                    {
                        PayslipRepo.AddPayslip(payslip);
                        response = Request.CreateResponse(HttpStatusCode.OK, new EMSResponseMessage("EMS_001", "Success", "Pay slip generated successfully!"));
                    }
                    else
                    {
                        response = Request.CreateResponse(HttpStatusCode.OK, new EMSResponseMessage("EMS_109", "Failure", "Pay slip generation Error or payslip for specified month is already exists"));
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
