using EMS.Repository;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace EMS.Utility
{
    public class Utils
    {
        public static decimal LeaveCalculationBasedDOJ(DateTime dateofjoining, int Leavetype_id)
        {
            if(Leavetype_id == 1)
            {
                decimal DBCL = LeaveRepo.GetYearleave("CL");
                decimal Clleave = DBCL - dateofjoining.Month;
                if (dateofjoining.Day <= 15)
                {
                    decimal CL = DBCL - dateofjoining.Month + 1;
                    return CL;
                }
                return Clleave;
            }
            else if(Leavetype_id == 2)
            {
                decimal DBEL = LeaveRepo.GetYearleave("EL");
                decimal Elleave = 12 - dateofjoining.Month;
                DBEL = DBEL / 12;
                decimal EL = Elleave * (decimal)DBEL;
                if (dateofjoining.Day <= 15)
                {
                    decimal Ell = 12 - dateofjoining.Month + 1;
                    decimal E = Math.Abs(Ell) * (decimal)DBEL;
                    return E;
                }
                return EL;
            }
            else if(Leavetype_id == 3)
            {
                return 0;
            }
            else
            {
                return -999;
            }
        }
      
    }
}