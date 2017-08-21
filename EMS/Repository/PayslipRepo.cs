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
 
        public static void AddPayslip(Salary_Structure salary)
        {
            EMSEntities datacontext = new EMSEntities();
            try
            {
                datacontext.Salary_Structure.Add(salary);
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
    }
}