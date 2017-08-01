using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace EMS.Models
{
	public class TimeSheetModel
	{
        public int id { get; set; }
        public int employee_id { get; set; }
        public int task_id { get; set; }
        public System.DateTime work_date { get; set; }
        public int work_hour { get; set; }
        public int project_id { get; set; }

    }
}