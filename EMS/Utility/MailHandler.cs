using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Net;
using System.Net.Mail;
using System.Diagnostics;

namespace EMS.Utility
{
    public class MailHandler
    {
        public static void PasswordMailingFunction(string username, string user_mail, string password)
        {
            try
            {

                SmtpClient SmtpServer = new SmtpClient();
                SmtpServer.Credentials = new NetworkCredential("kirutiga96@gmail.com", "Structuring*Data.");
                SmtpServer.Port = 587;
                SmtpServer.Host = "smtp.gmail.com";
                SmtpServer.EnableSsl = true;
                MailMessage mail = new MailMessage();
                mail.From = new MailAddress("kirutiga96@gmail.com");
            
                //String[] addr = toemail.Split('kirutigaramesh@gmail.com'); // toemail is a string which contains many email address separated by comma
                //mail.From = new MailAddress(toemail);
                //Byte i;
                //for (i = 0; i < addr.Length; i++)
                mail.To.Add(user_mail);

                mail.Subject = "Employee Login credentials";

                //mail.Body = "\n HI "+emp_name+"\n\t Your Leave application has been approved and it is from "+from_date+"to "+to_date+"\n Thank You";
                mail.Body = "Hi " + username + ".." + "<br><br> Welcome to <b>Jaishu Consulting Pvt. ltd.</b> Your Employee account has been created, please find the login credentials specified below <em><b> <br><br> Login&nbsp;:&nbsp;" + user_mail + "<br>Password&nbsp;:&nbsp;" + password + "</em></b><br><br>To Login Click <a href=https://www.google.co.in/>here</a> Thank You";

                mail.IsBodyHtml = true;
                //mail.DeliveryNotificationOptions = DeliveryNotificationOptions.OnFailure;
                //mail.ReplyTo = new MailAddress("praveen@jaishu.com");
                //mail.ReplyToList.Add(toemail);
                SmtpServer.Send(mail);
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex.Message);
                Debug.WriteLine(ex.GetBaseException());
            }
            //  }
        }

    }
}