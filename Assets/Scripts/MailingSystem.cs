using System.Net.Mail;
using System.Net;

public static class MailingSystem {

    private static string subjectOfMail = "";
    private static string mailAddress = "bachelor2019@hotmail.com";
    private static string mailAddressPassword = "ThisIsAStupidPasswordForTheBachelorProject2019CreatedByTobias";
    private static string clientMail = "bias2402@hotmail.dk";

    public static void SendEmailToClient() {
        using (MailMessage mail = new MailMessage {
            From = new MailAddress(mailAddress),
            Subject = subjectOfMail,
            Body = "This mail was sent from the Turtle-Programming System."
        }) {
            mail.To.Add(clientMail);

            SmtpClient client = new SmtpClient("smtp.sdu.dk") {
                Port = 7525,
                DeliveryMethod = SmtpDeliveryMethod.Network,
                EnableSsl = true,
                UseDefaultCredentials = false,
                Credentials = (ICredentialsByHost)new NetworkCredential(mailAddress, mailAddressPassword)
            };

            ServicePointManager.ServerCertificateValidationCallback = delegate { return true; };
            client.Send(mail);
        }
    }
}