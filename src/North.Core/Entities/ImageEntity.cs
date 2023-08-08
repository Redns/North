﻿using SqlSugar;

namespace North.Core.Entities
{
    [SugarTable("Images")]
    public class ImageEntity : Entity
    {
        /// <summary>
        /// 图片名
        /// </summary>
        [SugarColumn(Length = 64)]
        public string Name { get; set; } = string.Empty;

        /// <summary>
        /// 图片高度
        /// </summary>
        public int Height { get; set; } = 0;

        /// <summary>
        /// 图片宽度
        /// </summary>
        public int Width { get; set; } = 0;

        /// <summary>
        /// 图片大小
        /// </summary>
        public long Length { get; set; } = 0L;

        /// <summary>
        /// 上传时间
        /// </summary>
        public DateTime UploadTime { get; set; } = DateTime.Now;

        /// <summary>
        /// 图片链接
        /// </summary>
        [SugarColumn(Length = 256)]
        public string SourceUrl { get; set; } = string.Empty;

        /// <summary>
        /// 图片缩略图链接
        /// </summary>
        [SugarColumn(Length = 256)]
        public string ThumbnailUrl { get; set; } = string.Empty;

        /// <summary>
        /// 请求次数
        /// </summary>
        public int Request { get; set; } = 0;

        /// <summary>
        /// 访问权限
        /// </summary>
        public ImageAccessPermission AccessPermission { get; set; }

        /// <summary>
        /// 关联用户ID
        /// </summary>
        public Guid UserId { get; set; }

        #region 导航属性
        /// <summary>
        /// 图片下载历史
        /// </summary>
        [Navigate(NavigateType.OneToMany, nameof(ImageDownloadHistoryEntity.ImageId))]
        public List<ImageDownloadHistoryEntity> DownloadHistories { get; set; }
        #endregion
    }


    /// <summary>
    /// 图片访问权限
    /// </summary>
    public enum ImageAccessPermission
    {
        Public = 0,     // 公开（所有人可见）
        LoggedInUser,   // 已登录用户可见
        Private         // 私密（仅本人可见）
    }
}