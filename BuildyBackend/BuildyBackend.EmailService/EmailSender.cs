using MailKit.Net.Smtp;
using MimeKit;

namespace BuildyBackend.EmailService
{
    public class EmailSender : IEmailSender
    {
        private readonly EmailConfiguration _emailConfig;
        public EmailSender(EmailConfiguration emailConfig)
        {
            _emailConfig = emailConfig;
        }
        public void SendEmail(Message message)
        {
            var emailMessage = CreateEmailMessage(message);
            Send(emailMessage);
        }
        private MimeMessage CreateEmailMessage(Message message)
        {
            var emailMessage = new MimeMessage();
            emailMessage.From.Add(new MailboxAddress("email", _emailConfig.From));
            emailMessage.To.AddRange(message.To);
            emailMessage.Subject = message.Subject;
            emailMessage.Body = new TextPart(MimeKit.Text.TextFormat.Html) { Text = message.Content };
            //emailMessage.Body = new TextPart(MimeKit.Text.TextFormat.Html) { Text = string.Format("<h2 style='color:red;'>{0}</h2>", message.Content) };

            return emailMessage;
        }
        private void Send(MimeMessage mailMessage)
        {
            using (var client = new SmtpClient())
            {
                try
                {
                    client.Connect(_emailConfig.SmtpServer, _emailConfig.SmtpPort, true);
                    client.AuthenticationMechanisms.Remove("XOAUTH2");
                    client.Authenticate(_emailConfig.UserName, _emailConfig.Password);
                    client.Send(mailMessage);
                }
                catch
                {
                    //log an error message or throw an exception or both.
                    throw;
                }
                finally
                {
                    client.Disconnect(true);
                    //client.Dispose();
                }
            }
        }
        public async Task SendEmailAsync(Message message)
        {
            var mailMessage = CreateEmailMessage(message);
            using (var client = new SmtpClient())
            {
                try
                {
                    await client.ConnectAsync(_emailConfig.SmtpServer, _emailConfig.SmtpPort, true);
                    client.AuthenticationMechanisms.Remove("XOAUTH2");

                    // Obtener la contraseña desde una variable de entorno
                    var emailPassword = Environment.GetEnvironmentVariable("EMAIL_PASSWORD");

                    // Si la variable de entorno no está establecida, intenta obtenerla desde la configuración cargada
                    if (string.IsNullOrEmpty(emailPassword))
                    {
                        emailPassword = _emailConfig.Password;
                    }

                    // Asegurarse de que la contraseña no sea nula o vacía
                    if (string.IsNullOrEmpty(emailPassword))
                    {
                        throw new InvalidOperationException("No se encontró la contraseña del correo electrónico. Asegúrese de configurar la variable de entorno 'EMAIL_PASSWORD'.");
                    }

                    await client.AuthenticateAsync(_emailConfig.UserName, emailPassword);
                    await client.SendAsync(mailMessage);
                }
                catch
                {
                    // Log an error message or throw an exception, or both.
                    throw;
                }
                finally
                {
                    await client.DisconnectAsync(true);
                    client.Dispose();
                }
            }
        }


    }
}
