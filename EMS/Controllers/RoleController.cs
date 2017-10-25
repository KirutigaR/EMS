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

    public class RoleController : ApiController
    {
        [Route("api/get/rolelist")]
        public HttpResponseMessage GetRoleList()
        {
            HttpResponseMessage response = null;
            try
            {
                List<RoleModel> rolelist = RoleRepo.GetRoleList();
                response = Request.CreateResponse(HttpStatusCode.OK, new EMSResponseMessage("EMS_001", "Success", rolelist));
            }
            catch (Exception exception)
            {
                Debug.WriteLine(exception.Message);
                Debug.WriteLine(exception.GetBaseException());
                response = Request.CreateResponse(HttpStatusCode.OK, new EMSResponseMessage("EMS_101", "Application Error", exception.Message));
            }
            return response;
        }

        [Route("api/rolelist/systemrole")]
        public HttpResponseMessage GetSystemRoleList()
        {
            HttpResponseMessage response = null;
            try
            {
                List<RoleModel> rolelist = RoleRepo.GetSystemRoleList();
                response = Request.CreateResponse(HttpStatusCode.OK, new EMSResponseMessage("EMS_001", "Success", rolelist));
            }
            catch (Exception exception)
            {
                Debug.WriteLine(exception.Message);
                Debug.WriteLine(exception.GetBaseException());
                response = Request.CreateResponse(HttpStatusCode.OK, new EMSResponseMessage("EMS_101", "Application Error", exception.Message));
            }
            return response;
        }

        [Route("api/rolelist/projectrole")]
        public HttpResponseMessage GetProjectRoleList()
        {
            HttpResponseMessage response = null;
            try
            {
                List<RoleModel> rolelist = RoleRepo.GetProjectRoleList();
                response = Request.CreateResponse(HttpStatusCode.OK, new EMSResponseMessage("EMS_001", "Success", rolelist));
            }
            catch (Exception exception)
            {
                Debug.WriteLine(exception.Message);
                Debug.WriteLine(exception.GetBaseException());
                response = Request.CreateResponse(HttpStatusCode.OK, new EMSResponseMessage("EMS_101", "Application Error", exception.Message));
            }
            return response;
        }

        [Route("api/role/add")]
        public HttpResponseMessage AddRole(Role role)
        {
            HttpResponseMessage response = null;
            try
            {
                if (role != null)
                {
                    RoleModel existing_instance = RoleRepo.GetExistingRole(role);
                    if (existing_instance != null)
                    {
                        response = Request.CreateResponse(HttpStatusCode.OK, new EMSResponseMessage("EMS_701", "Role already exists", "Role already exists"));
                    }
                    else
                    {
                        RoleRepo.CreateRole(role);
                        response = Request.CreateResponse(HttpStatusCode.OK, new EMSResponseMessage("EMS_001", "Role created Successfully!", "Role created Successfully!"));
                    }
                }
                else
                {
                    response = Request.CreateResponse(HttpStatusCode.OK, new EMSResponseMessage("EMS_102", "Failure", "Please check the Json input"));
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

        [Route("api/get/role/{role_id?}")]
        public HttpResponseMessage GetRoleById(int role_id)//r_id role_id
        {
            HttpResponseMessage response = null;
            try
            {
                if (role_id != 0)
                {
                    RoleModel instance = RoleRepo.GetRoleById(role_id);
                    if (instance != null)
                    {
                        response = Request.CreateResponse(HttpStatusCode.OK, new EMSResponseMessage("EMS_001", "Success", instance));
                    }
                    else
                    {
                        response = Request.CreateResponse(HttpStatusCode.OK, new EMSResponseMessage("EMS_702", "Invalid role ID", "Invalid role ID"));
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

        [Route("api/role/edit")]
        public HttpResponseMessage EditRole(Role role)
        {
            HttpResponseMessage response = null;
            try
            {
                if (role != null)
                {
                    if(RoleRepo.GetRoleById(role.id)!=null)
                    {
                        RoleRepo.EditRole(role);
                        response = Request.CreateResponse(HttpStatusCode.OK, new EMSResponseMessage("EMS_001", "ROle Updated successfully!", "ROle Updated successfully!"));
                    }
                    else
                    {
                        response = Request.CreateResponse(HttpStatusCode.OK, new EMSResponseMessage("EMS_702", "Invalid role object", "Invalid role object"));
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
        [Route("api/role/delete/{role_id?}")]
        public HttpResponseMessage DeleteRoleById(int role_id)//r_id role_id
        {
            HttpResponseMessage response = null;
            try
            {
                if (role_id != 0)
                {
                    if (RoleRepo.GetRoleById(role_id) != null)
                    {
                        RoleRepo.DeleteRoleById(role_id);
                        response = Request.CreateResponse(HttpStatusCode.OK, new EMSResponseMessage("EMS_001", "Role deleted successfully!", "Role deleted successfully!"));
                    }
                    else
                    {
                        response = Request.CreateResponse(HttpStatusCode.OK, new EMSResponseMessage("EMS_702", "Invalid role object", "Invalid role object"));
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
    }
}