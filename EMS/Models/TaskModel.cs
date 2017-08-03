using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace EMS.Models
{
	public class TaskModel
	{
        public int id { get; set; }
        public string task_name { get; set; }
        public string task_description { get; set; }
        public int project_id { get; set; }
    }
}