using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using EMS.Models;
using EMS.Repository;
using EMS.Utility;

namespace EMS.Controllers
{
    public class PayslipController : ApiController
    {
        [Route("api/payslip/generate/{employee_id?}")]
        public HttpResponseMessage GeneratePayslip(int employee_id)//e_id employee_id
        {
            HttpResponseMessage response = null;
            try
            {
                if(employee_id != 0 )
                {
                    Salary_Structure salary = SalaryRepo.GetSalaryStructureByEmpId(employee_id);
                    Payslip payslip = SalaryCalculation.Calculatepayslip(salary);
                    Payslip existing_instance = PayslipRepo.GetExistingPayslip(payslip.emp_id, payslip.payslip_month, payslip.payslip_year);
                    if(payslip!= null && existing_instance == null)
                    {
                        PayslipRepo.AddPayslip(payslip);
                        response = Request.CreateResponse(HttpStatusCode.OK, new EMSResponseMessage("EMS_001", "Pay slip generated successfully!", "Pay slip generated successfully!"));
                    }
                    else
                    {
                        response = Request.CreateResponse(HttpStatusCode.OK, new EMSResponseMessage("EMS_603", "Failure : Pay slip generation Error or payslip for specified month is already exists", "Pay slip generation Error or payslip for specified month is already exists"));
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
        [HttpPost]
        [Route("api/update/payslip")]
        public HttpResponseMessage UpdatePayslip(Payslip payslip)
        {
            HttpResponseMessage response = null;
            try
            {
                if(payslip != null)
                {
                    Payslip pay_slip = PayslipRepo.GetExistingPayslip(payslip.emp_id, payslip.payslip_month, payslip.payslip_year);
                    //pay_slip.emp_id = payslip.emp_id;
                    pay_slip.incometax = payslip.incometax;
                    pay_slip.arrears = payslip.arrears;
                    PayslipRepo.UpdatePayslip(pay_slip);
                    response = Request.CreateResponse(HttpStatusCode.OK, new EMSResponseMessage("EMS_001", "payslip successfully updated", "payslip successfully updated"));
                }
                else
                {
                    response = Request.CreateResponse(HttpStatusCode.OK, new EMSResponseMessage("EMS_102", "Invalid Input", "please check input json"));
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

        [Route("api/payslip/list/{employeeid?}/{year?}/{month?}")]
        public HttpResponseMessage GetPayslipListByEmpId(int employeeid = 0, int year = 0, int month = 0)
        {
            HttpResponseMessage response = null;
            try
            {
                List<PayslipModel> payslip_list = PayslipRepo.GetPayslipList(employeeid, year, month);
                response = Request.CreateResponse(HttpStatusCode.OK, new EMSResponseMessage("EMS_001", "success", payslip_list));
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
