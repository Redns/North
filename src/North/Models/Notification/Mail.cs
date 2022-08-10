using MimeKit;
using MailKit.Net.Smtp;

namespace North.Models.Notification
{
    public class Mail
    {
        /// <summary>
        /// 发送人
        /// </summary>
        public MailboxAddress From { get; set; }

        /// <summary>
        /// 收件人地址(多人)
        /// </summary>
        public IEnumerable<MailboxAddress> To { get; set; }

        /// <summary>
        /// 标题
        /// </summary>
        public string Title { get; set; }

        /// <summary>
        /// 正文
        /// </summary>
        public string Body { get; set; }

        /// <summary>
        /// 授权码
        /// </summary>
        public string Code { get; set; }

        /// <summary>
        /// SMTP 邮件服务器
        /// </summary>
        public string Host => $"smtp.{From.Address.Split(new char[] { '@', '.' })[1]}.com";

        /// <summary>
        /// 正文是否是 html 格式
        /// </summary>
        public bool IsBodyHtml { get; set; }

        public Mail(MailboxAddress from, IEnumerable<MailboxAddress> to, string title, string body, string code, bool isBodyHtml)
        {
            From = from;
            To = to;
            Title = title;
            Body = body;
            Code = code;
            IsBodyHtml = isBodyHtml;
        }

        public Mail(MailboxAddress from, MailboxAddress to, string title, string body, string code, bool isBodyHtml)
        {
            From = from;
            To = new MailboxAddress[] { to };
            Title = title;
            Body = body;
            Code = code;
            IsBodyHtml = isBodyHtml;
        }


        /// <summary>
        /// 发送邮件
        /// </summary>
        /// <param name="mail">待发送的邮件</param>
        public void Send()
        {
            // 创建邮件消息
            var bodyBuilder = IsBodyHtml ? new BodyBuilder { HtmlBody = Body } : new BodyBuilder { TextBody = Body };
            var message = new MimeMessage()
            {
                Subject = Title,
                Body = bodyBuilder.ToMessageBody()
            };
            message.From.Add(From);
            message.To.AddRange(To);

            // 发送邮件
            using var client = new SmtpClient()
            {
                ServerCertificateValidationCallback = (s, c, h, e) => true
            };
            client.Connect(Host, 465, true);
            client.Authenticate(From.Address, Code);
            client.Send(message);
            client.Disconnect(true);
        }


        /// <summary>
        /// 发送邮件的异步版本
        /// </summary>
        /// <param name="mail">待发送的邮件</param>
        /// <returns></returns>
        public async Task SendAsync()
        {
            // 创建邮件消息
            var bodyBuilder = IsBodyHtml ? new BodyBuilder { HtmlBody = Body } : new BodyBuilder { TextBody = Body };
            var message = new MimeMessage()
            {
                Subject = Title,
                Body = bodyBuilder.ToMessageBody()
            };
            message.From.Add(From);
            message.To.AddRange(To);

            // 发送邮件
            using var client = new SmtpClient()
            {
                ServerCertificateValidationCallback = (s, c, h, e) => true,
            };
            await client.ConnectAsync(Host, 465, true);
            await client.AuthenticateAsync(From.Address, Code);
            await client.SendAsync(message);
            await client.DisconnectAsync(true);
        }
    }
}
