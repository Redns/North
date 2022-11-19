using MudBlazor;

namespace North.Core.Models
{
    /// <summary>
    /// 图片上传模型
    /// </summary>
    public class ImageUploadModel
    {
        /// <summary>
        /// 图片名称（含后缀）
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// 图片类型
        /// </summary>
        public string ContentType { get; init; }

        /// <summary>
        /// 图片预览图链接
        /// </summary>
        public string PreviewUrl { get; init; }

        /// <summary>
        /// 图片缩略图链接
        /// </summary>
        public string ThumbnailUrl { get; set; }

        /// <summary>
        /// 图片链接
        /// </summary>
        public string SourceUrl { get; set; }

        /// <summary>
        /// 图片高度
        /// </summary>
        public int Height { get; set; }

        /// <summary>
        /// 图片宽度
        /// </summary>
        public int Width { get; set; }

        /// <summary>
        /// 图片数据流
        /// </summary>
        public Stream Stream { get; init; }

        /// <summary>
        /// 图片大小（单位：字节）
        /// </summary>
        public long Length => Stream.Length;

        /// <summary>
        /// 图片上传进度（0-100）
        /// </summary>
        public int Progress { get; set; }

        /// <summary>
        /// 图片上传提示信息
        /// </summary>
        public string Message { get; set; }

        /// <summary>
        /// 图片上传状态
        /// </summary>
        public ImageUploadState State { get; set; }

        /// <summary>
        /// 进度条颜色
        /// </summary>
        public Color ProgressColor => State switch
        {
            ImageUploadState.Failed => Color.Error,
            ImageUploadState.Success => Color.Tertiary,
            _ => Color.Info
        };
    }


    /// <summary>
    /// 图片上传状态
    /// </summary>
    public enum ImageUploadState
    {
        UnStart = 0,        // 未开始
        Uploading,          // 上传中
        Success,            // 成功
        Failed              // 失败
    }
}
