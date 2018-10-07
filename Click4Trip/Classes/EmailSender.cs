using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Mail;
using System.Web;

namespace Click4Trip.Classes
{
    public class EmailSender
    {
        public SmtpClient client { get; set; }

        public MailMessage mailMessage { get; set; }

        public EmailSender()
        {
            client = new SmtpClient()
            {
                Host = "Smtp.Gmail.com",
                Port = 587,
                EnableSsl = true,
                Timeout = 10000,
                DeliveryMethod = SmtpDeliveryMethod.Network,
                UseDefaultCredentials = false,
                Credentials = new NetworkCredential("Click4TripService@gmail.com", "ClickTheTrip")
            };

            mailMessage = new MailMessage();
            mailMessage.IsBodyHtml = true;
            mailMessage.From = new System.Net.Mail.MailAddress("Click4TripService@gmail.com");
            mailMessage.Subject = "Click4Trip- Order receipt";
        }

        public bool SendMail(string To, string Body)
        {
            mailMessage.To.Add(To);         
            mailMessage.Body = Body;

            try
            {
                client.Send(mailMessage);
                return true;
            }

            catch
            {
                Console.WriteLine("Mail failed");
                return false;
            }    
        }
    }
}