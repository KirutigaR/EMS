using System;
using System.Collections.Generic;
using System.Data.Entity.Validation;
using System.Diagnostics;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using EMS.Models;
using EMS.Repository;
using EMS.Utility;

namespace EMS.Controllers
{
    public class ClientController : ApiController
    {
        [HttpPost]
        [Route("api/create/client")]
        public HttpResponseMessage CreateNewClient(Client client)
        {
            HttpResponseMessage Response = null;
            try
            {
                if (client != null)
                {
                    Client existingInstance = ClientRepo.GetExistingClientInstance(client.client_name, client.type_id);
                    if (existingInstance == null)
                    {
                        client.is_active = 1;
                        ClientRepo.CreateNewClient(client);
                        Response = Request.CreateResponse(HttpStatusCode.OK, new EMSResponseMessage("EMS_001", "Success", "Client added successfully"));
                    }
                    else
                    {
                        Response = Request.CreateResponse(HttpStatusCode.OK, new EMSResponseMessage("EMS_201", "Client already exists", "Client already exists"));
                    }
                }
                else
                {
                    Response = Request.CreateResponse(HttpStatusCode.OK, new EMSResponseMessage("EMS_102", "Failure", "Please check the Json input"));
                }
            }
            catch (DbEntityValidationException DBexception)
            {
                Debug.WriteLine(DBexception.Message);
                Debug.WriteLine(DBexception.GetBaseException());
                Response = Request.CreateResponse(HttpStatusCode.OK, new EMSResponseMessage("EMS_190", "Mandatory fields missing", DBexception.Message));
            }
            catch (Exception exception)
            {
                Debug.WriteLine(exception.Message);
                Debug.WriteLine(exception.GetBaseException());
                Response = Request.CreateResponse(HttpStatusCode.OK, new EMSResponseMessage("EMS_101", "Application Error", exception.Message));
            }
            return Response;
        }


        [Route("api/client/list")]
        public HttpResponseMessage GetClientList()
        {
            HttpResponseMessage response = null;
            try
            {
                List<ClientModel> Client_List = ClientRepo.GetClientList();
                response = Request.CreateResponse(HttpStatusCode.OK, new EMSResponseMessage("EMS_001", "Success", Client_List));
            }
            catch (Exception exception)
            {
                Debug.WriteLine(exception.Message);
                Debug.WriteLine(exception.GetBaseException());
                response = Request.CreateResponse(HttpStatusCode.OK, new EMSResponseMessage("EMS_101", "Application Error", exception.Message));
            }
            return response;
        }

        [Route("api/active/client/list")]
        public HttpResponseMessage GetActiveClientList()
        {
            HttpResponseMessage response = null;
            try
            {
                List<ClientModel> Client_List = ClientRepo.GetActiveClientList();
                response = Request.CreateResponse(HttpStatusCode.OK, new EMSResponseMessage("EMS_001", "Success", Client_List));
            }
            catch (Exception exception)
            {
                Debug.WriteLine(exception.Message);
                Debug.WriteLine(exception.GetBaseException());
                response = Request.CreateResponse(HttpStatusCode.OK, new EMSResponseMessage("EMS_101", "Application Error", exception.Message));
            }
            return response;
        }

        [Route("api/get/client/{client_id?}")]
        public HttpResponseMessage GetClientById(int client_id)//c_id client_id
        {
            HttpResponseMessage response = null;
            try
            {
                if (client_id != 0)
                {
                    ClientModel existinginstance = ClientRepo.GetClientDetailsById(client_id);
                    if (existinginstance != null)
                    {
                        response = Request.CreateResponse(HttpStatusCode.OK, new EMSResponseMessage("EMS_001", "Success", existinginstance));
                    }
                    else
                    {
                        response = Request.CreateResponse(HttpStatusCode.OK, new EMSResponseMessage("EMS_202", "Client ID doesnot exists", "Client ID doesnot exists"));
                    }
                }
                else
                {
                    response = Request.CreateResponse(HttpStatusCode.OK, new EMSResponseMessage("EMS_102", "Invalid Input", "Please check input Json"));
                }
            }
            catch (Exception exception)
            {
                Debug.WriteLine(exception.Message);
                Debug.WriteLine(exception.GetBaseException());
                response = Request.CreateResponse(HttpStatusCode.OK, new EMSResponseMessage("EMS_101", "Application Error", exception.Message));
            }
            return response;
        }

        [HttpPost]
        [Route("api/client/edit")]
        public HttpResponseMessage EditClientDetails(Client client)
        {
            HttpResponseMessage response = null;
            try
            {
                if (client != null)
                {
                    Client existingInstance = ClientRepo.GetClientById(client.id);
                    if (existingInstance != null)
                    {
                        ClientRepo.EditClient(client);
                        response = Request.CreateResponse(HttpStatusCode.OK, new EMSResponseMessage("EMS_001", "Success", "Client details Updated successfully!"));
                    }
                    else
                    {
                        response = Request.CreateResponse(HttpStatusCode.OK, new EMSResponseMessage("EMS_202", " Client ID does not exists", "Invalid Client ID"));
                    }
                }
                else
                {
                    response = Request.CreateResponse(HttpStatusCode.OK, new EMSResponseMessage("EMS_102", "Invalid Input", "Please check input Json"));
                }
            }
            catch (Exception exception)
            {
                Debug.WriteLine(exception.Message);
                Debug.WriteLine(exception.GetBaseException());
                response = Request.CreateResponse(HttpStatusCode.OK, new EMSResponseMessage("EMS_101", "Application Error", exception.Message));
            }
            return response;
        }


        [HttpGet]
        [Route("api/client/drop/{client_id}")]
        public HttpResponseMessage DropClient(int client_id)//c_id client_id
        {
            HttpResponseMessage response = null;
            try
            {
                if (client_id != 0)
                {
                    Client existinginstace = ClientRepo.GetClientById(client_id);
                    if (existinginstace != null)
                    {
                        ClientRepo.DropClient(client_id);
                        response = Request.CreateResponse(HttpStatusCode.OK, new EMSResponseMessage("EMS_001", "Success", "Client details dropped!"));
                    }
                    else
                    {
                        response = Request.CreateResponse(HttpStatusCode.OK, new EMSResponseMessage("EMS_202", "Client ID doesnot exists", "Invalid Client ID"));
                    }
                }
                else
                {
                    response = Request.CreateResponse(HttpStatusCode.OK, new EMSResponseMessage("EMS_102", "Invalid Input", "Please check input Json"));
                }
            }
            catch (Exception exception)
            {
                Debug.WriteLine(exception.Message);
                Debug.WriteLine(exception.GetBaseException());
                response = Request.CreateResponse(HttpStatusCode.OK, new EMSResponseMessage("EMS_101", "Application Error", exception.Message));
            }
            return response;
        }

        [Route("api/client/project/list/{client_id?}")]
        public HttpResponseMessage GetProjectListByClientId(int client_id)//c_id client_id
        {
            HttpResponseMessage response = null;
            try
            {
                if(client_id != 0)
                {
                    if(ClientRepo.GetClientById(client_id) !=null)
                    {
                        List<ProjectModel> project_list = ClientRepo.GetProjectListByClientId(client_id);
                        response = Request.CreateResponse(HttpStatusCode.OK, new EMSResponseMessage("EMS_001", "Success", project_list));
                    }
                    else
                    {
                        response = Request.CreateResponse(HttpStatusCode.OK, new EMSResponseMessage("EMS_202", "Client ID doesnot exists", "Invalid Client ID"));
                    }
                }
                else
                {
                    response = Request.CreateResponse(HttpStatusCode.OK, new EMSResponseMessage("EMS_102", "Invalid Input", "Please check input Json"));
                }
            }
            catch (Exception exception)
            {
                Debug.WriteLine(exception.Message);
                Debug.WriteLine(exception.GetBaseException());
                response = Request.CreateResponse(HttpStatusCode.OK, new EMSResponseMessage("EMS_101", "Application Error", exception.Message));
            }
            return response;
        }

        [Route("api/client/type/list")]
        public HttpResponseMessage GetClientTypeList()
        {
            HttpResponseMessage response = null;
            try
            {
                List<ClientModel> Client_type_List = ClientRepo.GetClientTypeList();
                response = Request.CreateResponse(HttpStatusCode.OK, new EMSResponseMessage("EMS_001", "Success", Client_type_List));
            }
            catch (Exception exception)
            {
                Debug.WriteLine(exception.Message);
                Debug.WriteLine(exception.GetBaseException());
                response = Request.CreateResponse(HttpStatusCode.OK, new EMSResponseMessage("EMS_101", "Application Error", exception.Message));
            }
            return response;
        }
    }
}
