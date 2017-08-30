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
                throw ex;
            }
            //  }
        }
        public static void LeaveMailing(DateTime from_date, DateTime to_date, string user_name, int status, string user_mail)
        {
            try
            {
                

                //Console.WriteLine("Your Message");
                if (status == 2)
                {
                    //Console.WriteLine("Mail To");
                    MailAddress to = new MailAddress(user_mail);

                    //Console.WriteLine("Mail From");
                    MailAddress from = new MailAddress("testems32@gmail.com");

                    MailMessage mail = new MailMessage(from, to);
                    mail.CC.Add("");
                    //Console.WriteLine("Subject");
                    mail.Subject = "Employee Leave Status";
                    mail.Body = "Hi" + user_name + "Your leave application" + from_date + "-" + to_date + "has been approved";
                }
                else if(status == 3)
                {
                    //Console.WriteLine("Mail To");
                    MailAddress to = new MailAddress(user_mail);

                    //Console.WriteLine("Mail From");
                    MailAddress from = new MailAddress("testems32@gmail.com");

                    MailMessage mail = new MailMessage(from, to);
                    mail.CC.Add("");
                    //Console.WriteLine("Subject");
                    mail.Subject = "Employee Leave Status";
                    mail.Body = "Hi" + user_name + "Your leave application" + from_date + "-" + to_date + "has been Rejected";
                }
                else 
                {
                    //Console.WriteLine("Mail To");
                    MailAddress to = new MailAddress("");

                    //Console.WriteLine("Mail From");
                    MailAddress from = new MailAddress(user_mail);

                    MailMessage mail = new MailMessage(from, to);
                    mail.CC.Add("");
                    //Console.WriteLine("Subject");
                    mail.Subject = "Employee Leave Application";
                    mail.Body = "hi" + user_name + "applied leave from" + from_date + "to" + to_date;
                }

                //System.Net.Mail.Attachment attachment;
                //attachment = new System.Net.Mail.Attachment("c:/textfile.txt");
                //mail.Attachments.Add(attachment);


                SmtpClient smtp = new SmtpClient();
                smtp.Port = 587;
                smtp.Host = "smtp.gmail.com";

                smtp.UseDefaultCredentials = true;
                smtp.EnableSsl = true;
                smtp.Credentials = new NetworkCredential("sathranathbaskaran17@gmail.com", "muralidhoni7");
                //smtp.EnableSsl = true;
                Console.WriteLine("Sending email...");
                smtp.Send(mail);
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex.Message);
                Debug.WriteLine(ex.GetBaseException());
                throw ex;
            }

        }
        public static void ApplyLeaveMailing(DateTime from_date, DateTime to_date, string user_name, int status, string user_mail)
        {
            try
            {
                //Console.WriteLine("Mail To");
                MailAddress to = new MailAddress(user_mail);

                //Console.WriteLine("Mail From");
                MailAddress from = new MailAddress("testems32@gmail.com");
                //MailAddress cc = new MailAddress("", "");

                MailMessage mail = new MailMessage(from, to);
                mail.CC.Add()

                //Console.WriteLine("Subject");
                mail.Subject = "Employee Leave Status";

                //Console.WriteLine("Your Message");
                if (status == 2)
                {
                    mail.Body = "Hi" + user_name + "Your leave application" + from_date + "-" + to_date + "has been approved";
                }
                else //if(status == 3)
                {
                    mail.Body = "Hi" + user_name + "Your leave application" + from_date + "-" + to_date + "has been Rejected";
                }


                //System.Net.Mail.Attachment attachment;
                //attachment = new System.Net.Mail.Attachment("c:/textfile.txt");
                //mail.Attachments.Add(attachment);


                SmtpClient smtp = new SmtpClient();
                smtp.Port = 587;
                smtp.Host = "smtp.gmail.com";

                smtp.UseDefaultCredentials = true;
                smtp.EnableSsl = true;
                smtp.Credentials = new NetworkCredential("sathranathbaskaran17@gmail.com", "muralidhoni7");
                //smtp.EnableSsl = true;
                Console.WriteLine("Sending email...");
                smtp.Send(mail);
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex.Message);
                Debug.WriteLine(ex.GetBaseException());
                throw ex;
            }

        }
    }
}