using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Web;

namespace EMS.Repository
{
    public class LeaveRepo
    {
        public static List<Holiday_List> GetHoliday()
        {
            EMSEntities datacontext = new EMSEntities();
            try
            {
                var query = from h in datacontext.Holiday_List
                            select h;
                return query.ToList();
            }
            catch (Exception e)
            {
                Debug.WriteLine(e.Message);
                Debug.WriteLine(e.GetBaseException());
                return null;
            }
            finally
            {
                datacontext.Dispose();
            }
        }
        public static decimal GetYearleave(string Leavetype)
        {
            EMSEntities datacontext = new EMSEntities();
            try
            {
                var query = from lt in datacontext.Leave_type
                            where lt.type_name == Leavetype
                            select lt.days_per_year;
                return query.FirstOrDefault();
            }
            catch (Exception e)
            {
                Debug.WriteLine(e.Message);
                Debug.WriteLine(e.GetBaseException());
                return 0;
            }
            finally
            {
                datacontext.Dispose();
            }
        }
    }
}