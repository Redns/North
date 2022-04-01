using System.ComponentModel;

namespace ImageBed.Data.Entity
{
    public class ImageEntity
    {
        public string? Id { get; set; }

        [DisplayName("名 称")]
        public string? Name { get; set; }

        public string? Url { get; set; }

        [DisplayName("大 小")]
        public string? Size { get; set; }

        [DisplayName("标 签")]
        public string? Tags { get; set; }

        [DisplayName("上传时间")]
        public string? UploadTime { get; set; }

        [DisplayName("修改时间")]
        public string? LastModifyTime { get; set; }

        [DisplayName("操作者")]
        public string? Uploader { get; set; }
    }
}
