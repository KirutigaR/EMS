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
    
    public partial class Employee
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public Employee()
        {
            this.Employee1 = new HashSet<Employee>();
            this.Leavebalance_sheet = new HashSet<Leavebalance_sheet>();
            this.Project_role = new HashSet<Project_role>();
            this.Timesheets = new HashSet<Timesheet>();
            this.Incometaxes = new HashSet<Incometax>();
            this.Salary_Structure = new HashSet<Salary_Structure>();
            this.Payslips = new HashSet<Payslip>();
            this.Leaves = new HashSet<Leave>();
        }
    
        public int id { get; set; }
        public string first_name { get; set; }
        public string last_name { get; set; }
        public string email { get; set; }
        public System.DateTime date_of_birth { get; set; }
        public System.DateTime date_of_joining { get; set; }
        public string contact_no { get; set; }
        public int user_id { get; set; }
        public int reporting_to { get; set; }
        public string gender { get; set; }
        public string pan_no { get; set; }
        public string bank_account_no { get; set; }
        public string PF_no { get; set; }
        public string medical_insurance_no { get; set; }
        public string emergency_contact_no { get; set; }
        public string emergency_contact_person { get; set; }
        public int designation { get; set; }
        public string blood_group { get; set; }
        public Nullable<decimal> year_of_experience { get; set; }
        public Nullable<System.DateTime> created_on { get; set; }
    
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<Employee> Employee1 { get; set; }
        public virtual Employee Employee2 { get; set; }
        public virtual User User { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<Leavebalance_sheet> Leavebalance_sheet { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<Project_role> Project_role { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<Timesheet> Timesheets { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<Incometax> Incometaxes { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<Salary_Structure> Salary_Structure { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<Payslip> Payslips { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<Leave> Leaves { get; set; }
    }
}
