using System.Net;
using System.Net.Mail;

namespace ShopWeb.Models.Sevices
{
    public interface ISendEmail
    {
        void sendEmail(string To, string Message, string subject);
    }

    public class SendEmail : ISendEmail
    {
        public void sendEmail(string To, string Message, string subject)
        {
            MailMessage mail = new MailMessage();

            mail.From = new MailAddress("JafariAmirhossein15@gmail.com");
            mail.To.Add(new MailAddress(To));
            mail.Body = Message;
            mail.Subject = subject;
            mail.IsBodyHtml = false;

            SmtpClient smtp = new SmtpClient();

            smtp.Host = "smtp.gmail.com";
            smtp.Port = 587;
            smtp.EnableSsl = true;

            smtp.UseDefaultCredentials = false;
            smtp.Credentials = new NetworkCredential("JafariAmirhossein15@gmail.com", "*************");
            smtp.DeliveryMethod = SmtpDeliveryMethod.Network;

            smtp.Send(mail);
        }
    }
}
