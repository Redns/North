using System.ComponentModel.DataAnnotations;

namespace ImageBed.Data.Entity
{
    public class RecordEntity
    {
        [Key]
        public string Date { get; set; } = string.Empty;    // 日期
        public int UploadImageNum { get; set; } = 0;        // 上传图片数量
        public int UploadImageSize { get; set; } = 0;       // 上传图片尺寸(单位:MB)
        public int RequestNum { get; set; } = 0;            // 请求次数

        public RecordEntity(string date, int uploadImageNum, int uploadImageSize, int requestNum)
        {
            Date = date;
            UploadImageNum = uploadImageNum;
            UploadImageSize = uploadImageSize;
            RequestNum = requestNum;
        }
    }
}
