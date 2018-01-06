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
    }
}