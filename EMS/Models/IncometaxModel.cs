using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace EMS.Models
{
    public class IncometaxModel
    {
        public int id { get; set; }
        public int emp_id { get; set; }
        public int is_active { get; set; }
        public decimal income_tax { get; set; }
        public System.DateTime from_date { get; set; }
        public Nullable<System.DateTime> to_date { get; set; }
        public string notes { get; set; }
    }
}