using SqlSugar;

namespace North.Core.Entities
{
    public class ImageDownloadHistoryEntity : Entity
    {
        /// <summary>
        /// 下载用户 ID
        /// </summary>
        public Guid DownloadUserId { get; set; } = Guid.Empty;

        /// <summary>
        /// 登录地址
        /// </summary>
        [SugarColumn(Length = 16)]
        public string IPAddress { get; set; } = string.Empty;

        /// <summary>
        /// 登录时间
        /// </summary>
        public DateTime Time { get; set; } = DateTime.Now;

        /// <summary>
        /// 关联图片 ID
        /// </summary>
        public Guid ImageId { get; set; }
    }
}
