using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace EMS.Models
{
    public class Password_Token
    {
        public int Employee_Id { get; set; }
        public string Token { get; set; }
        public DateTime Generated_on { get; set; }
    }
}