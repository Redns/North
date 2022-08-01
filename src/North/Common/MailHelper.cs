using System.Net;
using System.Net.Mail;

namespace North.Common
{
    public class MailHelper
    {
        /// <summary>
        /// 发送邮件
        /// </summary>
        /// <param name="mail">待发送的邮件</param>
        /// <returns></returns>
        public static async Task PostEmails(Mail mail)
        {
            // 设置 SMTP
            mail.Host = $"smtp.{mail.FromPerson.Split(new char[] { '@', '.' })[1]}.com";

            // 初始化 MailAddress、Message
            MailAddress mailAddress = new(mail.FromPerson);
            MailMessage mailMessage = new()
            {
                From = mailAddress,
                Subject = mail.MailTitle,
                SubjectEncoding = System.Text.Encoding.UTF8,
                Body = mail.MailBody,
                BodyEncoding = System.Text.Encoding.Default,
                Priority = MailPriority.High,
                IsBodyHtml = mail.IsBodyHtml,
            };

            // 判断是否有收件人
            if (mail.RecipientArry.Any())
            {
                foreach (var recipient in mail.RecipientArry)
                {
                    if (!string.IsNullOrEmpty(recipient))
                    {
                        mailMessage.To.Add(recipient);
                    }
                }
            }

            // 实例化 SMTP 客户端
            SmtpClient smtp = new()
            {
                Credentials = new NetworkCredential(mail.FromPerson, mail.Code),
                Host = mail.Host
            };
            await smtp.SendMailAsync(mailMessage);
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
        public string FromPerson { get; set; }

        /// <summary>
        /// 收件人地址(多人)
        /// </summary>
        public string[] RecipientArry { get; set; }

        /// <summary>
        /// 标题
        /// </summary>
        public string MailTitle { get; set; }

        /// <summary>
        /// 正文
        /// </summary>
        public string MailBody { get; set; }

        /// <summary>
        /// 客户端授权码(可存在配置文件中)
        /// </summary>
        public string Code { get; set; }

        /// <summary>
        /// SMTP邮件服务器
        /// </summary>
        public string Host { get; set; }

        /// <summary>
        /// 正文是否是html格式
        /// </summary>
        public bool IsBodyHtml { get; set; }

        public Mail(string fromPerson, string[] recipientArry, string mailTitle, string mailBody, string code, string host, bool isBodyHtml)
        {
            FromPerson = fromPerson;
            RecipientArry = recipientArry;
            MailTitle = mailTitle;
            MailBody = mailBody;
            Code = code;
            Host = host;
            IsBodyHtml = isBodyHtml;
        }
    }
}