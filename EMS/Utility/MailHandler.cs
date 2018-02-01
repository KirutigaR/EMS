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
                SmtpServer.Credentials = new NetworkCredential("testems32@gmail.com", "Testem$32");
                SmtpServer.Port = 587;
                SmtpServer.Host = "smtp.gmail.com";
                SmtpServer.EnableSsl = true;
                MailMessage mail = new MailMessage();
                mail.From = new MailAddress("testems32@gmail.com", "Jaishu EMS");

                //String[] addr = toemail.Split('kirutigaramesh@gmail.com'); // toemail is a string which contains many email address separated by comma
                //mail.From = new MailAddress(toemail);
                //Byte i;
                //for (i = 0; i < addr.Length; i++)
                mail.To.Add(user_mail);

                mail.Subject = "Employee Login credentials";

                //mail.Body = "\n HI "+emp_name+"\n\t Your Leave application has been approved and it is from "+from_date+"to "+to_date+"\n Thank You";
                mail.Body = "Hi " + username + "," + "<br><br> Welcome to <b>Jaishu Consulting Private Limited.</b><br><br> Your Employee account has been created, please find the login credentials specified below <em><b> <br><br> Username&nbsp;:&nbsp;" + user_mail + "<br>Password&nbsp;:&nbsp;" + password + "</em></b><br><br>Click <a href=http://192.168.1.19:8080/>here</a> to Login.<br><br>Regards,<br>Jaishu Consulting Pvt. Ltd.";

                mail.IsBodyHtml = true;
                //mail.DeliveryNotificationOptions = DeliveryNotificationOptions.OnFailure;
                //mail.ReplyTo = new MailAddress("praveenk@jaishu.com");
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
        public static void LeaveMailing(DateTime from_date, DateTime to_date, string user_name, int status, string user_mail, string reporting_to_mailid, string remarks, string Reportingto_name)
        {
            try
            {
                MailMessage mail = new MailMessage();
                mail.From = new MailAddress("testems32@gmail.com", "Jaishu EMS");
                mail.CC.Add("praveenkk@jaishu.com");
                //Console.WriteLine("Your Message");
                if (status == Constants.LEAVE_STATUS_APPROVED)
                {
                    mail.To.Add(user_mail);
                    mail.CC.Add(reporting_to_mailid);
                    mail.Subject = "Employee Leave Status";
                    mail.Body = "Hi " + user_name + "<br><br>Your leave application from the date " + from_date.ToShortDateString() + " to " + to_date.ToShortDateString() + " has been approved by " + Reportingto_name + " <br><b>Comments : </b>" + remarks + "<br><br>Regards,<br> Jaishu Consulting Pvt. Ltd.";
                }
                else if (status == Constants.LEAVE_STATUS_REJECTED)
                {
                    mail.To.Add(user_mail);
                    mail.CC.Add(reporting_to_mailid);
                    mail.Subject = "Employee Leave Status";
                    mail.Body = "Hi " + user_name + "<br><br>Your leave application from the date " + from_date.ToShortDateString() + " to " + to_date.ToShortDateString() + " has been rejected by " + Reportingto_name + " <br><b>Comments : </b>" + remarks + "<br><br> Regards,<br> Jaishu Consulting Pvt. Ltd.";
                }
                else if (status == Constants.LEAVE_STATUS_PENDING)
                {
                    mail.To.Add(reporting_to_mailid);
                    mail.CC.Add(user_mail);
                    mail.Subject = "Employee Leave Application";
                    mail.Body = "Hi " + Reportingto_name + "<br><br>Your team member " + user_name + " applied leave from " + from_date.ToShortDateString() + " to " + to_date.ToShortDateString() + ". <br><br>Kindly login <a href=http://192.168.1.19:8080/>here</a> to approve or reject.<br><br>Regards,<br>Jaishu Consulting Pvt. Ltd.";
                }
                else
                {
                    mail.To.Add(reporting_to_mailid);
                    mail.CC.Add(user_mail);
                    mail.Subject = "Jaishu Leave Management";
                    mail.Body = "Hi " + Reportingto_name + "<br><br>Your team member " + user_name + " cancelled a leave application from " + from_date.ToShortDateString() + " to " + to_date.ToShortDateString() + ". <br><br>Kindly login <a href=http://192.168.1.19:8080/>here</a> to check the status.<br><br>Regards,<br>Jaishu Consulting Pvt. Ltd.";
                }
                //System.Net.Mail.Attachment attachment;
                //attachment = new System.Net.Mail.Attachment("c:/textfile.txt");
                //mail.Attachments.Add(attachment);


                SmtpClient smtp = new SmtpClient();
                smtp.Port = 587;
                smtp.Host = "smtp.gmail.com";

                smtp.UseDefaultCredentials = true;
                smtp.EnableSsl = true;
                smtp.Credentials = new NetworkCredential("testems32@gmail.com", "Testem$32");
                //smtp.EnableSsl = true;
                Console.WriteLine("Sending email...");
                mail.IsBodyHtml = true;
                smtp.Send(mail);
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex.Message);
                Debug.WriteLine(ex.GetBaseException());
                throw ex;
            }
        }

        public static void ForgotPassword(string username, string user_mail)
        {
            try
            {

                SmtpClient SmtpServer = new SmtpClient();
                SmtpServer.Credentials = new NetworkCredential("testems32@gmail.com", "Testem$32");
                SmtpServer.Port = 587;
                SmtpServer.Host = "smtp.gmail.com";
                SmtpServer.EnableSsl = true;
                MailMessage mail = new MailMessage();
                mail.From = new MailAddress("testems32@gmail.com", "Jaishu EMS");
                mail.To.Add(user_mail);
                mail.Subject = "Jaishu Consulting pvt. ltd.";
                mail.Body = "Hi " + username + ".." + "<br><br>Click <a href=http://192.168.1.19:8080/>here</a> to change your password...<br><br> Thank You";
                mail.IsBodyHtml = true;
                SmtpServer.Send(mail);
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex.Message);
                Debug.WriteLine(ex.GetBaseException());
                throw ex;
            }
        }

        public static void ChangePasswordIntimation(string username, string user_mail)
        {
            try
            {
                SmtpClient SmtpServer = new SmtpClient();
                SmtpServer.Credentials = new NetworkCredential("testems32@gmail.com", "Testem$32");
                SmtpServer.Port = 587;
                SmtpServer.Host = "smtp.gmail.com";
                SmtpServer.EnableSsl = true;
                MailMessage mail = new MailMessage();
                mail.From = new MailAddress("testems32@gmail.com", "Jaishu EMS");
                mail.To.Add(user_mail);
                mail.Subject = "Jaishu Consulting pvt. ltd.";
                mail.Body = "Hi " + username + "," + "<br><br>Your Login password has been changed recently, <br><br>Contact HR if it is not done by you.<br><br>Regards,<br>Jaishu Consulting Pvt. Ltd.";
                mail.IsBodyHtml = true;
                SmtpServer.Send(mail);
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