using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using EMS.Utility;
using EMS.Repository;

namespace EMS.Controllers
{
    public class PayslipController : ApiController
    {
        public HttpResponseMessage GeneratePayslip(int e_id)//e_id employee_id
        {
            HttpResponseMessage response = null;
            try
            {
                if(e_id != 0 )
                {

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
                    Payslip pay_slip = PayslipRepo.GetPayslipinstanceById(payslip.emp_id, payslip.payslip_month);
                    //pay_slip.emp_id = payslip.emp_id;
                    pay_slip.incometax = payslip.incometax;
                    pay_slip.arrears = payslip.arrears;
                    PayslipRepo.AddPayslip(pay_slip);
                    response = Request.CreateResponse(HttpStatusCode.OK, new EMSResponseMessage("EMS_001", "success", "payslip successfully updated"));
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
    }
}
