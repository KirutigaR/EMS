using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Diagnostics;
using System.Linq;
using System.Web;
using EMS.Models;

namespace EMS.Repository
{
    public class PayslipRepo
    {
 
        public static void AddPayslip(Payslip payslip)
        {
            EMSEntities datacontext = new EMSEntities();
            try
            {
                datacontext.Payslips.Add(payslip);
                datacontext.SaveChanges();
            }
            catch (Exception exception)
            {
                Debug.WriteLine(exception.Message);
                Debug.WriteLine(exception.GetBaseException());
                throw exception;
            }
            finally
            {
                datacontext.Dispose();
            }
        }

        public static Payslip GetExistingPayslip(int e_id, int month)
        {
            EMSEntities datacontext = new EMSEntities();
            try
            {
                var query = from ps in datacontext.Payslips
                            where ps.emp_id == e_id && ps.payslip_month == month
                            select ps;
                return query.FirstOrDefault();
            }
             catch (Exception exception)
            {
                Debug.WriteLine(exception.Message);
                Debug.WriteLine(exception.GetBaseException());
                throw exception;
            }
            finally
            {
                datacontext.Dispose();
            }
        }
    }
}