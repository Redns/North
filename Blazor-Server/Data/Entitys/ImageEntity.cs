using System.ComponentModel;

namespace ImageBed.Data.Entity
{
    public class ImageEntity
    {
        public string Id { get; set; }

        [DisplayName("名 称")]
        public string Name { get; set; }

        public string Url { get; set; }

        [DisplayName("分辨率")]
        public string Dpi { get; set; }

        [DisplayName("大 小")]
        public string Size { get; set; }

        [DisplayName("上传时间")]
        public string UploadTime { get; set; }

        [DisplayName("所有者")]
        public string Owner { get; set; }

        [DisplayName("请求次数")]
        public int RequestNum { get; set; }

        public ImageEntity(string id, string name, string url, string dpi, string size, string uploadTime, string owner, int requestNum)
        {
            Id = id;
            Name = name;
            Url = url;
            Dpi = dpi;
            Size = size;
            UploadTime = uploadTime;
            Owner = owner;
            RequestNum = requestNum;
        }
    }
}
