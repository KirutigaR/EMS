using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Diagnostics;
using System.Linq;
using System.Web;
using EMS.Models;

namespace EMS.Repository
{
    public class IncometaxRepo
    {
        public static void AddNewTaxDeclaration(Incometax incometax)
        {
            EMSEntities datacontext = new EMSEntities();
            try
            {
                datacontext.Incometaxes.Add(incometax);
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

        public static void UpdateTaxDeclaration(Incometax incometax)
        {
            EMSEntities datacontext = new EMSEntities();
            try
            {
                datacontext.Entry(incometax).State = EntityState.Modified;
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

        public static Incometax GetIncometaxById(int it_id)//i_id income_tax_id
        {
            EMSEntities datacontext = new EMSEntities();
            try
            {
                var query = from tax in datacontext.Incometaxes
                            where tax.id == it_id
                            select tax;
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

        public static Incometax GetTaxValueByEmpId(int employee_id)//e_id employee_id
        {
            EMSEntities datacontext = new EMSEntities();
            try
            {
                var query = from tax in datacontext.Incometaxes
                            where tax.is_active == 1 && tax.from_date <= DateTime.Now && tax.to_date == null && tax.emp_id == employee_id
                            select tax;
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

        public static List<IncometaxModel> GetIncometaxListByEmpId(int employee_id)//e_id employee_id
        {
            EMSEntities datacontext = new EMSEntities();
            try
            {
                var query = from tax in datacontext.Incometaxes
                            where tax.emp_id == employee_id
                            select new IncometaxModel
                            {
                                id = tax.id,
                                emp_id = tax.emp_id,
                                from_date = tax.from_date,
                                to_date = tax.to_date,
                                income_tax = tax.income_tax,
                                is_active = tax.is_active,
                                notes = tax.notes
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