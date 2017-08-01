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
                var query = from client in datacontext.Clients
                            where client.id == c_id
                            select new ClientModel
                            {
                                client_name = client.client_name,
                                client_type = client.client_type,
                                is_active = client.is_active
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
                var query = from client in datacontext.Clients
                            select new ClientModel
                            {
                                client_name = client.client_name,
                                client_type = client.client_type,
                                is_active = client.is_active
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
    }
}