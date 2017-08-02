using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace EMS.Utility
{
    public class PasswordGenerator
    {
        private static int lengthOfPassword = 8;
        public static string GeneratePassword()
        {
           
            string passCode = DateTime.Now.Ticks.ToString();
            string pass = BitConverter.ToString(new System.Security.Cryptography.SHA512CryptoServiceProvider().ComputeHash(System.Text.Encoding.Default.GetBytes(passCode))).Replace("-", String.Empty);
            return pass.Substring(0, lengthOfPassword);
        }

    }
}