using North.Core.Models.Notification;

namespace North.Core.Services.Poster
{
    /// <summary>
    /// 电子邮件发送接口
    /// </summary>
    public interface IPoster
    {
        /// <summary>
        /// 发送邮件
        /// </summary>
        /// <param name="email">待发送的邮件</param>
        void Send(MailModel email);


        /// <summary>
        /// 发送邮件的异步版本
        /// </summary>
        /// <param name="email">待发送的邮件</param>
        /// <returns></returns>
        Task SendAsync(MailModel email);
    }
}
