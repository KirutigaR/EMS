using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Web;

namespace EMS.Repository
{
    public class SalaryRepo
    {
        public static void CreateSalaryStructure(Salary_Structure salary)
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