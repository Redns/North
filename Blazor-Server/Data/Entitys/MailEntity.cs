namespace ImageBed.Data.Entity
{
    public class MailEntity
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
        /// <summary>
        /// 接收文件
        /// </summary>
        public List<IFormFile> Files { get; set; }

        public MailEntity(string fromPerson, string[] recipientArry, string mailTitle, string mailBody, string code, string host, bool isBodyHtml, List<IFormFile> files)
        {
            FromPerson = fromPerson;
            RecipientArry = recipientArry;
            MailTitle = mailTitle;
            MailBody = mailBody;
            Code = code;
            Host = host;
            IsBodyHtml = isBodyHtml;
            Files = files;
        }
    }
}
