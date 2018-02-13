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
    
    public partial class Asset
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public Asset()
        {
            this.Employee_Asset = new HashSet<Employee_Asset>();
        }
    
        public int id { get; set; }
        public int type_id { get; set; }
        public string model { get; set; }
        public string make { get; set; }
        public System.DateTime purchase_date { get; set; }
        public string invoice_no { get; set; }
        public string vendor_name { get; set; }
        public string asset_serial_no { get; set; }
        public int warranty_period { get; set; }
        public int status_id { get; set; }
        public string notes { get; set; }
        public Nullable<System.DateTime> scrap_date { get; set; }
        public decimal price { get; set; }
        public System.DateTime warranty_expiry_date { get; set; }
    
        public virtual Status Status { get; set; }
        public virtual Asset_type Asset_type { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<Employee_Asset> Employee_Asset { get; set; }
    }
}
