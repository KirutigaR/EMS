//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated from a template.
//
//     Manual changes to this file may cause unexpected behavior in your application.
//     Manual changes to this file will be overwritten if the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace EMS
{
    using System;
    using System.Collections.Generic;
    
    public partial class Payslip
    {
        public int id { get; set; }
        public int emp_id { get; set; }
        public int payslip_month { get; set; }
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
        public decimal incometax { get; set; }
        public decimal arrears { get; set; }
    
        public virtual Employee Employee { get; set; }
    }
}
