using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace EMS.Models
{
    public class ProjectModel
    {
        public int project_id { get; set; }
        public string project_name { get; set; }
        public System.DateTime start_date { get; set; }
        public System.DateTime end_date { get; set; }
        public string status { get; set; }
        public string po { get; set; }
        public string project_description { get; set; }
        public int client_id { get; set; }
        public string client_name { get; set; }
        public int type_id { get; set; }
        public int resources_req { get; set; }
    }
}