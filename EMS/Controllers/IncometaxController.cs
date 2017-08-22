﻿using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using EMS.Repository;
using EMS.Utility;

namespace EMS.Controllers
{
    public class IncometaxController : ApiController
    {
        [Route("api/incometax/add")]
        public HttpResponseMessage AddNewTaxDeclaration(Incometax incometax)
        {
            HttpResponseMessage response = null;
            try
            {
                if (incometax != null)
                {
                    IncometaxRepo.AddNewTaxDeclaration(incometax);
                    response = Request.CreateResponse(HttpStatusCode.OK, new EMSResponseMessage("EMS_001", "Success", "Tax declaration added Successfully"));
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

        [Route("api/get/incometax/{e_id?}")]
        public HttpResponseMessage GetTaxValueByEmpId(int e_id)//e_id employee_id
        {
            HttpResponseMessage response = null;
            try
            {
                if (e_id != 0)
                {
                    Incometax tax = IncometaxRepo.GetTaxValueByEmpId(e_id);
                    if (tax != null)
                    {
                        decimal tax_value = tax.income_tax;
                        response = Request.CreateResponse(HttpStatusCode.OK, new EMSResponseMessage("EMS_001", "Success", tax_value));
                    }
                    else
                    {
                        response = Request.CreateResponse(HttpStatusCode.OK, new EMSResponseMessage("EMS_108", "Null Object", "Employee tax value doesnot exists!"));
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

        [Route("api/incometax/update")]
        public HttpResponseMessage UpdateTaxDeclaration(Incometax incometax)
        {
            HttpResponseMessage response = null;
            try
            {
                if (incometax != null && IncometaxRepo.GetIncometaxById(incometax.id) != null)
                {
                    IncometaxRepo.UpdateTaxDeclaration(incometax);
                    response = Request.CreateResponse(HttpStatusCode.OK, new EMSResponseMessage("EMS_001", "Success", "Tax declaration added Successfully"));
                }
                else
                {
                    response = Request.CreateResponse(HttpStatusCode.OK, new EMSResponseMessage("EMS_102", "Invalid Input or ID doesnot exists", "Please check input Json and ID "));
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

        [Route("api/incometax/list/{e_id?}")]
        public HttpResponseMessage GetIncometaxListByEmpId(int e_id)//e_id employww_id
        {
            HttpResponseMessage response = null;
            try
            {
                if(e_id!= 0)
                {
                    List<Incometax> tax_list = IncometaxRepo.GetIncometaxListByEmpId(e_id);
                    response = Request.CreateResponse(HttpStatusCode.OK, new EMSResponseMessage("EMS_001", "Success", tax_list));
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
