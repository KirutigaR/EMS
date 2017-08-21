using System;
using EMS.Models;

namespace EMS.Utility
{
    public class PayslipCalculation
    {
        public static Salary_Structure CalculatePayslipDetails(decimal ctc)
        {
            Salary_Structure payslipobj = new Salary_Structure();
            payslipobj.ctc = ctc;
            payslipobj.basic_pay = ((payslipobj.ctc*40)/(100));//basic pay = 40% of ctc 
            payslipobj.HRA = payslipobj.basic_pay / 2;
            payslipobj.MA = Constants.MEDICAL_ALLOWANCE;
            payslipobj.CA = Constants.CONVEYANCE_ALLOWANCE;
            payslipobj.FA = Constants.FOOD_ALLOWANCE;
            payslipobj.PT = Constants.PT;
            payslipobj.PF = ((payslipobj.basic_pay*12)/ 100);
            payslipobj.SA = payslipobj.ctc - (payslipobj.basic_pay + payslipobj.HRA + payslipobj.MA + payslipobj.CA + payslipobj.FA + payslipobj.PF + payslipobj.Gratuity);
            payslipobj.MI = 0;
            payslipobj.Gratuity = 0;
            if (payslipobj.ctc<252000)   
            {
                decimal temp = (payslipobj.basic_pay + payslipobj.HRA + payslipobj.MA + payslipobj.CA + payslipobj.FA + payslipobj.SA + payslipobj.PF);
                payslipobj.ESI = (temp * 475)/10000;
            }
            else
            {
                payslipobj.ESI = 0;
            }
            return payslipobj;
        }
    }
}