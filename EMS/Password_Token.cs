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
    
    public partial class Password_Token
    {
        public int ID { get; set; }
        public int User_Id { get; set; }
        public string Token { get; set; }
        public System.DateTime Generated_on { get; set; }
    
        public virtual User User { get; set; }
    }
}