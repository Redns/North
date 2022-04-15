using ImageBed.Data.Entity;
using System.Net;
using System.Net.Mail;

namespace ImageBed.Common
{
    public class MailHelper
    {
        /// <summary>
        /// 发送邮件
        /// </summary>
        /// <param name="email">待发送的邮件</param>
        /// <returns></returns>
        public static async Task PostEmails(MailEntity mail)
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
                foreach(var recipient in mail.RecipientArry)
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
}
