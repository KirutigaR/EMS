using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Diagnostics;
using System.Linq;
using System.Web;

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

        public static Incometax GetIncometaxById(int i_id)//i_id income_tax_id
        {
            EMSEntities datacontext = new EMSEntities();
            try
            {
                var query = from tax in datacontext.Incometaxes
                            where tax.id == i_id
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

        public static Incometax GetTaxValueByEmpId(int e_id)//e_id employee_id
        {
            EMSEntities datacontext = new EMSEntities();
            try
            {
                var query = from tax in datacontext.Incometaxes
                            where tax.is_active == 1 && tax.from_date <= DateTime.Now && tax.to_date == null
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

        public static List<Incometax> GetIncometaxListByEmpId(int e_id)//e_id employee_id
        {
            EMSEntities datacontext = new EMSEntities();
            try
            {
                var query = from tax in datacontext.Incometaxes
                            where tax.emp_id == e_id
                            select tax;
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