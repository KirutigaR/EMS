﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace EMS.Models
{
    public class LeavehistoryModel
    {
        public int employee_id { get; set; }
        public string first_name { get; set; }
        public string last_name { get; set; }
        public string type_name { get; set; }
        public DateTime from_date { get; set; }
        public DateTime to_date { get; set; }
        public decimal no_of_days { get; set; }
        public int reportingto { get; set; }
        public string reporting_to { get; set; }
        public string leave_status { get; set; }
        public int leave_id { get; set; }
        public string remark { get; set; }
        //public int cancel_flag { get; set; }
    }
}