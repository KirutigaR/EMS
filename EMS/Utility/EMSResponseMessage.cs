using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace EMS.Utility
{
    public class EMSResponseMessage
    {
        public String Code { get; set; }
        public String Message { get; set; }
        public Object Data{ get; set; }
        public EMSResponseMessage(String code, String message, Object data)
        {
            this.Code = code;
            this.Message = message;
            this.Data = data;
        }
    }
}