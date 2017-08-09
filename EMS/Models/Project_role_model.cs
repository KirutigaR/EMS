using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace EMS.Models
{
    public class Project_role_model
    {
        public int id { get; set; }
        public string employee_name { get; set; }
        public string project_name { get; set; }
        public string role_name { get; set; }
        public System.DateTime start_date { get; set; }
        public System.DateTime end_date { get; set; }
        public int association { get; set; }
    }
}