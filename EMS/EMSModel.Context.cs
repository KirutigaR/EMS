﻿//------------------------------------------------------------------------------
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
    using System.Data.Entity;
    using System.Data.Entity.Infrastructure;
    
    public partial class EMSEntities : DbContext
    {
        public EMSEntities()
            : base("name=EMSEntities")
        {
        }
    
        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            throw new UnintentionalCodeFirstException();
        }
    
        public virtual DbSet<Holiday_List> Holiday_List { get; set; }
        public virtual DbSet<Leave> Leaves { get; set; }
        public virtual DbSet<Leave_type> Leave_type { get; set; }
        public virtual DbSet<Leavebalance_sheet> Leavebalance_sheet { get; set; }
        public virtual DbSet<Project> Projects { get; set; }
        public virtual DbSet<Project_role> Project_role { get; set; }
        public virtual DbSet<Role> Roles { get; set; }
        public virtual DbSet<Task> Tasks { get; set; }
        public virtual DbSet<Timesheet> Timesheets { get; set; }
        public virtual DbSet<User_role> User_role { get; set; }
        public virtual DbSet<User> Users { get; set; }
        public virtual DbSet<Designation> Designations { get; set; }
        public virtual DbSet<Client> Clients { get; set; }
        public virtual DbSet<Client_type> Client_type { get; set; }
        public virtual DbSet<Employee> Employees { get; set; }
        public virtual DbSet<Status_leave> Status_leave { get; set; }
    }
}
