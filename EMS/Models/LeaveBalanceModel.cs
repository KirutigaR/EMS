using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace EMS.Models
{
    public class LeaveBalanceModel
    {
        public int leavetype_id { get; set; }
        public string type_name { get; set; }
        public decimal no_of_days { get; set; }
    }
}