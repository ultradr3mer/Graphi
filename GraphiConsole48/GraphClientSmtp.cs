using GraphiConsole48.MailKit;
using MailKit.Net.Smtp;
using MailKit.Security;
using MimeKit;

namespace Graphi
{
  internal class GraphClientSmtp : GraphClient
  {
    public void SendSmtp(string token)
    {
      var message = new MimeMessage();
      message.From.Add(new MailboxAddress(settings.Sender, settings.Sender));
      message.To.Add(new MailboxAddress(settings.Recipient, settings.Recipient));
      message.Subject = "How you doin'?";

      message.Body = new TextPart("plain")
      {
        Text = @"Hey Chandler,

I just wanted to let you know that Monica and I were going to go play some paintball, you in?+

-- Joey"
      };

      var oauth2 = new SaslMechanismOAuth2(settings.Sender, token);

      using (var client = new SmtpClient())
      {
        client.Connect("smtp.office365.com", 587, SecureSocketOptions.StartTls);
        client.Authenticate(oauth2);
        client.Send(message);
        client.Disconnect(true);
      }
    }
  }
}