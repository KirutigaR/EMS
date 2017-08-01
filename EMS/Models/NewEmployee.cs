using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace EMS.Models
{
    public class NewEmployee
    {
        public int id { get; set; }
        public string first_name { get; set; }
        public string last_name { get; set; }
        public string email { get; set; }
        public System.DateTime date_of_birth { get; set; }
        public System.DateTime date_of_joining { get; set; }
        public string contact_no { get; set; }
        public int user_id { get; set; }
        public int reporting_to { get; set; }
        public decimal Year_of_experence { get; set; }
        public int role_id { get; set; }
    }
}