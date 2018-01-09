using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace EMS.Models
{
    public class AssetLogModel
    {
        public int id { get; set; }
        public string employee_name { get; set; }
        public string asset_serial_no { get; set; }
        public DateTime assigned_on { get; set; }
        public Nullable<DateTime> released_on { get; set; }
    }
}