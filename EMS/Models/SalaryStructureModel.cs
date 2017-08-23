using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace EMS.Models
{
    public class SalaryStructureModel
    {
        public int id { get; set; }
        public int emp_id { get; set; }
        public decimal ctc { get; set; }
        public decimal basic_pay { get; set; }
        public decimal HRA { get; set; }
        public decimal FA { get; set; }
        public decimal MA { get; set; }
        public decimal CA { get; set; }
        public decimal PF { get; set; }
        public decimal MI { get; set; }
        public decimal ESI { get; set; }
        public decimal Gratuity { get; set; }
        public decimal SA { get; set; }
        public decimal PT { get; set; }
        public int is_active { get; set; }
        public System.DateTime from_date { get; set; }
        public Nullable<System.DateTime> to_date { get; set; }
    }
}