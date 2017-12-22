using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace EMS.Models
{
    public class EmployeeModel
    {
        public int id { get; set; }
        public string first_name { get; set; }
        public string last_name { get; set; }
        public string email { get; set; }
        public System.DateTime date_of_birth { get; set; }
        public System.DateTime date_of_joining { get; set; }
        public string contact_no { get; set; }
        public int user_id { get; set; }
        public int reporting_to { get; set; }
        public string reportingto_name { get; set; }
        public string Year_of_experience { get; set; }
        public string gender { get; set; }
        public int role_id { get; set; }
        public string pan_no { get; set; }
        public string bank_account_no { get; set; }
        public string PF_no { get; set; }
        public string medical_insurance_no { get; set; }
        public string emergency_contact_no { get; set; }
        public string emergency_contact_person { get; set; }
        public int designation_id { get; set; }
        public string designation { get; set; }
        public string blood_group { get; set; }
        public decimal ctc { get; set; }
        public DateTime created_on { get; set; }
        public string role_name { get; set; }
    }
}