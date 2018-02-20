using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Net;
using System.Net.Mail;
using System.Diagnostics;
using EMS.Models;

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
                mail.CC.Add(Constants.HR_MAIL_ID);
                //Console.WriteLine("Your Message");
                switch(status)
                {
                    case 1://LEAVE_STATUS_PENDING = 1
                        mail.To.Add(reporting_to_mailid);
                        mail.CC.Add(user_mail);
                        mail.Subject = "Employee Leave Application";
                        mail.Body = "Hi " + Reportingto_name + "<br><br>Your team member " + user_name + " applied leave from "
                                //+ from_date.ToShortDateString() + " to " + to_date.ToShortDateString() 
                                + from_date.Day + "-" + from_date.ToString("MMM") + "-" + from_date.Year + " to "
                                + to_date.Day + "-" + to_date.ToString("MMM") + "-" + to_date.Year
                                + ". <br><br>Kindly login <a href=http://192.168.1.19:8080/>here</a> to approve or reject.<br><br>Regards,<br>Jaishu Consulting Pvt. Ltd.";
                        break;
                    case 2://Constants.LEAVE_STATUS_APPROVED = 2
                        if (from_date < DateTime.Now)
                        {
                            mail.To.Add(user_mail);
                            mail.CC.Add(reporting_to_mailid);
                            mail.Subject = "Employee Leave Status";
                            mail.Body = "Hi " + user_name + "<br><br>Your leave application from the date " 
                                + from_date.Day + "-" + from_date.ToString("MMM") + "-" + from_date.Year + " to " 
                                + to_date.Day + "-" + to_date.ToString("MMM") + "-" + to_date.Year 
                                + " is approved automatically by the system. <br><br>Regards,<br> Jaishu Consulting Pvt. Ltd.";
                        }
                        else
                        {
                            mail.To.Add(user_mail);
                            mail.CC.Add(reporting_to_mailid);
                            mail.Subject = "Employee Leave Status";
                            mail.Body = "Hi " + user_name + "<br><br>Your leave application from the date "
                                //+ from_date.ToShortDateString() + " to " + to_date.ToShortDateString()
                                + from_date.Day + "-" + from_date.ToString("MMM") + "-" + from_date.Year + " to "
                                + to_date.Day + "-" + to_date.ToString("MMM") + "-" + to_date.Year
                                + " has been approved by " + Reportingto_name + " <br><b>Comments : </b>" + remarks + "<br><br>Regards,<br> Jaishu Consulting Pvt. Ltd.";
                        }
                        break;
                    case 3://LEAVE_STATUS_REJECTED = 3
                        mail.To.Add(user_mail);
                        mail.CC.Add(reporting_to_mailid);
                        mail.Subject = "Employee Leave Status";
                        mail.Body = "Hi " + user_name + "<br><br>Your leave application from the date "
                            //+ from_date.ToShortDateString() + " to " + to_date.ToShortDateString() 
                            + from_date.Day + "-" + from_date.ToString("MMM") + "-" + from_date.Year + " to "
                            + to_date.Day + "-" + to_date.ToString("MMM") + "-" + to_date.Year
                            + " has been rejected by " + Reportingto_name + " <br><b>Comments : </b>" + remarks + "<br><br> Regards,<br> Jaishu Consulting Pvt. Ltd.";
                        break;
                    case 4://LEAVE_STATUS_CANCELLED = 4
                        mail.To.Add(reporting_to_mailid);
                        mail.CC.Add(user_mail);
                        mail.Subject = "Jaishu Leave Management";
                        mail.Body = "Hi " + Reportingto_name + "<br><br>Your team member " + user_name + " cancelled a leave application from "
                            //+ from_date.ToShortDateString() + " to " + to_date.ToShortDateString() 
                            + from_date.Day + "-" + from_date.ToString("MMM") + "-" + from_date.Year + " to "
                            + to_date.Day + "-" + to_date.ToString("MMM") + "-" + to_date.Year
                            + ". <br><br>Kindly login <a href=http://192.168.1.19:8080/>here</a> to check the status.<br><br>Regards,<br>Jaishu Consulting Pvt. Ltd.";
                        break;
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

        public static void AssetMailing(string username, string user_mail, string asset_status, List<AssetModel>Asset_Details, DateTime assignedon_date)
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
                mail.CC.Add(Constants.HR_MAIL_ID);
                mail.Subject = "Jaishu Consulting pvt. ltd.";
                if (asset_status == "ASSIGNED")
                {
                    mail.Body = "Hi " + username + "," + "<br><br>The following Assets has been <b>Assigned</b> to you on <b>" + assignedon_date.Day + "-" + assignedon_date.ToString("MMM") + "-" + assignedon_date.Year+"</b>.";
                    mail.Body += "<br><br><table border="+1+ " width="+"80%"+ "><tr><th>Asset Serial No.</th><th>Asset Type</th><th>Model</th><th>Make</th></tr>";
                    foreach (AssetModel asset_item in Asset_Details)
                    {
                        mail.Body += "<tr><td>" + asset_item.asset_serial_no+ "</td><td>"+ asset_item.type_name+ "</td><td>"+ asset_item.model+ "</td><td>"+ asset_item.make+ "</td></tr>";
                    }
                    mail.Body += "</table>";
                    mail.Body += "<br><br>Kindly handle carefully because you will be the complete responsibility in case of damage.  <br><br>Regards,<br>Jaishu Consulting Pvt. Ltd.";
                }
                else if(asset_status == "RELEASED")
                {
                    mail.Body = "Hi " + username + "," + "<br><br>The Following Assets has been <b>Released</b> from your name on <b>" + DateTime.Now.Date.Day + "-" + DateTime.Now.Date.ToString("MMM") + "-" + DateTime.Now.Date.Year + "</b>.";
                    mail.Body += "<br><br><table border=" + 1 + " width=" + "80%" + "><tr><th>Asset Serial No.</th><th>Asset Type</th><th>Model</th><th>Make</th></tr>";
                    foreach (AssetModel asset_item in Asset_Details)
                    {
                        mail.Body += "<tr><td>" + asset_item.asset_serial_no + "</td><td>" + asset_item.type_name + "</td><td>" + asset_item.model + "</td><td>" + asset_item.make + "</td></tr>";
                    }
                    mail.Body += "</table>";
                    mail.Body += "<br><br>Regards,<br>Jaishu Consulting Pvt. Ltd."; 
                }
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