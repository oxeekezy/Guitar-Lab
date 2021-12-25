using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Mail;
using System.Net;
using System.Web;


namespace GuitLab
{
    public class SendEmail
    {
        public string BodeyMessage(int num,decimal cost,string LastName, string FirstName, List<string> li)
        {
            string message ="Dear, "+ LastName +" "+FirstName+
                            "\nYour order №"+num+" has been created." +
                             "\nWe are waiting for you at the nearest Guitar Lab workshop. " +
                             "\nYou can pay for the order and discuss the details at the reception." +
                             "\nThe cost of the services you have selected is: "+cost+"$. " +
                            "\nBest regards, Guitar Lab administration\n \n \n";

            foreach (var s in li)
            {
                message += "\n" + "• " + s;
            }

            return message;
        }

        public void Send(string emailFrom, string password, string emailTo, string subject, string body,string smtpAddress)
        {
            int port = 587; // порт smtp сервера, в случае mail.ru это 587
            bool enableSSL = true;

            MailMessage mail = new MailMessage();

            mail.From = new MailAddress(emailFrom);
            mail.To.Add(emailTo);
            mail.Subject = subject;
            mail.Body = body;
            mail.IsBodyHtml = false; //можно поставить false, если отправляется только текст

            using (SmtpClient smtp = new SmtpClient(smtpAddress, port))
            {
                smtp.Credentials = new NetworkCredential(emailFrom, password);
                smtp.EnableSsl = enableSSL;
                try
                {
                    smtp.Send(mail); // отправка сообщения
                }
                catch (Exception ex)
                {
                    using (StreamWriter sw = new StreamWriter(@"C:\Users\Рабочий дух\source\repos\GuitLab\GuitLab\Log\WebLog.txt", true))
                    {
                        sw.WriteLine("----ERROR AT "+ DateTime.Now + ": " + ex);
                    }
                }
            }
        }
        
    }
}