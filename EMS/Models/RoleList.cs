using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace EMS.Models
{
    public class RoleList
    {
        public int id { get; set; }
        public string role_name { get; set; }
        public string role_description { get; set; }
        public string role_type { get; set; }
    }
}