using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace EMS.Models
{
	public class ClientModel
	{
        public int client_id { get; set; }
        public string client_name { get; set; }
        public int is_active { get; set; }
        //client_type table fields
        public int client_type_id { get; set;}
        public string client_type_name { get; set; }
        public string client_type_description { get; set; }
    }
}