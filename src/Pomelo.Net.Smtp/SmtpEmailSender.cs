using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Net;
using System.Net.Sockets;
using System.Net.Security;
using System.IO;
using Microsoft.AspNetCore.StaticFiles;

namespace Pomelo.Net.Smtp
{
    public class SmtpEmailSender : EmailSender
    {
        public SmtpEmailSender(IContentTypeProvider ctp, string Server, int Port, string SenderName, string SenderEmail, string UserName, string Password, bool SSL = false)
        {
            server = Server;
            port = Port;
            my_name = SenderName;
            my_email = SenderEmail;
            pwd = Password;
            ssl = SSL;
            username = UserName;
            mime = ctp;
        }

        private IContentTypeProvider mime;
        private string username;
        private bool ssl;
        private string server;
        private int port;
        private string my_name;
        private string my_email;
        private string pwd;
        private string boundary = "--------------=Pomelo_" + DateTime.Now.ToTimeStamp();

        public override Task SendEmailAsync(string[] to, string[] cc, string[] bcc, string subject, string message, params Attachment[] Attachments)
        {
            if (!ssl)
                return Task.Factory.StartNew(async () =>
                {
                    using (var client = new TcpClient())
                    {
                        await client.ConnectAsync(server, port);
                        using (var stream = client.GetStream())
                        using (var reader = new StreamReader(stream))
                        using (var writer = new StreamWriter(stream) { AutoFlush = true, NewLine = "\r\n" })
                        {
                            TcpWrite(writer, reader, to, cc, bcc, subject, message, Attachments);
                        }
                    }
                });
            else
                return Task.Factory.StartNew(async () =>
                {
                    using (var client = new TcpClient())
                    {
                        await client.ConnectAsync(server, port);
                        using (var stream = new SslStream(client.GetStream(), false))
                        {
                            await stream.AuthenticateAsClientAsync(server);
                            using (var reader = new StreamReader(stream))
                            using (var writer = new StreamWriter(stream) { AutoFlush = true, NewLine = "\r\n" })
                            {
                                TcpWrite(writer, reader, to, cc, bcc, subject, message, Attachments);
                            }
                        }
                    }
                });
        }

        private string GetName(string email)
        {
            return email.Split('@')[0];
        }

        private string GetAddress(string email)
        {
            return $"\"{GetName(email)}\"<{email}>";
        }

        private string GetAddresses(string[] emails)
        {
            return string.Join(", ", emails.Select(x => GetAddress(x)));
        }

        private void TcpWrite(StreamWriter writer, StreamReader reader, string[] to, string[] cc, string[] bcc, string subject, string message, params Attachment[] Attachments)
        {
            Console.WriteLine(reader.ReadLine());
            writer.WriteLine("HELO " + server);
            Console.WriteLine(reader.ReadLine());
            writer.WriteLine("AUTH LOGIN");
            Console.WriteLine(reader.ReadLine());
            var plainTextBytes1 = System.Text.Encoding.UTF8.GetBytes(username);
            string base64Username = System.Convert.ToBase64String(plainTextBytes1);
            writer.WriteLine(base64Username);
            Console.WriteLine(reader.ReadLine());
            string password = pwd;
            var plainTextBytes2 = System.Text.Encoding.UTF8.GetBytes(password);
            string base64Password = System.Convert.ToBase64String(plainTextBytes2);
            writer.WriteLine(base64Password);
            Console.WriteLine(reader.ReadLine());
            writer.WriteLine($"MAIL FROM:<{my_email}>");
            Console.WriteLine(reader.ReadLine());
            var tmp = to.ToList();
            tmp.AddRange(cc);
            tmp.AddRange(bcc);
            foreach(var x in tmp)
            {
                writer.WriteLine($"RCPT TO:<{x}>");
                Console.WriteLine(reader.ReadLine());
            }
            writer.WriteLine("DATA");
            Console.WriteLine(reader.ReadLine());
            writer.WriteLine($"BodyFormat: 0");
            writer.WriteLine($"MailFormat: 0");
            writer.WriteLine($"From: \"{my_name}\" <{my_email}>");
            if (to.Count() > 0)
                writer.WriteLine($"To: " + GetAddresses(to));
            if (cc.Count() > 0)
                writer.WriteLine($"Cc: " + GetAddresses(cc));
            if (bcc.Count() > 0)
                writer.WriteLine($"Bcc: " + GetAddresses(bcc));
            writer.WriteLine($"Subject: {subject}");
            writer.WriteLine("MIME-Version: 1.0; ");

            // Sending plain html
            writer.WriteLine($"Content-Type: multipart/mixed; charset=\"utf8\"; boundary=\"{boundary}_EMAIL\"");
            writer.WriteLine("");
            writer.WriteLine($"--{boundary}_EMAIL");
            writer.WriteLine($"Content-Type: multipart/alternative; charset=\"utf8\"; boundary=\"{boundary}_CONTENT\"");
            writer.WriteLine("");

            writer.WriteLine($"--{boundary}_CONTENT");
            writer.WriteLine("Content-Transfer-Encoding: base64;");
            writer.WriteLine("Content-Type: text/plain; charset=\"utf8\";");
            writer.WriteLine("");
            writer.WriteLine(Convert.ToBase64String(System.Text.Encoding.UTF8.GetBytes(HtmlHelper.NoHTML(message))));
            writer.WriteLine("");

            writer.WriteLine($"--{boundary}_CONTENT");
            writer.WriteLine("Content-Transfer-Encoding: base64;");
            writer.WriteLine("Content-Type: text/html; charset=\"utf8\";");
            writer.WriteLine("");
            writer.WriteLine(Convert.ToBase64String(System.Text.Encoding.UTF8.GetBytes(message)));
            writer.WriteLine("");

            writer.WriteLine($"--{boundary}_CONTENT--");
            writer.WriteLine($"");


            // Sending attachments
            foreach (var x in Attachments)
            {
                string contentType;
                if (!mime.TryGetContentType(x.FileName, out contentType))
                {
                    contentType = "application/octet-stream";
                }
                writer.WriteLine($"--{boundary}_EMAIL");
                writer.WriteLine($"Content-Type:{contentType}; name=\"{x.FileName}\"");
                writer.WriteLine($"Content-Disposition: attachment; filename={x.FileName}");
                writer.WriteLine($"Content-Transfer-Encoding: base64");
                writer.WriteLine($"");
                writer.WriteLine($"{ Convert.ToBase64String(x.File) }");
                writer.WriteLine($"");
            }

            writer.WriteLine($"--{boundary}_EMAIL--");

            writer.WriteLine(".");
            Console.WriteLine(reader.ReadLine());

            writer.WriteLine("QUIT");
            Console.WriteLine(reader.ReadLine());
        }
    }
}
