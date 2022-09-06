using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace North.Core.Entities
{
    public class ImageEntity
    {
        [Required]
        public Guid Id { get; set; } = Guid.NewGuid();

        /// <summary>
        /// 文件名
        /// </summary>
        [Required]
        [MaxLength(64)]
        public string Name { get; set; } = string.Empty;

        /// <summary>
        /// 图片高度
        /// </summary>
        [Required]
        public int Height { get; set; } = 0;

        /// <summary>
        /// 图片宽度
        /// </summary>
        [Required]
        public int Width { get; set; } = 0;

        /// <summary>
        /// 图片大小
        /// </summary>
        [Required]
        public long Length { get; set; } = 0L;

        /// <summary>
        /// 上传时间
        /// </summary>
        [Required]
        public DateTime UploadTime { get; set; } = DateTime.Now;

        /// <summary>
        /// 存储方 ID
        /// </summary>
        [Required]
        [MaxLength(64)]
        public string Storager { get; set; } = string.Empty;

        /// <summary>
        /// 图片链接
        /// </summary>
        [Required]
        [MaxLength(256)]
        public string SourceUrl { get; set; } = string.Empty;

        /// <summary>
        /// 图片缩略图链接
        /// </summary>
        [Required]
        [MaxLength(256)]
        public string ThumbnailUrl { get; set; } = string.Empty;

        /// <summary>
        /// 请求次数
        /// </summary>
        [Required]
        public long Request { get; set; } = 0;

        /// <summary>
        /// 所有者
        /// </summary>
        public UserEntity Owner { get; set; }
    }


    /// <summary>
    /// 图片尺寸
    /// </summary>
    // TODO 增加图片编码等信息，更新为 ImageInfo
    public struct ImageSize
    {
        /// <summary>
        /// 图片高度
        /// </summary>
        public int Height { get; set; }

        /// <summary>
        /// 图片宽度
        /// </summary>
        public int Width { get; set; }

        public ImageSize(int height, int width)
        {
            Height = height;
            Width = width;
        }
    }


    /// <summary>
    /// 图片链接
    /// </summary>
    public struct ImageUrl
    {
        /// <summary>
        /// 图片链接
        /// </summary>
        public string Source { get; set; }

        /// <summary>
        /// 图片缩略图链接
        /// </summary>
        public string Thumbnail { get; set; }

        public ImageUrl(string source, string thumbnail)
        {
            Source = source;
            Thumbnail = thumbnail;
        }
    }
}