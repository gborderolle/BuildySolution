﻿namespace BuildyBackend.EmailService
{
    public class EmailConfiguration
    {
        public string To { get; set; }
        public string From { get; set; }
        public string Subject { get; set; }
        public string SmtpServer { get; set; }
        public int SmtpPort { get; set; }
        public string UserName { get; set; }
        public string Password { get; set; }
    }
}
