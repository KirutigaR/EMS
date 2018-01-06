using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace EMS.Models
{
    public class AssetLogModel
    {
        public int id { get; set; }
        public int employee_id { get; set; }
        public int asset_id { get; set; }
        public DateTime assigned_on { get; set; }
        public Nullable<DateTime> released_on { get; set; }
    }
}