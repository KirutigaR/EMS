using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace EMS.Models
{
	public class ClientModel
	{
        public string client_name { get; set; }
        public string client_type { get; set; }
        public int is_active { get; set; }
    }
}