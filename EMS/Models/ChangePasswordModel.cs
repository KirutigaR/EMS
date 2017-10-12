using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace EMS.Models
{
    public class ChangePasswordModel
    {
        public int employee_id { get; set; }
        public string oldpassword { get; set; }
        public string new_password { get; set; }
        public string confirm_password { get; set; }
    }
}