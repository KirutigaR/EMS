using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Diagnostics;
using System.Linq;
using System.Web;
using EMS.Models;

namespace EMS.Repository
{
	public class ClientRepo
	{
        public static void CreateNewClient(Client client)
        {
            EMSEntities datacontext = new EMSEntities();
            try
            {
                datacontext.Clients.Add(client);
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

        public static Client GetClientById(int c_id)
        {
            EMSEntities datacontext = new EMSEntities();
            try
            {
                var query = from x in datacontext.Clients
                            where x.id == c_id
                            select x;
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

        public static ClientModel GetClientDetailsById(int c_id)
        {
            EMSEntities datacontext = new EMSEntities();
            try
            {
                var query = from x in datacontext.Clients
                            join y in datacontext.Client_type
                            on x.type_id equals y.id
                            where x.id == c_id
                            select new ClientModel
                            {
                                client_id = x.id,
                                client_name = x.client_name,
                                client_type_id = x.type_id,
                                client_type_name = y.type_name,
                                client_type_description = y.type_description,
                                is_active = x.is_active
                            };
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

        public static List<ClientModel> GetClientList()
        {
            EMSEntities datacontext = new EMSEntities();
            try
            {
                var query = from x in datacontext.Clients
                            join y in datacontext.Client_type
                            on x.type_id equals y.id
                            select new ClientModel
                            {
                                client_id = x.id,
                                client_name = x.client_name,
                                client_type_id = x.type_id,
                                client_type_name = y.type_name,
                                client_type_description = y.type_description,
                                is_active = x.is_active
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

        public static List<ClientModel> GetActiveClientList()
        {
            EMSEntities datacontext = new EMSEntities();
            try
            {
                var query = from x in datacontext.Clients
                            join y in datacontext.Client_type
                            on x.type_id equals y.id
                            where x.is_active == 1
                            select new ClientModel
                            {
                                client_id = x.id,
                                client_name = x.client_name,
                                client_type_id = x.type_id,
                                client_type_name = y.type_name,
                                client_type_description = y.type_description,
                                is_active = x.is_active
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

        public static List<ClientModel> GetClientTypeList()
        {
            EMSEntities datacontext = new EMSEntities();
            try
            {
                var query = from c_type in datacontext.Client_type
                            select new ClientModel
                            {
                                client_type_id = c_type.id,
                                client_type_name = c_type.type_name,
                                client_type_description = c_type.type_description
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

        public static void EditClient(Client client)
        {
            EMSEntities datacontext = new EMSEntities();
            try
            {
                datacontext.Entry(client).State = EntityState.Modified;
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

        public static void DropClient(int c_id)
        {
            EMSEntities datacontext = new EMSEntities();
            try
            {
                var query = from client in datacontext.Clients
                            where client.id == c_id
                            select client;
                query.FirstOrDefault().is_active = 0;
                datacontext.Entry(query.FirstOrDefault()).State = EntityState.Modified;
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

        public static List<ProjectModel> GetProjectListByClientId(int c_id)
        {
            EMSEntities datacontext = new EMSEntities();
            try
            {
                var query = from x in datacontext.Clients
                            join y in datacontext.Projects
                            on x.id equals y.client_id
                            where x.id == c_id
                            select new ProjectModel
                            {
                                project_id = y.id,
                                project_name = y.project_name,
                                project_description = y.project_description,
                                start_date = y.start_date,
                                end_date = y.end_date,
                                status = y.status,
                                po = y.po,
                                client_id = x.id,
                                client_name = x.client_name,
                                type_id = x.type_id,
                                resources_req = y.resources_req
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