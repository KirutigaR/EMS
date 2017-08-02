using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace EMS.Models
{
    public class LeavehistoryModel
    {
        public int id { get; set; }
        public string first_name { get; set; }
        public string last_name { get; set; }
        public string type_name { get; set; }
        public DateTime from_date { get; set; }
        public DateTime to_date { get; set; }
        public int no_of_days { get; set; }
    }
}