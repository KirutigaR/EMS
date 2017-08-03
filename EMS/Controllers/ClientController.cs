using System;
using System.Collections.Generic;
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
                    Client existingInstance = ClientRepo.GetClientById(client.id);
                    if (existingInstance == null)
                    {
                        ClientRepo.CreateNewClient(client);
                        Response = Request.CreateResponse(HttpStatusCode.OK, new EMSResponseMessage("EMS_001", "Success", "Client added successfully"));
                    }
                    else
                    {
                        Response = Request.CreateResponse(HttpStatusCode.OK, new EMSResponseMessage("EMS_104", "Client ID already exists", "Client ID already exists"));
                    }
                }
                else
                {
                    Response = Request.CreateResponse(HttpStatusCode.OK, new EMSResponseMessage("EMS_102", "Failure", "Please check the Json input"));
                }
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

        [Route("api/get/client/{c_id?}")]
        public HttpResponseMessage GetClientById(int c_id)
        {
            HttpResponseMessage response = null;
            try
            {
                if (c_id != 0)
                {
                    ClientModel existinginstance = ClientRepo.GetClientDetailsById(c_id);
                    if (existinginstance != null)
                    {
                        response = Request.CreateResponse(HttpStatusCode.OK, new EMSResponseMessage("EMS_001", "Success", existinginstance));
                    }
                    else
                    {
                        response = Request.CreateResponse(HttpStatusCode.OK, new EMSResponseMessage("EMS_103", "Client ID doesnot exists", "Client ID doesnot exists"));
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
                        response = Request.CreateResponse(HttpStatusCode.OK, new EMSResponseMessage("EMS_103", "Invalid Client ID", "Invalid Client ID"));
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
        [Route("api/client/drop/{c_id}")]
        public HttpResponseMessage DropClient(int c_id)
        {
            HttpResponseMessage response = null;
            try
            {
                if (c_id != 0)
                {
                    Client existinginstace = ClientRepo.GetClientById(c_id);
                    if (existinginstace != null)
                    {
                        ClientRepo.DropClient(c_id);
                        response = Request.CreateResponse(HttpStatusCode.OK, new EMSResponseMessage("EMS_001", "Success", "Client details dropped!"));
                    }
                    else
                    {
                        response = Request.CreateResponse(HttpStatusCode.OK, new EMSResponseMessage("EMS_103", "Invalid Client ID", "Invalid Client ID"));
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

        [Route("api/client/project/list/{c_id?}")]
        public HttpResponseMessage GetProjectListByClientId(int c_id)
        {
            HttpResponseMessage response = null;
            try
            {
                if(c_id!=0)
                {
                    if(ClientRepo.GetClientById(c_id)!=null)
                    {
                        List<ProjectModel> project_list = ClientRepo.GetProjectListByClientId(c_id);
                        response = Request.CreateResponse(HttpStatusCode.OK, new EMSResponseMessage("EMS_001", "Success", project_list));
                    }
                    else
                    {
                        response = Request.CreateResponse(HttpStatusCode.OK, new EMSResponseMessage("EMS_103", "Invalid Client ID", "Invalid Client ID"));
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
