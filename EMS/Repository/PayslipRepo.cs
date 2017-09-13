using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Diagnostics;
using System.Linq;
using System.Web;
using EMS.Models;
using LinqKit;

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

        public static void UpdatePayslip(Payslip payslip)
        {
            EMSEntities datacontext = new EMSEntities();
            try
            {
                datacontext.Entry(payslip).State = EntityState.Modified;
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

        public static Payslip GetExistingPayslip(int employee_id, int month, int year)
        {
            EMSEntities datacontext = new EMSEntities();
            try
            {
                var query = from ps in datacontext.Payslips
                            where ps.emp_id == employee_id && ps.payslip_month == month && ps.payslip_year == year
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

        public static List<PayslipModel> GetPayslipList(int employeeid, int year , int month)
        {
            EMSEntities datacontext = new EMSEntities();
            try
            {
                var predicate = LinqKit.PredicateBuilder.True<Payslip>();
                if(employeeid!=0)
                {
                    predicate = predicate.And(i => i.emp_id == employeeid);
                }
                if(year!=0)
                {
                    predicate = predicate.And(i => i.payslip_year == year);
                }
                if(month!=0)
                {
                    predicate = predicate.And(i => i.payslip_month == month);
                }
                var query = from ps in datacontext.Payslips.AsExpandable().Where(predicate) orderby year, month ascending
                            select new PayslipModel
                            {
                                id = ps.id,
                                emp_id = ps.emp_id,
                                payslip_month = ps.payslip_month,
                                payslip_year = ps.payslip_year,
                                basic_pay = ps.basic_pay,
                                HRA = ps.HRA,
                                MA = ps.MA,
                                FA = ps.FA,
                                CA =ps.CA,
                                PF = ps.PF,
                                MI= ps.MI,
                                ESI= ps.ESI,
                                Gratuity = ps.Gratuity,
                                SA =ps.SA,
                                PT = ps.PT,
                                incometax = ps.incometax,
                                arrears = ps.arrears
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