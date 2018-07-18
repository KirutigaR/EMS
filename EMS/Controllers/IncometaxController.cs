﻿using EMS.Filters;
using EMS.Models;
using EMS.Repository;
using EMS.Utility;
using System;
using System.Collections.Generic;
using System.Data.Entity.Validation;
using System.Diagnostics;
using System.Net;
using System.Net.Http;
using System.Web.Http;

namespace EMS.Controllers
{
	[AuthenticationFilter]
	public class IncometaxController : ApiController
    {
        [Route("api/v2/incometax/add")]
        public HttpResponseMessage AddNewTaxDeclaration(Incometax incometax)
        {
            HttpResponseMessage response = null;
            try
            {
                if (incometax != null)
                {
                    incometax.is_active = 1;
                    incometax.to_date = null;
                    IncometaxRepo.AddNewTaxDeclaration(incometax);
                    response = Request.CreateResponse(HttpStatusCode.OK, new EMSResponseMessage("EMS_001", "Tax declaration added Successfully", "Tax declaration added Successfully"));
                }
                else
                {
                    response = Request.CreateResponse(HttpStatusCode.OK, new EMSResponseMessage("EMS_102", "Invalid Input", "Please check input Json"));
                }
            }
            catch (DbEntityValidationException DBexception)
            {
                Debug.WriteLine(DBexception.Message);
                Debug.WriteLine(DBexception.GetBaseException());
                response = Request.CreateResponse(HttpStatusCode.OK, new EMSResponseMessage("EMS_190", "Mandatory fields missing", DBexception.Message));
            }
            catch (Exception exception)
            {
                Debug.WriteLine(exception.Message);
                Debug.WriteLine(exception.GetBaseException());
                response = Request.CreateResponse(HttpStatusCode.OK, new EMSResponseMessage("EMS_101", "Application Error", exception.Message));
            }
            return response;
        }

        [Route("api/v2/get/incometax/{employee_id?}")]
        public HttpResponseMessage GetTaxValueByEmpId(int employee_id)//e_id employee_id
        {
            HttpResponseMessage response = null;
            try
            {
                if (employee_id != 0)
                {
                    Incometax tax = IncometaxRepo.GetTaxValueByEmpId(employee_id);
                    if (tax != null)
                    {
                        decimal tax_value = tax.income_tax;
                        response = Request.CreateResponse(HttpStatusCode.OK, new EMSResponseMessage("EMS_001", "Success", tax_value));
                    }
                    else
                    {
                        response = Request.CreateResponse(HttpStatusCode.OK, new EMSResponseMessage("EMS_601", "Employee tax value doesnot exists!", "Employee tax value doesnot exists!"));
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

        [Route("api/v2/incometax/update")]
        public HttpResponseMessage UpdateTaxDeclaration(Incometax incometax)
        {
            HttpResponseMessage response = null;
            try
            {
                if (incometax != null && IncometaxRepo.GetIncometaxById(incometax.id) != null)
                {
                    IncometaxRepo.UpdateTaxDeclaration(incometax);
                    response = Request.CreateResponse(HttpStatusCode.OK, new EMSResponseMessage("EMS_001", "Tax declaration added Successfully", "Tax declaration added Successfully"));
                }
                else
                {
                    response = Request.CreateResponse(HttpStatusCode.OK, new EMSResponseMessage("EMS_602", "Invalid Input or ID doesnot exists", "Please check input Json and ID "));
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

        [Route("api/v2/incometax/list/{employee_id?}")]
        public HttpResponseMessage GetIncometaxListByEmpId(int employee_id)//e_id employww_id
        {
            HttpResponseMessage response = null;
            try
            {
                if(employee_id!= 0)
                {
                    List<IncometaxModel> tax_list = IncometaxRepo.GetIncometaxListByEmpId(employee_id);
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
