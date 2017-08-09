﻿using System;
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
    }
}