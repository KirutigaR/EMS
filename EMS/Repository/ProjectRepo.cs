using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Web;

namespace EMS.Repository
{
	public class ProjectRepo
	{
        public static void CreateNewClient(Client client)
        {
            EMSEntities datacontent = new EMSEntities();
            try
            {
                datacontent.Clients.Add(client);
                datacontent.SaveChanges();
            }
            catch (Exception exception)
            {
                Debug.WriteLine(exception.Message);
                Debug.WriteLine(exception.GetBaseException());
                throw exception;
            }
            finally
            {
                datacontent.Dispose();
            }

        }
        public static void CreateNewProject(Project project)
        {
            EMSEntities datacontent = new EMSEntities();
            try
            {
                datacontent.Projects.Add(project);
                datacontent.SaveChanges();
            }
            catch (Exception exception)
            {
                Debug.WriteLine(exception.Message);
                Debug.WriteLine(exception.GetBaseException());
                throw exception;
            }
            finally
            {
                datacontent.Dispose();
            }
        }
    }
}