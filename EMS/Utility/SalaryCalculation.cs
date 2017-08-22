using System;
using EMS.Models;
using System.Diagnostics;

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
        public static Payslip FirstMonthSalary(DateTime DOJ, Salary_Structure salary)
        {
            int working_days = DateTime.DaysInMonth(DateTime.Now.Year, DateTime.Now.Month) - DOJ.Day;
            Payslip payslip = new Payslip();
            decimal per_day_salary = (salary.basic_pay / 12) / 30;            
            Debug.WriteLine(working_days);
            payslip.basic_pay = payslip.basic_pay * working_days;
            payslip.HRA = ((salary.HRA / 12) /30 ) * working_days;
            payslip.FA = (Constants.FOOD_ALLOWANCE /30) * working_days;
            payslip.MA = (Constants.MEDICAL_ALLOWANCE /30) * working_days;
            payslip.CA = (Constants.CONVEYANCE_ALLOWANCE /30) * working_days;
            payslip.PF = ((salary.PF / 12) /30) * working_days;
            payslip.MI = ((salary.MI / 12) /30) * working_days ;
            payslip.Gratuity = ((salary.Gratuity / 12)/30) * working_days;
            payslip.SA = ((salary.SA /12) /30) * working_days;
            payslip.ESI = ((salary.ESI / 12) /30) * working_days;
            payslip.payslip_month = DOJ.Month;
            return payslip;
        }
    }
}