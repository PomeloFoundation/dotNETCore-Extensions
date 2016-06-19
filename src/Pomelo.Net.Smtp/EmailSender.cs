using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Pomelo.Net.Smtp
{
    public abstract class EmailSender : IEmailSender
    {
        public virtual Task SendEmailAsync(string to, string cc, string bcc, string subject, string message, params Attachment[] Attachments)
        {
            var _to = new string[0];
            var _cc = new string[0];
            var _bcc = new string[0];
            if (!string.IsNullOrEmpty(to))
                _to = to.Split(',').Select(x => x.Trim()).ToArray();
            if (!string.IsNullOrEmpty(cc))
                _cc = cc.Split(',').Select(x => x.Trim()).ToArray();
            if (!string.IsNullOrEmpty(bcc))
                _bcc = bcc.Split(',').Select(x => x.Trim()).ToArray();
            return SendEmailAsync(_to, _cc, _bcc, subject, message, Attachments);
        }

        public abstract Task SendEmailAsync(string[] to, string[] cc, string[] bcc, string subject, string message, params Attachment[] Attachments);
    }
}
