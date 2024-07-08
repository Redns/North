using Microsoft.AspNetCore.Components.Forms;
using MudBlazor;
using NetTaste;

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
        public string Name { get; set; } = string.Empty;

        /// <summary>
        /// 图片类型
        /// </summary>
        public string ContentType { get; init; } = string.Empty;

        /// <summary>
        /// 图片预览图链接
        /// </summary>
        public string PreviewUrl { get; init; } = string.Empty;

        /// <summary>
        /// 图片缩略图链接
        /// </summary>
        public string ThumbnailUrl { get; set; } = string.Empty;

        /// <summary>
        /// 图片链接
        /// </summary>
        public string SourceUrl { get; set; } = string.Empty;

        /// <summary>
        /// 图片数据流
        /// </summary>
        public Stream Stream { get; init; }

        /// <summary>
        /// 图片大小（单位：字节）
        /// </summary>
        public long Length => Stream?.Length ?? 0L;

        /// <summary>
        /// 图片上传进度（0-100）
        /// </summary>
        public int Progress { get; set; } = 0;

        /// <summary>
        /// 图片上传提示信息
        /// </summary>
        public string Message { get; set; } = string.Empty;

        /// <summary>
        /// 图片上传状态
        /// </summary>
        public ImageUploadState State { get; set; } = ImageUploadState.UnStart;

        /// <summary>
        /// 进度条颜色
        /// </summary>
        public Color ProgressColor => State switch
        {
            ImageUploadState.Failed => Color.Error,
            ImageUploadState.Success => Color.Tertiary,
            _ => Color.Info
        };

        public ImageUploadModel(Stream stream)
        {
            Stream = stream;
        }

        public ImageUploadModel(IBrowserFile file, Stream stream, string preiewUrl)
        {
            Name = file.Name;
            ContentType = file.ContentType;
            PreviewUrl = preiewUrl;
            Stream = stream;
            State = ImageUploadState.UnStart;
            Progress = 0;
            Message = "等待上传...";
        }
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