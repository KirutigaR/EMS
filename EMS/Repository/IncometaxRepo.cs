using System;
using System.Collections.Generic;
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
    }
}