using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Diagnostics;
using System.Linq;
using System.Web;
using EMS.Models;

namespace EMS.Repository
{
	public class AssetRepo
	{
        public static List<ListModel> GetAssetTypeList()
        {
            EMSEntities datacontext = new EMSEntities();
            try
            {
                var query = from asset_type in datacontext.Asset_type
                            select new ListModel
                            {
                                type_id = asset_type.id,
                                type_name = asset_type.asset_type
                            };
                return query.ToList();
            }
            catch (Exception e)
            {
                Debug.WriteLine(e.Message);
                Debug.WriteLine(e.GetBaseException());
                throw e;
            }
            finally
            {
                datacontext.Dispose();
            }
        }

        public static List<ListModel> GetAssetStatusList()
        {
            EMSEntities datacontext = new EMSEntities();
            try
            {
                var query = from status in datacontext.Status
                            where status.description == "Asset"
                            select new ListModel
                            {
                                type_id = status.id,
                                type_name = status.status
                            };
                return query.ToList();
            }
            catch (Exception e)
            {
                Debug.WriteLine(e.Message);
                Debug.WriteLine(e.GetBaseException());
                throw e;
            }
            finally
            {
                datacontext.Dispose();
            }
        }

        public static void CreateAsset(Asset asset)
        {
            EMSEntities datacontext = new EMSEntities();
            try
            {
                datacontext.Assets.Add(asset);
                datacontext.SaveChanges();
            }
            catch (Exception e)
            {
                Debug.WriteLine(e.Message);
                Debug.WriteLine(e.GetBaseException());
                throw e;
            }
            finally
            {
                datacontext.Dispose();
            }
        }

        public static void UpdateAsset(Asset asset)
        {
            EMSEntities datacontext = new EMSEntities();
            try
            {
                datacontext.Entry(asset).State = EntityState.Modified;
                datacontext.SaveChanges();
            }
            catch (Exception e)
            {
                Debug.WriteLine(e.Message);
                Debug.WriteLine(e.GetBaseException());
                throw e;
            }
            finally
            {
                datacontext.Dispose();
            }
        }

        public static List<AssetModel> GetAvailableAssetList()
        {
            EMSEntities datacontext = new EMSEntities();
            try
            {
                var query = from asset in datacontext.Assets
                            join asset_type in datacontext.Asset_type on asset.type_id equals asset_type.id
                            where asset.status_id == 6
                            select new AssetModel
                            {
                                id = asset.id,
                                type_name = asset_type.asset_type,
                                model = asset.model,
                                make = asset.make,
                                asset_serial_no = asset.asset_serial_no,
                                warranty_expiry_date = asset.warranty_expiry_date
                            };
                return query.ToList();
            }
            catch (Exception e)
            {
                Debug.WriteLine(e.Message);
                Debug.WriteLine(e.GetBaseException());
                throw e;
            }
            finally
            {
                datacontext.Dispose();
            }
        }

        public static bool AssignAsset(AssetModel Asset_Assign_Details)
        {
            EMSEntities datacontext = new EMSEntities();
            try
            {
                Employee_Asset Assign_obj = new Employee_Asset();
                foreach(int asset_id in Asset_Assign_Details.asset_id_list)
                {
                    Assign_obj.asset_id = asset_id;
                    Assign_obj.employee_id = Asset_Assign_Details.employee_id;
                    Assign_obj.assigned_on = Asset_Assign_Details.assigned_on;
                    datacontext.Employee_Asset.Add(Assign_obj);
                    datacontext.SaveChanges();
                }
                return true;
            }
            catch (Exception e)
            {
                Debug.WriteLine(e.Message);
                Debug.WriteLine(e.GetBaseException());
                throw e;
            }
            finally
            {
                datacontext.Dispose();
            }
        }

        public static bool UpdateAssetStatus(AssetModel Asset_details)
        {
            EMSEntities datacontext = new EMSEntities();
            try
            {
                foreach(int assert_id in Asset_details.asset_id_list)
                {
                    //assert_obj.id = assert_id;
                    var asset_obj = from asset in datacontext.Assets where asset.id == assert_id select asset;
                    asset_obj.FirstOrDefault().status_id = Asset_details.status_id;
                    datacontext.Entry(asset_obj.FirstOrDefault()).State = EntityState.Modified;
                }
                datacontext.SaveChanges();
                return true;
            }
            catch (Exception e)
            {
                Debug.WriteLine(e.Message);
                Debug.WriteLine(e.GetBaseException());
                throw e;
            }
            finally
            {
                datacontext.Dispose();
            }
        }

        public static Asset GetAssetInstance(int asset_id)
        {
            EMSEntities datacontext = new EMSEntities();
            try
            {
                var query = from asset in datacontext.Assets
                            where asset.id == asset_id
                            select asset;
                return query.FirstOrDefault();
            }
            catch (Exception e)
            {
                Debug.WriteLine(e.Message);
                Debug.WriteLine(e.GetBaseException());
                throw e;
            }
            finally
            {
                datacontext.Dispose();
            }
        }

        public static bool LoadAssetDataFromTable()
        {
            EMSEntities datacontext = new EMSEntities();
            try
            {
                var query = datacontext.MoveToAssetTable();
                return true;
            }
            catch (Exception e)
            {
                Debug.WriteLine(e.Message);
                Debug.WriteLine(e.GetBaseException());
                throw e;
            }
            finally
            {
                datacontext.Dispose();
            }
        }
    }
}