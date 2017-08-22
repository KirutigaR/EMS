using System;
using EMS.Models;

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
    }
}