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

        public static List<AssetModel> GetAssetList(int status_id)
        {
            EMSEntities datacontext = new EMSEntities();
            try
            {
                var query = from asset in datacontext.Assets
                            join asset_type in datacontext.Asset_type on asset.type_id equals asset_type.id
                            where asset.status_id == status_id
                            select new AssetModel
                            {
                                id = asset.id,
                                type_name = asset_type.asset_type,
                                model = asset.model,
                                make = asset.make,
                                asset_serial_no = asset.asset_serial_no,
                                warranty_expiry_date = asset.warranty_expiry_date,
                                employee_name = (from employee in datacontext.Employees
                                                 where employee.id == (from employee_assert in datacontext.Employee_Asset
                                                                       where employee_assert.asset_id == asset.id && employee_assert.released_on == null
                                                                       select employee_assert.employee_id).FirstOrDefault()
                                                 select employee.first_name + " " + employee.last_name).FirstOrDefault(),
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
                    var asset_obj = (from asset in datacontext.Assets where asset.id == assert_id select asset).FirstOrDefault();
                    asset_obj.status_id = Asset_details.status_id;
                    datacontext.Entry(asset_obj).State = EntityState.Modified;
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

        public static AssetModel GetAssetDetailsByID(int asset_id)
        {
            EMSEntities datacontext = new EMSEntities();
            try
            {
                var query = from asset in datacontext.Assets
                            join asset_type in datacontext.Asset_type on asset.type_id equals asset_type.id
                            join asset_status in datacontext.Status on asset.status_id equals asset_status.id
                            where asset.id == asset_id
                            select new AssetModel
                            {
                                id = asset.id,
                                type_name = asset_type.asset_type,
                                type_id = asset.type_id,
                                model = asset.model,
                                make = asset.make,
                                purchase_date = asset.purchase_date,
                                invoice_no = asset.invoice_no,
                                vendor_name = asset.vendor_name,
                                asset_serial_no = asset.asset_serial_no,
                                status_name = asset_status.status,
                                status_id = asset.status_id,
                                notes = asset.notes,
                                scrap_date = asset.scrap_date,
                                price = asset.price,
                                warranty_expiry_date = asset.warranty_expiry_date,
                                employee_name = (from employee in datacontext.Employees where employee.id == ( from employee_assert in datacontext.Employee_Asset
                                               where employee_assert.asset_id == asset_id && employee_assert.released_on == null
                                               select employee_assert.employee_id).FirstOrDefault()
                                               select employee.first_name +" "+ employee.last_name).FirstOrDefault(),
                                employee_id = (from employee_assert in datacontext.Employee_Asset
                                               where employee_assert.asset_id == asset_id && employee_assert.released_on == null
                                               select employee_assert.employee_id).FirstOrDefault()
                            };
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

        public static List<AssetLogModel> GetAssetLogDetails(int asset_id)
        {
            EMSEntities datacontext = new EMSEntities();
            try
            {
                var query = from x in datacontext.Employee_Asset
                            where x.asset_id == asset_id
                            select new AssetLogModel
                            {
                                id = x.id,
                                employee_id = x.employee_id,
                                asset_id = x.asset_id,
                                assigned_on = x.assigned_on,
                                released_on = x.released_on
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