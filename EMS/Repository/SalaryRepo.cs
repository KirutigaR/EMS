using System;
using System.Collections.Generic;
using System.Data.Entity;
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

        public static void UpdateSalaryStructure(Salary_Structure salary)
        {
            EMSEntities datacontext = new EMSEntities();
            try
            {
                datacontext.Entry(salary).State = EntityState.Modified;
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

        public static Salary_Structure GetSalaryStructureByEmpId(int e_id)//e_id employee_id
        {
            EMSEntities datacontext = new EMSEntities();
            try
            {
                var query = from sal_structure in datacontext.Salary_Structure
                            where sal_structure.emp_id == e_id && sal_structure.is_active == 1
                            select sal_structure;
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