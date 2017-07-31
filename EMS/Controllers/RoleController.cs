using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Web;
using System.Web.Http;
using EMS.Repository;


namespace EMS.Controllers
{
    public class RoleController
    {
        [Route("api/get/rolelist")]
        public HttpResponseMessage GetRoleList()
        {
            HttpResponseMessage response = null;
            try
            {
             
            }
            catch (Exception exception)
            {

            }
            return response;
        }
    }
}