using Microsoft.AspNetCore.StaticFiles;
using Pomelo.Net.Smtp;

namespace Microsoft.Extensions.DependencyInjection
{
    public static class SmtpEmailSenderExtensions
    {
        public static IServiceCollection AddSmtpEmailSender(this IServiceCollection self, string server, int port, string senderName, string email, string username, string password, bool ssl = false)
        {
            return self.AddSingleton<IContentTypeProvider, FileExtensionContentTypeProvider>()
                .AddSingleton<IEmailSender>(x => new SmtpEmailSender(x.GetRequiredService<IContentTypeProvider>() ,server, port, senderName, email, username, password, ssl));
        }
    }
}
