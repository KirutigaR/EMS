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
using System.Data.OleDb;
using System.Threading.Tasks;
using System.Web;
using System.Data;
using System.Data.SqlClient;
using System.IO;
using System.Configuration;
using Newtonsoft.Json;

namespace EMS.Controllers
{
/// <summary>
/// Phase2.0 Requirement - Asset tracking system
/// </summary>
    public class AssetController : ApiController
    {
        /// <summary>
        /// To get the currently available asset types 
        /// </summary>
        /// <returns>asset type id and name  </returns>
        [HttpGet]
        [Route("api/v2/asset/type/list")]
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
        /// <returns>asset status id and status name </returns>
        [Route("api/v2/asset/status/list")]
        [HttpGet]
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
        /// To Create an Asset
        /// </summary>
        /// <returns></returns>
        [Route("api/v2/create/asset")]
        [HttpPost]
        public HttpResponseMessage CreateAsset(Asset asset)
        {
            HttpResponseMessage response = null;
            try
            {
                if (asset != null)
                {
                    if(AssetRepo.GetAssetDetailsByID(0, asset.asset_serial_no) != null)
                    {
                        response = Request.CreateResponse(HttpStatusCode.OK, new EMSResponseMessage("EMS_109", "Asset serial number already exists", "Asset serial number already exists"));
                    }
                    else
                    {
                        asset.warranty_expiry_date = asset.purchase_date.AddMonths(asset.warranty_period);
                        asset.status_id = Constants.ASSET_STATUS_AVAILABLE;
                        AssetRepo.CreateAsset(asset);
                        response = Request.CreateResponse(HttpStatusCode.OK, new EMSResponseMessage("EMS_001", "Asset created successfully", "Asset created successfully"));
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

        /// <summary>
        /// To Update Asset details
        /// </summary>
        /// <returns></returns>
        [Route("api/v2/update/asset")]
        [HttpPost]
        public HttpResponseMessage UpdateAsset(Asset asset)
        {
            HttpResponseMessage response = null;
            try
            {
                AssetModel existing_instance = AssetRepo.GetAssetDetailsByID(asset.id,null);
                if (asset != null)
                {
                    if(existing_instance.warranty_period != asset.warranty_period)
                    {
                        asset.warranty_expiry_date = asset.purchase_date.AddMonths(asset.warranty_period);
                    }
                    asset.status_id = existing_instance.status_id;
                    AssetRepo.UpdateAsset(asset);
                    response = Request.CreateResponse(HttpStatusCode.OK, new EMSResponseMessage("EMS_001", "Asset Updated successfully", "Asset Updated successfully"));
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
        /// to get list of assets based on status (status can be :'AVAILABLE', 'ASSIGNED', 'SCRAP')
        /// </summary>
        /// <param name="status">string value which sholud be either 'AVAILABLE' or 'ASSIGNED' or 'SCRAP'</param>
        /// <returns>list of asset object</returns>
        [Route("api/v2/asset/list/{status?}")]
        [HttpGet]
        public HttpResponseMessage GetAssetList(string status)
        {
            HttpResponseMessage response = null;
            try
            {
                int status_id = (status == "AVAILABLE")? 6 : (status == "ASSIGNED") ? 5 : (status == "SCRAP")? 7 : 0;
                List<AssetModel> Asset_List  = AssetRepo.GetAssetList(status_id);
                Asset_List = (status == "SCRAP") ? Asset_List.OrderByDescending(x => x.released_on).ToList() :
                                        (status == "ASSIGNED") ? Asset_List.OrderByDescending(x=>x.assigned_on).ToList() : Asset_List.ToList();
                response = Request.CreateResponse(HttpStatusCode.OK, new EMSResponseMessage("EMS_001", "Success", Asset_List));
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
        /// To Upload Assets details through bulk upload 
        /// </summary>
        /// <returns></returns>
        [Route("api/v2/asset/bulkupload")]
        [HttpPost]
        public async Task<HttpResponseMessage> UploadAssetDetails()
        {
            // Check if the request contains multipart/form-data.
            if (!Request.Content.IsMimeMultipartContent())
            {
                throw new HttpResponseException(HttpStatusCode.UnsupportedMediaType);
            }

            string root = HttpContext.Current.Server.MapPath("~/App_Data");
            var provider = new MultipartFormDataStreamProvider(root);

            DataSet ds = new DataSet();
            DataSet ospDS = new DataSet();
            SqlConnection con = null;
            OleDbDataAdapter oda = null;
            SqlBulkCopy objbulk = null;

            try
            {
                // Read the form data.
                await Request.Content.ReadAsMultipartAsync(provider);

                // This illustrates how to get the file names.
                foreach (MultipartFileData file in provider.FileData)
                {
                    Trace.WriteLine(file.Headers.ContentDisposition.FileName);
                    Trace.WriteLine("Server file path: " + file.LocalFileName);
                }
                MultipartFileData fileData = provider.FileData.First();
                string fileName = fileData.Headers.ContentDisposition.FileName;

                if (fileName.StartsWith("\"") && fileName.EndsWith("\""))
                {
                    fileName = fileName.Trim('"');
                }

                string destinationFileName = fileName + Guid.NewGuid().ToString() + Path.GetExtension(fileName.Replace(" ", "_"));
                //string destinationFileName = fileName;
                // Use the Path.Combine method to safely append the file name to the path.
                // Will not overwrite if the destination file already exists.
                File.Move(fileData.LocalFileName, Path.Combine(HttpContext.Current.Server.MapPath("~/App_Data/Uploads/"), destinationFileName));
                File.Delete(fileData.LocalFileName);

                string fileExtension = Path.GetExtension(destinationFileName);
                string fileLocation = HttpContext.Current.Server.MapPath("~/App_Data/Uploads/") + destinationFileName;

                if (fileExtension == ".xls" || fileExtension == ".xlsx")
                {

                    string excelConnectionString = string.Empty;
                    excelConnectionString = "Provider=Microsoft.ACE.OLEDB.12.0;Data Source=" + fileLocation + ";Extended Properties=\"Excel 12.0;HDR=Yes;IMEX=2\"";
                    //connection String for xls file format.
                    if (fileExtension == ".xls")
                    {
                        excelConnectionString = "Provider=Microsoft.Jet.OLEDB.4.0;Data Source=" + fileLocation + ";Extended Properties=\"Excel 8.0;HDR=Yes;IMEX=2\"";
                    }
                    //connection String for xlsx file format.
                    else if (fileExtension == ".xlsx")
                    {
                        excelConnectionString = "Provider=Microsoft.ACE.OLEDB.12.0;Data Source=" + fileLocation + ";Extended Properties=\"Excel 12.0;HDR=Yes;IMEX=2\"";
                    }
                    //Create Connection to Excel work book and add oledb namespace
                    OleDbConnection excelConnection = new OleDbConnection(excelConnectionString);
                    excelConnection.Open();
                    string[] sheets = Utility.GetSheet.GetSheetName(excelConnection);
                    bool flag = false;
                    if (sheets.Length == 0)
                    {
                        return Request.CreateResponse(HttpStatusCode.OK, new EMSResponseMessage("EMS_304", "No Sheets available in the excel file", "No Sheets available in the excel file"));
                    }
                    else
                    {
                        string query = string.Format("Select * FROM [{0}]", sheets.FirstOrDefault());
                        oda = new OleDbDataAdapter(query, excelConnection);
                        oda.Fill(ds);
                        DataTable Exceldt = ds.Tables[0];

                        string sqlconn = ConfigurationManager.ConnectionStrings["dbcon"].ConnectionString;
                        con = new SqlConnection(sqlconn);
                        //creating object of SqlBulkCopy    
                        objbulk = new SqlBulkCopy(con);
                        //assigning Destination table name    

                        objbulk.DestinationTableName = "Asset_Temp_Table";
                        //Mapping Table column    
                        objbulk.ColumnMappings.Add("Type_name", "Type_name");
                        objbulk.ColumnMappings.Add("Model", "Model");
                        objbulk.ColumnMappings.Add("Make", "Make");
                        objbulk.ColumnMappings.Add("Purchase_date", "Purchase_date");
                        objbulk.ColumnMappings.Add("Invoice_no", "Invoice_no");
                        objbulk.ColumnMappings.Add("Vendor_name", "Vendor_name");
                        objbulk.ColumnMappings.Add("Asset_serial_no", "Asset_serial_no");
                        objbulk.ColumnMappings.Add("Warranty_period", "Warranty_period");
                        objbulk.ColumnMappings.Add("Notes", "Notes");
                        objbulk.ColumnMappings.Add("Scrap_date", "Scrap_date");
                        objbulk.ColumnMappings.Add("Price", "Price");
                        objbulk.ColumnMappings.Add("Warranty_expiry_date", "Warranty_expiry_date");

                        //inserting Datatable Records to DataBase    
                        con.Open();
                        objbulk.WriteToServer(Exceldt);
                        con.Close();
                        flag = true;
                    }

                    if (flag && AssetRepo.LoadAssetDataFromTable())
                    {
                        return Request.CreateResponse(HttpStatusCode.OK, new EMSResponseMessage("EMS_306", "Asset details uploaded successfully", "Asset details uploaded successfully"));
                    }
                    else
                    {
                        return Request.CreateResponse(HttpStatusCode.OK, new EMSResponseMessage("EMS_305", "Some problem in Stored Procedure", "Some problem in Stored Procedure"));
                    }
                }
                else
                {
                    return Request.CreateResponse(HttpStatusCode.OK, new EMSResponseMessage("EMS_307", "Invalid File", "Invalid File"));
                }
            }
            catch (OleDbException e)
            {
                Debug.WriteLine(e.Message);
                Debug.WriteLine(e.GetBaseException());
                return Request.CreateResponse(HttpStatusCode.OK, new EMSResponseMessage("EMS_308", "Invalid Excel Sheet", e.Message));
            }
            catch (InvalidOperationException e)
            {
                Debug.WriteLine(e.Message);
                Debug.WriteLine(e.GetBaseException());
                return Request.CreateResponse(HttpStatusCode.OK, new EMSResponseMessage("EMS_308", "Mandatory columns are missing", e.Message));
            }
            catch (IOException IOException)
            {
                Debug.WriteLine(IOException.Message);
                Debug.WriteLine(IOException.GetBaseException());
                return Request.CreateResponse(HttpStatusCode.OK, new EMSResponseMessage("EMS_308", "Please select a file to upload", IOException.Message));
            }
            catch (Exception e)
            {
                Debug.WriteLine(e.Message);
                Debug.WriteLine(e.GetBaseException());
                return Request.CreateResponse(HttpStatusCode.OK, new EMSResponseMessage("EMS_308", "Application Error", e.Message));
            }
        }

        /// <summary>
        /// to update asset status (common api to assign a list of asset to an employee , to move a list of asset either to scrap or to make it available)
        /// </summary>
        /// <param name="Asset_Assign_Details"></param>
        /// <returns></returns>
        [Route("api/v2/update/asset/status")]
        [HttpPost]
        public HttpResponseMessage UpdateAssetStatus(AssetModel Asset_Assign_Details)
        {
            HttpResponseMessage response = null;
            try
            {
                if (Asset_Assign_Details.asset_id_list.Count != 0)
                {
                    //user should get a mail while an asset is released from him 
                    int current_status = AssetRepo.GetAssetDetailsByID(Asset_Assign_Details.asset_id_list[0],null).status_id;
                    if (AssetRepo.UpdateAssetStatus(Asset_Assign_Details))
                    {
                        //get list of assets and its details to send it in mail 
                        List<AssetModel> Asset_details_List = AssetRepo.GetAsserDetailstListByAssetID(Asset_Assign_Details.asset_id_list, new List<int>());
                        //to set response msg differently while assigning asset(s) to an employee and also for mailing functionality
                        if (Asset_Assign_Details.status_name == "ASSIGNED")
                        {
                            //get employee mail id and user name 
                            Asset_details_List = Asset_details_List.Distinct().ToList();
                            MailHandler.AssetMailing(Asset_details_List.FirstOrDefault().employee_name, Asset_details_List.FirstOrDefault().employee_mailid, "ASSIGNED", Asset_details_List, Asset_Assign_Details.assigned_on.Date);
                            response = Request.CreateResponse(HttpStatusCode.OK, new EMSResponseMessage("EMS_001", "Asset(S) Assigned to the employee", "Asset(S) Assigned to the employee"));
                        }
                        //response msg while updating asset status other than ASSIGNED 
                        else if(Asset_Assign_Details.status_name == "AVAILABLE" || Asset_Assign_Details.status_name == "SCRAP")
                        {
                            response = Request.CreateResponse(HttpStatusCode.OK, new EMSResponseMessage("EMS_001", "Asset(S) status updated successfully", "Asset(S) status updated successfully"));
                        }
                        //condition used to send mail while releasing an asset ie. moving from ASSIGNED TO available or From ASSIGNED to scrap
                        if (current_status == Constants.ASSET_STATUS_ASSIGNED)
                        {
                            //get list of employee mail id's to send mail
                            //get the list of assets and its details which has been released from the user 
                            List<int> Released_from_emp_id = Asset_details_List.Select(x => x.employee_id).Distinct().ToList();
                            foreach(int employee in Released_from_emp_id)
                            {
                                List<AssetModel> Asset_released_list = Asset_details_List.Where(i => i.employee_id == employee).ToList();
                                MailHandler.AssetMailing(Asset_released_list.FirstOrDefault().employee_name , Asset_released_list.FirstOrDefault().employee_mailid, "RELEASED", Asset_released_list, DateTime.Now.Date);
                            }
                        }
                    }
                    else//if(!(AssetRepo.UpdateAssetStatus(Asset_Assign_Details)))
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
                //Debug.WriteLine(exception.InnerException);
                //Debug.WriteLine(exception.InnerException.GetType());Debug.WriteLine((typeof(SqlException)));
                if (exception.InnerException.GetType().Equals(typeof(SqlException)))
                {
                    Debug.WriteLine(exception.Message);
                    Debug.WriteLine(exception.GetBaseException());
                    response = Request.CreateResponse(HttpStatusCode.OK, new EMSResponseMessage("EMS_101", "Invalid Employee ID", exception.Message));
                }
                else
                {
                    Debug.WriteLine(exception.Message);
                    Debug.WriteLine(exception.GetBaseException());
                    response = Request.CreateResponse(HttpStatusCode.OK, new EMSResponseMessage("EMS_101", "Application Error", exception.Message));
                }
            }
            return response;
        }

        /// <summary>
        /// to get asset details and log using asset id 
        /// </summary>
        /// <param name="asset_id"></param>
        /// <returns></returns>
        [Route("api/v2/get/asset/details/{asset_id?}")]
        [HttpGet]
        public HttpResponseMessage GetAssetDetailsByID(int asset_id)
        {
            HttpResponseMessage response = null;
            try
            {
                if(asset_id != 0)
                {
                    Dictionary<string, object> result_set = new Dictionary<string, object>();
                    result_set.Add("asset_details",AssetRepo.GetAssetDetailsByID(asset_id,null));
                    result_set.Add("asset_log", (AssetRepo.GetAssetLogDetails(asset_id)));
                    response = Request.CreateResponse(HttpStatusCode.OK, new EMSResponseMessage("EMS_001", "Success", result_set));
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
        /// To get assets assigned to an employee
        /// </summary>
        /// <param name="employee_id"> unique id </param>
        /// <returns></returns>
        [Route("api/v2/get/asset/{employee_id?}")]
        public HttpResponseMessage GetAssetByemployee_Id(int employee_id)
        {
            HttpResponseMessage response = null;
            try
            {
                if(employee_id != 0)
                {
                    Dictionary<string, object> result_set = new Dictionary<string, object>();
                    result_set.Add("CurrentAssignedAsset", AssetRepo.GetCurrentAssetByEmployeeId(employee_id));
                    result_set.Add("PreviousAssignedAsset", AssetRepo.GetPreviousAssetByEmployeeId(employee_id));
                    response = Request.CreateResponse(HttpStatusCode.OK, new EMSResponseMessage("EMS_001", "Success", result_set));
                }
                else
                {
                    response = Request.CreateResponse(HttpStatusCode.OK, new EMSResponseMessage("EMS_102", "Invalid Employee ID", "Please check input Json"));
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
