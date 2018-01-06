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
using Newtonsoft.Json;

namespace EMS.Controllers
{
    public class AssetController : ApiController
    {
        /// <summary>
        /// To Get AssetType List
        /// </summary>
        /// <returns>ToList<string> </returns>
        [HttpGet]
        [Route("api/v1/asset/type/list")]
        public HttpResponseMessage GetAssetTypeList()
        {
            HttpResponseMessage response = null;
            try
            {
                response = Request.CreateResponse(HttpStatusCode.OK, new EMSResponseMessage("EMS_001", "Success", AssetRepo.GetAssetTypeList()));
            }
            catch (Exception exception)
            {
                Debug.WriteLine(exception.Message);
                Debug.WriteLine(exception.GetBaseException());
                response = Request.CreateResponse(HttpStatusCode.OK, new EMSResponseMessage("EMS_101", "Application Error", exception.Message));
            }
            return response;
        }

        /// <summary>
        /// To Get AssetStatus List
        /// </summary>
        /// <returns>ToList<string> </returns>
        [HttpGet]
        [Route("api/v1/asset/status/list")]
        public HttpResponseMessage GetAssetStatusList()
        {
            HttpResponseMessage response = null;
            try
            {
                response = Request.CreateResponse(HttpStatusCode.OK, new EMSResponseMessage("EMS_001", "Success", AssetRepo.GetAssetStatusList()));
            }
            catch (Exception exception)
            {
                Debug.WriteLine(exception.Message);
                Debug.WriteLine(exception.GetBaseException());
                response = Request.CreateResponse(HttpStatusCode.OK, new EMSResponseMessage("EMS_101", "Application Error", exception.Message));
            }
            return response;
        }

        /// <summary>
        /// To Create a Asset
        /// </summary>
        /// <returns>ToList<string> </returns>
        [Route("api/v1/create/asset")]
        [HttpPost]
        public HttpResponseMessage CreateAsset(Asset asset)
        {
            HttpResponseMessage response = null;
            try
            {
                if (asset != null)
                {
                    asset.warranty_expiry_date = asset.purchase_date.AddMonths(asset.warranty_period);
                    AssetRepo.CreateAsset(asset);
                    response = Request.CreateResponse(HttpStatusCode.OK, new EMSResponseMessage("EMS_001", "Asset created successfully", "Asset created successfully"));
                }
                else
                {
                    response = Request.CreateResponse(HttpStatusCode.OK, new EMSResponseMessage("EMS_102", "Invalid Input", "Invalid Input"));
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

        /// <summary>
        /// To Update Asset
        /// </summary>
        /// <returns>ToList<string> </returns>
        [Route("api/v1/update/asset")]
        [HttpPost]
        public HttpResponseMessage UpdateAsset(Asset asset)
        {
            HttpResponseMessage response = null;
            try
            {
                if (asset != null)
                {
                    AssetRepo.UpdateAsset(asset);
                    response = Request.CreateResponse(HttpStatusCode.OK, new EMSResponseMessage("EMS_001", "Success", "Asset Updated successfully"));
                }
                else
                {
                    response = Request.CreateResponse(HttpStatusCode.OK, new EMSResponseMessage("EMS_102", "Invalid Input", "Invalid Input"));
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

        [Route("api/v1/asset/list/available")]
        [HttpGet]
        public HttpResponseMessage GetAvailableAssetList(Asset asset)
        {
            HttpResponseMessage response = null;
            try
            {
                response = Request.CreateResponse(HttpStatusCode.OK, new EMSResponseMessage("EMS_001", "Success", AssetRepo.GetAvailableAssetList()));
            }
            catch (Exception exception)
            {
                Debug.WriteLine(exception.Message);
                Debug.WriteLine(exception.GetBaseException());
                response = Request.CreateResponse(HttpStatusCode.OK, new EMSResponseMessage("EMS_101", "Application Error", exception.Message));
            }
            return response;
        }

        [Route("api/v1/assign/asset")]
        [HttpPost]
        public HttpResponseMessage AssignAsset(AssetModel Asset_Assign_Details)
        {
            HttpResponseMessage response = null;
            try
            {
                if (Asset_Assign_Details.employee_id != 0 && Asset_Assign_Details.asset_id_list.Count != 0 && Asset_Assign_Details.assigned_on != null )
                {
                    if(AssetRepo.AssignAsset(Asset_Assign_Details))
                    {
                        Asset_Assign_Details.status_id = 5;
                        if(AssetRepo.UpdateAssetStatus(Asset_Assign_Details))
                        {
                            response = Request.CreateResponse(HttpStatusCode.OK, new EMSResponseMessage("EMS_001", "Asset(S) Assigned to the employee", "Asset(S) Assigned to the employee"));
                        }
                    }
                    else
                    {
                        response = Request.CreateResponse(HttpStatusCode.OK, new EMSResponseMessage("EMS_103", "Error While Assigning Asset(s)", "Error While Assigning Asset(s)"));
                    }
                }
                else
                {
                    response = Request.CreateResponse(HttpStatusCode.OK, new EMSResponseMessage("EMS_102", "Invalid Input", "Invalid Input"));
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

        [Route("api/v1/update/asset/status")]
        [HttpPost]
        public HttpResponseMessage UpdateAssetStatus(AssetModel Asset_Assign_Details)
        {
            HttpResponseMessage response = null;
            try
            {
                if(AssetRepo.UpdateAssetStatus(Asset_Assign_Details))
                {
                    response = Request.CreateResponse(HttpStatusCode.OK, new EMSResponseMessage("EMS_001", "Asset(S) status updated successfully", "Asset(S) status updated successfully"));
                }
                else
                {
                    response = Request.CreateResponse(HttpStatusCode.OK, new EMSResponseMessage("EMS_103", "Error While updating status", "Error While updating status"));
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
