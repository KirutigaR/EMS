﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace EMS.Models
{
    public class LeaveStatusModel
    {
        public int leave_id { get; set; }
        public int status_id { get; set; }
        public string remarks { get; set; }
    }
}