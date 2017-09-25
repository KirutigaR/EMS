using System;
using EMS.Models;
using EMS.Repository;
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
            salarypobj.PT = Constants.PT * 12;
            salarypobj.PF = ((salarypobj.basic_pay*12)/ 100);

            #region FA calculation
            if (salarypobj.ctc < 252000)
                salarypobj.FA = 0;  // CTC Less than 252000 NO need of Food Allowance
            else
                salarypobj.FA = Constants.FOOD_ALLOWANCE * 12;
            #endregion

            salarypobj.SA = salarypobj.ctc - (salarypobj.basic_pay + salarypobj.HRA + salarypobj.MA + salarypobj.CA + salarypobj.FA + salarypobj.PF + salarypobj.Gratuity);
            if (salarypobj.SA < 0)
                salarypobj.SA = 0;
            salarypobj.MI = 0;
            salarypobj.Gratuity = 0;

            #region ESI calculation
            //ESI Calculation starts
            if (salarypobj.ctc<252000)   
            {
                decimal temp = (salarypobj.basic_pay + salarypobj.HRA + salarypobj.MA + salarypobj.CA + salarypobj.FA + salarypobj.SA + salarypobj.PF);
                salarypobj.ESI = (temp * 475)/10000;
            }
            else
            {
                salarypobj.ESI = 0;
            }
            #endregion

            return salarypobj;
        }

        public static Payslip Calculatepayslip(Salary_Structure salary)
        {
            Payslip payslip = new Payslip();
            payslip.emp_id = salary.emp_id;
            payslip.payslip_month = DateTime.Now.Month;
            payslip.payslip_year = DateTime.Now.Year;
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
            if(incometax != null)
            {
                payslip.incometax = incometax.income_tax;
            }
            else
            {
                payslip.incometax = 0;
            }
            payslip.arrears = 0;
            return payslip;
        }
        public static Payslip FirstMonthSalary(DateTime DOJ, Salary_Structure salary)
        {
            if(salary.is_active==1)
            {
                int working_days = (DateTime.DaysInMonth(DateTime.Now.Year, DateTime.Now.Month) - DOJ.Day) + 1;
                Payslip payslip = new Payslip();
                decimal per_day_salary = (salary.basic_pay / 12) / 30;
                Debug.WriteLine(working_days);
                payslip.emp_id = salary.emp_id;
                payslip.basic_pay = per_day_salary * working_days;
                payslip.HRA = ((salary.HRA / 12) / 30) * working_days;
                payslip.FA = (Constants.FOOD_ALLOWANCE / 30) * working_days;
                payslip.MA = (Constants.MEDICAL_ALLOWANCE / 30) * working_days;
                payslip.CA = (Constants.CONVEYANCE_ALLOWANCE / 30) * working_days;
                payslip.PF = ((salary.PF / 12) / 30) * working_days;
                payslip.MI = ((salary.MI / 12) / 30) * working_days;
                payslip.Gratuity = ((salary.Gratuity / 12) / 30) * working_days;
                payslip.SA = ((salary.SA / 12) / 30) * working_days;
                payslip.ESI = ((salary.ESI / 12) / 30) * working_days;
                payslip.PT = (Constants.PT / 30) * working_days;
                payslip.payslip_month = DOJ.Month;
                payslip.payslip_year = DOJ.Year;
                return payslip;
            }
            else
            {
                return null;
            }
        }
    }
}