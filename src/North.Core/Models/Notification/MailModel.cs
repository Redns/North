namespace North.Core.Models.Notification;

public class MailModel
{
    /// <summary>
    /// 发送人
    /// </summary>
    public MailAddress From { get; set; }

    /// <summary>
    /// 收件人地址(多人)
    /// </summary>
    public IEnumerable<MailAddress> To { get; set; }

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

    public MailModel(MailAddress from, IEnumerable<MailAddress> to, string title, string body, string code, bool isBodyHtml = true)
    {
        From = from;
        To = to;
        Title = title;
        Body = body;
        Code = code;
        IsBodyHtml = isBodyHtml;
    }

    public MailModel(MailAddress from, MailAddress to, string title, string body, string code, bool isBodyHtml = true)
    {
        From = from;
        To = new MailAddress[] { to };
        Title = title;
        Body = body;
        Code = code;
        IsBodyHtml = isBodyHtml;
    }
}


/// <summary>
/// 邮箱地址
/// </summary>
public class MailAddress
{
    /// <summary>
    /// 用户名
    /// </summary>
    public string Name { get; set; }

    /// <summary>
    /// 邮箱地址
    /// </summary>
    public string Address { get; set; }

    public MailAddress(string name, string address)
    {
        Name = name;
        Address = address;
    }
}