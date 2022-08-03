using MimeKit;
using MailKit.Net.Smtp;

namespace North.Common
{
    public class MailHelper
    {
        /// <summary>
        /// 发送邮件
        /// </summary>
        /// <param name="mail">待发送的邮件</param>
        public static void Send(Mail mail)
        {
            // 创建邮件消息
            var bodyBuilder = mail.IsBodyHtml ? new BodyBuilder { HtmlBody = mail.Body } : new BodyBuilder { TextBody = mail.Body };
            var message = new MimeMessage()
            {
                Subject = mail.Title,
                Body = bodyBuilder.ToMessageBody()
            };
            message.From.Add(mail.From);
            message.To.AddRange(mail.To);

            // 发送邮件
            using var client = new SmtpClient()
            {
                ServerCertificateValidationCallback = (s, c, h, e) => true
            };
            client.Connect(mail.Host, 465, true);
            client.Authenticate(mail.From.Address, mail.Code);
            client.Send(message);
            client.Disconnect(true);
        }


        /// <summary>
        /// 发送邮件的异步版本
        /// </summary>
        /// <param name="mail">待发送的邮件</param>
        /// <returns></returns>
        public static async Task SendAsync(Mail mail)
        {
            // 创建邮件消息
            var bodyBuilder = mail.IsBodyHtml ? new BodyBuilder { HtmlBody = mail.Body } : new BodyBuilder { TextBody = mail.Body };
            var message = new MimeMessage()
            {
                Subject = mail.Title,
                Body = bodyBuilder.ToMessageBody()
            };
            message.From.Add(mail.From);
            message.To.AddRange(mail.To);

            // 发送邮件
            using var client = new SmtpClient()
            {
                ServerCertificateValidationCallback = (s, c, h, e) => true,
            };
            await client.ConnectAsync(mail.Host, 465, true);
            await client.AuthenticateAsync(mail.From.Address, mail.Code);
            await client.SendAsync(message);
            await client.DisconnectAsync(true);
        }
    }


    /// <summary>
    /// 电子邮件
    /// </summary>
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
        public string Host  => $"smtp.{From.Address.Split(new char[] { '@', '.' })[1]}.com";

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
    }
}