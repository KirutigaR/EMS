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
                Asset existing_instance = AssetRepo.GetAssetInstance(asset.id);
                if (asset != null)
                {
                    AssetRepo.UpdateAsset(existing_instance);
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

        [HttpPost]
        [Route("api/v1/bulkupload")]
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

                        objbulk.DestinationTableName = "Asset Temp Table";
                        //Mapping Table column    
                        objbulk.ColumnMappings.Add("Employee Id", "Employee_id");
                        objbulk.ColumnMappings.Add("CL", "CL");
                        objbulk.ColumnMappings.Add("EL", "EL");
                        objbulk.ColumnMappings.Add("ML", "ML");
                        objbulk.ColumnMappings.Add("LOP", "LOP");
                        objbulk.ColumnMappings.Add("WFH", "WFH");

                        //inserting Datatable Records to DataBase    
                        con.Open();
                        objbulk.WriteToServer(Exceldt);
                        con.Close();
                        flag = true;
                    }

                    if (flag)
                    {
                        if (!CommonRepo.LoadDataFromTable())
                        {
                            return Request.CreateResponse(HttpStatusCode.OK, new EMSResponseMessage("EMS_305", "Some problem in Stored Procedure", "Some problem in Stored Procedure"));
                        }
                    }
                    return Request.CreateResponse(HttpStatusCode.OK, new EMSResponseMessage("EMS_306", "Assets Updated Successfully", "Assets updated successfully"));

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
