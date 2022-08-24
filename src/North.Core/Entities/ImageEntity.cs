namespace North.Core.Entities
{
    public class ImageEntity
    {
        public string Id { get; set; }

        /// <summary>
        /// 文件名
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// 大小（单位：字节）
        /// </summary>
        public long Size { get; set; }

        /// <summary>
        /// 分辨率
        /// </summary>
        public Dpi Dpi { get; set; }

        /// <summary>
        /// 所有者
        /// </summary>
        public Owner Owner { get; set; }

        /// <summary>
        /// 上传时间
        /// </summary>
        public string UploadTime { get; set; }

        /// <summary>
        /// 存储方
        /// </summary>
        public Storager Storager { get; set; }

        /// <summary>
        /// 请求次数
        /// </summary>
        public long Request { get; set; }

        public ImageEntity(string id, string name, long size, Dpi dpi, Owner owner, Storager storager)
        {
            Id = id;
            Name = name;
            Size = size;
            Dpi = dpi;
            Owner = owner;
            UploadTime = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
            Storager = storager;
            Request = 0;
        }

        public ImageEntity(string id, string name, long size, Dpi dpi, Owner owner, string uploadTime, Storager storager, long request)
        {
            Id = id;
            Name = name;
            Size = size;
            Dpi = dpi;
            Owner = owner;
            UploadTime = uploadTime;
            Storager = storager;
            Request = request;
        }
    }


    /// <summary>
    /// 图片尺寸
    /// </summary>
    public struct Dpi
    {
        public int Height { get; set; }
        public int Width { get; set; }

        public Dpi(int height, int width)
        {
            Height = height;
            Width = width;
        }
    }


    /// <summary>
    /// 图片所有者
    /// </summary>
    public struct Owner
    {
        public string Id { get; set; }
        public string Name { get; set; }
        public string Email { get; set; }

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
    public struct Storager
    {
        /// <summary>
        /// 存储方 ID
        /// 存储方：指的是实现了 IStorage 接口的插件，North 的本地存储也是通过插件实现的；
        /// 存储方ID：IStorage 接口的 ID，不是插件本身的 ID（因为一个插件可能会实现若干个 IStorage 接口）；
        /// 若仅保留图片链接而不保留接口ID，则无法执行图片删除等操作。
        /// </summary>
        public string Id { get; set;}

        /// <summary>
        /// 图片绝对地址
        /// </summary>
        public string AbsoluteUrl { get; set; }

        public Storager(string id, string absoluteUrl)
        {
            Id = id;
            AbsoluteUrl = absoluteUrl;
        }
    }
}