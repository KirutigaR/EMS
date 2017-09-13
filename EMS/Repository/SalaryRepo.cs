using EMS.Models;
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

        public static Salary_Structure GetSalaryStructureByEmpId(int employee_id)//e_id employee_id
        {
            EMSEntities datacontext = new EMSEntities();
            try
            {
                var query = from sal_structure in datacontext.Salary_Structure
                            where sal_structure.emp_id == employee_id && sal_structure.is_active == 1
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
        public static List<SalaryStructureModel> GetSalaryStructureList(int employee_id)//e_id employee_id
        {
            EMSEntities datacontext = new EMSEntities();
            try
            {
                var query = from ss in datacontext.Salary_Structure
                            where ss.emp_id == employee_id
                            select new SalaryStructureModel
                            {
                                id = ss.id,
                                emp_id = ss.emp_id,
                                ctc = ss.ctc,
                                basic_pay = ss.basic_pay,
                                HRA = ss.HRA,
                                FA = ss.FA,
                                MA = ss.MA,
                                CA = ss.CA,
                                PF = ss.PF,
                                MI = ss.MI,
                                ESI = ss.ESI,
                                Gratuity = ss.Gratuity,
                                SA = ss.SA,
                                PT = ss.PT,
                                from_date = ss.from_date,
                                to_date = ss.to_date
                            };
                return query.ToList();
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