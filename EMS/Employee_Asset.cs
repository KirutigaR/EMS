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
    
    public partial class Employee_Asset
    {
        public int id { get; set; }
        public int employee_id { get; set; }
        public int asset_id { get; set; }
        public System.DateTime assigned_on { get; set; }
        public Nullable<System.DateTime> released_on { get; set; }
    
        public virtual Employee Employee { get; set; }
        public virtual Asset Asset { get; set; }
    }
}
