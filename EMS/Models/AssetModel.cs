using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace EMS.Models
{
    public class AssetModel
    {
        public int id { get; set; }
        public string type_name { get; set; }
        public string model { get; set; }
        public string make { get; set; }
        public System.DateTime purchase_date { get; set; }
        public string invoice_no { get; set; }
        public string vendor_name { get; set; }
        public string asset_serial_no { get; set; }
        public int warranty_period { get; set; }
        public string status_name { get; set; }
        public string notes { get; set; }
        public Nullable<System.DateTime> scrap_date { get; set; }
        public decimal price { get; set; }
        public System.DateTime warranty_expiry_date { get; set; }

        public List<int> asset_id_list { get; set; }
        public int employee_id { get; set; }
        public string employee_name { get; set; }
        public DateTime assigned_on { get; set; }
        public int status_id { get; set; }
        public int type_id { get; set; }
        public Nullable<DateTime> released_on { get; set; }
        public string employee_mailid { get; set; }
        public int emp_asset_id { get; set; }
    }
}