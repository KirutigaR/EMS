using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Diagnostics;
using System.Linq;
using System.Web;

namespace EMS.Repository
{
	public class AssetRepo
	{
        public static List<Asset_type> GetAssetTypeList()
        {
            EMSEntities datacontext = new EMSEntities();
            try
            {
                var query = from asset_type in datacontext.Asset_type
                            select asset_type;
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

        public static List<Status> GetAssetStatusList()
        {
            EMSEntities datacontext = new EMSEntities();
            try
            {
                var query = from status in datacontext.Status
                            where status.description == "Asset"
                            select status;
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
    }
}