using MimeKit;

namespace BuildyBackend.EmailService
{
    public class Message
    {
        public List<MailboxAddress> To { get; set; }
        public string Subject { get; set; }
        public string Content { get; set; }

        public Message(IEnumerable<string> destinationList, string subject, string content)
        {
            To = destinationList.Select(destination => new MailboxAddress(destination, destination)).ToList();
            Subject = subject;
            Content = content;
        }

    }
}
