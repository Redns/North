using North.Core.Helper;
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
        /// 存储方
        /// </summary>
        public Storager Storager { get; set; }

        /// <summary>
        /// 请求次数
        /// </summary>
        public long Request { get; set; }

        public ImageEntity(string name, ImageSize size, Owner owner, Storager storager)
        {
            Id = IdentifyHelper.Generate();
            Name = name;
            Size = size;
            Owner = owner;
            UploadTime = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
            Storager = storager;
            Request = 0;
        }

        public ImageEntity(string id, string name, ImageSize size, Owner owner, Storager storager)
        {
            Id = id;
            Name = name;
            Size = size;
            Owner = owner;
            UploadTime = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
            Storager = storager;
            Request = 0;
        }

        public ImageEntity(string id, string name, ImageSize size, Owner owner, string uploadTime, Storager storager, long request)
        {
            Id = id;
            Name = name;
            Size = size;
            Owner = owner;
            UploadTime = uploadTime;
            Storager = storager;
            Request = request;
        }
    }


    /// <summary>
    /// 图片尺寸
    /// </summary>
    public class ImageSize
    {
        [Column("Height")]
        public int Height { get; set; }

        [Column("Width")]
        public int Width { get; set; }

        [Column("Length")]
        public long Length { get; set; }

        public ImageSize()
        {
            Height = 0;
            Width = 0;
            Length = 0L;
        }

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
    public class Owner
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

        public Owner() { }
        public Owner(string id, string name, string email)
        {
            Id = id;
            Name = name;
            Email = email;
        }
    }


    /// <summary>
    /// 存储方
    /// </summary>
    public class Storager
    {
        /// <summary>
        /// IStorge 插件 ID
        /// </summary>
        [MaxLength(128)]
        [Column("StoragerId")]
        public string Id { get; set;}

        /// <summary>
        /// 图片链接（相对）
        /// </summary>
        [MaxLength(256)]
        [Column("StoragerUrl")]
        public string RelativeUrl { get; set; }

        /// <summary>
        /// 图片缩略图链接（相对）
        /// </summary>
        [MaxLength(256)]
        [Column("StoragerThumbnailUrl")]
        public string RelativeThumbnailUrl { get; set; }

        public Storager()
        {
            Id = string.Empty;
            RelativeUrl = string.Empty;
            RelativeThumbnailUrl = string.Empty;
        }

        public Storager(string id, string relativeUrl, string relativeThumbnailUrl)
        {
            Id = id;
            RelativeUrl = relativeUrl;
            RelativeThumbnailUrl = relativeThumbnailUrl;
        }
    }
}