using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Pomelo.Net.Smtp;

namespace Microsoft.Extensions.DependencyInjection
{
    public static class SmtpEmailSenderExtensions
    {
        public static IServiceCollection AddSmtpEmailSender(this IServiceCollection self, string server, int port, string senderName, string email, string username, string password, bool ssl = false)
        {
            return self.AddSingleton<IEmailSender>(x => new SmtpEmailSender(server, port, senderName, email, username, password, ssl));
        }
    }
}
