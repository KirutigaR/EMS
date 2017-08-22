using System;
using EMS.Models;
using EMS.Repository;

namespace EMS.Utility
{
    public class SalaryCalculation
    {
        public static Salary_Structure CalculateSalaryStructure(decimal ctc)
        {
            Salary_Structure salarypobj = new Salary_Structure();
            salarypobj.ctc = ctc;
            salarypobj.basic_pay = ((salarypobj.ctc*40)/(100));//basic pay = 40% of ctc 
            salarypobj.HRA = salarypobj.basic_pay / 2;
            salarypobj.MA = Constants.MEDICAL_ALLOWANCE * 12;
            salarypobj.CA = Constants.CONVEYANCE_ALLOWANCE * 12;
            salarypobj.FA = Constants.FOOD_ALLOWANCE * 12;
            salarypobj.PT = Constants.PT * 12;
            salarypobj.PF = ((salarypobj.basic_pay*12)/ 100);
            salarypobj.SA = salarypobj.ctc - (salarypobj.basic_pay + salarypobj.HRA + salarypobj.MA + salarypobj.CA + salarypobj.FA + salarypobj.PF + salarypobj.Gratuity);
            salarypobj.MI = 0;
            salarypobj.Gratuity = 0;
            if (salarypobj.ctc<252000)   
            {
                decimal temp = (salarypobj.basic_pay + salarypobj.HRA + salarypobj.MA + salarypobj.CA + salarypobj.FA + salarypobj.SA + salarypobj.PF);
                salarypobj.ESI = (temp * 475)/10000;
            }
            else
            {
                salarypobj.ESI = 0;
            }
            return salarypobj;
        }

        public static Payslip Calculatepayslip(Salary_Structure salary)
        {
            Payslip payslip = new Payslip();
            payslip.emp_id = salary.emp_id;
            payslip.payslip_month = DateTime.Now.Month;
            payslip.basic_pay = salary.basic_pay / 12;
            payslip.HRA = salary.HRA / 12;
            payslip.FA = salary.FA / 12;
            payslip.MA = salary.MA / 12;
            payslip.CA = salary.CA / 12;
            payslip.PF = salary.PF / 12;
            payslip.MI = salary.MI / 12;
            payslip.ESI = salary.ESI / 12;
            payslip.Gratuity = salary.Gratuity / 12;
            payslip.SA = salary.SA / 12;
            payslip.PT = salary.PT / 12;
            Incometax incometax = IncometaxRepo.GetTaxValueByEmpId(salary.emp_id);
            payslip.incometax = incometax.income_tax;
            payslip.arrears = 0;
            return payslip;
        }
    }
}