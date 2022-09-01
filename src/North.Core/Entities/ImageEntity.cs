using North.Core.Helpers;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace North.Core.Entities
{
    public class ImageEntity
    {
        [MaxLength(32)]
        public string Id { get; set; }

        /// <summary>
        /// 文件名
        /// </summary>
        [MaxLength(64)]
        public string Name { get; set; }

        /// <summary>
        /// 尺寸
        /// </summary>
        public ImageSize Size { get; set; }

        /// <summary>
        /// 所有者
        /// </summary>
        public Owner Owner { get; set; }

        /// <summary>
        /// 上传时间
        /// </summary>
        [MaxLength(32)]
        public string UploadTime { get; set; }

        /// <summary>
        /// 存储方 ID
        /// </summary>
        public string Storager { get; set; }

        /// <summary>
        /// 图片链接
        /// </summary>
        public ImageUrl Url { get; set; }

        /// <summary>
        /// 请求次数
        /// </summary>
        public long Request { get; set; }

        public ImageEntity(string name, ImageSize size, Owner owner, string storager, ImageUrl url, string? id = null, long request = 0L)
        {
            Id = id ?? IdentifyHelper.Generate();
            Name = name;
            Size = size;
            Owner = owner;
            UploadTime = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
            Storager = storager;
            Url = url;
            Request = request;
        }
    }


    /// <summary>
    /// 图片尺寸
    /// </summary>
    public struct ImageSize
    {
        [Column("Height")]
        public int Height { get; set; }

        [Column("Width")]
        public int Width { get; set; }

        [Column("Length")]
        public long Length { get; set; }

        public ImageSize(int height, int width, long length)
        {
            Height = height;
            Width = width;
            Length = length;
        }
    }


    /// <summary>
    /// 图片所有者
    /// </summary>
    public struct Owner
    {
        [MaxLength(32)]
        [Column("OwnerId")]
        public string Id { get; set; }

        [MaxLength(32)]
        [Column("OwnerName")]
        public string Name { get; set; }

        [MaxLength(32)]
        [Column("OwnerEmail")]
        public string Email { get; set; }

        public Owner(string id, string name, string email)
        {
            Id = id;
            Name = name;
            Email = email;
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
        [MaxLength(256)]
        [Column("Url_Source")]
        public string Source { get; set; }

        /// <summary>
        /// 图片缩略图链接
        /// </summary>
        [MaxLength(256)]
        [Column("Url_Thumbnail")]
        public string Thumbnail { get; set; }

        public ImageUrl(string source, string thumbnail)
        {
            Source = source;
            Thumbnail = thumbnail;
        }
    }
}