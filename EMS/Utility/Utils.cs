using EMS.Repository;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace EMS.Utility
{
    public class Utils
    {
        public static decimal ClLeaveBasedDOJ(DateTime dateofjoining, string Leavetype)
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
        public static decimal ElLeaveBasedDOJ(DateTime dateofjoining, string Leavetype)
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
        public static int DaysLeft(DateTime? startDate, DateTime? endDate, Boolean excludeWeekends, List<DateTime> excludeDates)
        {
            int count = 0;
            for (DateTime index = (DateTime)startDate; index <= endDate; index = index.AddDays(1))
            {
                if (excludeWeekends && index.DayOfWeek != DayOfWeek.Sunday && index.DayOfWeek != DayOfWeek.Saturday)
                {
                    bool excluded = false; ;
                    if (excludeDates != null)
                    {
                        for (int i = 0; i < excludeDates.Count; i++)
                        {
                            if (index.Date.CompareTo(excludeDates[i].Date) == 0)
                            {
                                excluded = true;
                                break;
                            }
                        }
                    }

                    if (!excluded)
                    {
                        count++;
                    }
                }
            }

            return count;
        }
    }
}