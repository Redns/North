using MailKit.Net.Smtp;
using MimeKit;
using North.Core.Models.Notification;
using static MudBlazor.CategoryTypes;

namespace North.Core.Services.Poster
{
    /// <summary>
    /// 基于 MineKit 的邮件发送服务
    /// </summary>
    public class MineKitPoster : IPoster
    {
        /// <summary>
        /// 发送邮件
        /// </summary>
        /// <param name="email">待发送的邮件</param>
        public void Send(MailModel email)
        {
            // 创建邮件消息
            var bodyBuilder = email.IsBodyHtml ? new BodyBuilder { HtmlBody = email.Body } : new BodyBuilder { TextBody = email.Body };
            var message = new MimeMessage()
            {
                Subject = email.Title,
                Body = bodyBuilder.ToMessageBody()
            };
            message.From.Add(new MailboxAddress(email.From.Name, email.From.Address));
            message.To.AddRange(Array.ConvertAll(email.To.ToArray(), (email) => new MailboxAddress(email.Name, email.Address)));

            // 发送邮件
            using var client = new SmtpClient()
            {
                ServerCertificateValidationCallback = (s, c, h, e) => true,
            };
            client.Connect(email.Host, 465, true);
            client.Authenticate(email.From.Address, email.Code);
            client.Send(message);
            client.Disconnect(true);
        }


        /// <summary>
        /// 发送邮件的异步版本
        /// </summary>
        /// <param name="email">待发送的邮件</param>
        /// <returns></returns>
        public async Task SendAsync(MailModel email)
        {
            // 创建邮件消息
            var bodyBuilder = email.IsBodyHtml ? new BodyBuilder { HtmlBody = email.Body } : new BodyBuilder { TextBody = email.Body };
            var message = new MimeMessage()
            {
                Subject = email.Title,
                Body = bodyBuilder.ToMessageBody()
            };
            message.From.Add(new MailboxAddress(email.From.Name, email.From.Address));
            message.To.AddRange(Array.ConvertAll(email.To.ToArray(), (email) => new MailboxAddress(email.Name, email.Address)));

            // 发送邮件
            using var client = new SmtpClient()
            {
                ServerCertificateValidationCallback = (s, c, h, e) => true,
            };
            await client.ConnectAsync(email.Host, 465, true);
            await client.AuthenticateAsync(email.From.Address, email.Code);
            await client.SendAsync(message);
            await client.DisconnectAsync(true);
        }
    }
}
