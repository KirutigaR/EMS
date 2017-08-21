using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace EMS.Models
{
    public class PayslipModel
    {
        public int id { get; set; }
        public decimal ctc { get; set; }
        public decimal basic_pay { get; set; }
        public decimal HRA { get; set; }
        public decimal FA { get; set; }
        public decimal MA { get; set; }
        public decimal CA { get; set; }
        public decimal PF { get; set; }
        public decimal MI { get; set; }
        public decimal ESI { get; set; }
        public decimal gratuity { get; set; }
        public decimal SA { get; set; }
        public decimal pt { get; set; }
        public int emp_id { get; set; }
    }
}