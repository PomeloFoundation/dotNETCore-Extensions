using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Net;
using System.Net.Sockets;
using System.Net.Security;
using System.IO;

namespace Pomelo.Net.Smtp
{
    public class SmtpEmailSender : IEmailSender
    {
        public SmtpEmailSender(string Server, int Port, string SenderName, string SenderEmail, string UserName, string Password, bool SSL = false)
        {
            server = Server;
            port = Port;
            my_name = SenderName;
            my_email = SenderEmail;
            pwd = Password;
            ssl = SSL;
            username = UserName;
        }
        private string username;
        private bool ssl;
        private string server;
        private int port;
        private string my_name;
        private string my_email;
        private string pwd;

        public Task SendEmailAsync(string email, string subject, string message)
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

                            writer.WriteLine($"RCPT TO:<{email}>");
                            Console.WriteLine(reader.ReadLine());

                            writer.WriteLine("DATA");
                            Console.WriteLine(reader.ReadLine());

                            writer.WriteLine($"BodyFormat: 0");
                            writer.WriteLine($"MailFormat: 0");
                            writer.WriteLine($"From: \"{my_name}\" <{my_email}>");
                            writer.WriteLine($"To: \"Somebody\" <{email}>");
                            writer.WriteLine($"Subject: {subject}");
                            writer.WriteLine("Mime-Version: 1.0; ");
                            writer.WriteLine("Content-Type: text/html; charset=\"utf8\";");
                            writer.WriteLine("Content-Transfer-Encoding: 8bit;");
                            // Put one blank line after the Subject line, then start the message body.
                            writer.WriteLine("");
                            // Start the message body here
                            writer.WriteLine(message);
                            // Send a period to denote the end of the message body
                            writer.WriteLine(".");
                            Console.WriteLine(reader.ReadLine());

                            writer.WriteLine("QUIT");
                            Console.WriteLine(reader.ReadLine());
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

                                writer.WriteLine($"RCPT TO:<{email}>");
                                Console.WriteLine(reader.ReadLine());

                                writer.WriteLine("DATA");
                                Console.WriteLine(reader.ReadLine());

                                writer.WriteLine($"BodyFormat: 0");
                                writer.WriteLine($"MailFormat: 0");
                                writer.WriteLine($"From: \"{my_name}\" <{my_email}>");
                                writer.WriteLine($"To: \"MESSAGE_TO_NAME\" <{email}>");
                                writer.WriteLine($"Subject: {subject}");
                                writer.WriteLine("Mime-Version: 1.0; ");
                                writer.WriteLine("Content-Type: text/html; charset=\"utf8\";");
                                writer.WriteLine("Content-Transfer-Encoding: 8bit;");
                                // Leave one blank line after the subject
                                writer.WriteLine("");
                                // Start the message body here
                                writer.WriteLine(message);
                                // End the message body by sending a period
                                writer.WriteLine(".");
                                Console.WriteLine(reader.ReadLine());

                                writer.WriteLine("QUIT");
                                Console.WriteLine(reader.ReadLine());
                            }
                        }
                    }
                });
        }
    }
}
