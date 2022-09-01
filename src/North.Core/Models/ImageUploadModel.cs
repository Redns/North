using MudBlazor;
using North.Core.Entities;

namespace North.Core.Models
{
    /// <summary>
    /// 图片上传页面模型
    /// </summary>
    public class ImageUploadModel
    {
        /// <summary>
        /// 图片名称
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// 图片类型
        /// </summary>
        public string ContentType { get; private set; }

        /// <summary>
        /// 图片预览图链接
        /// </summary>
        public string PreviewUrl { get; private set; }

        /// <summary>
        /// 图片数据流
        /// </summary>
        public Stream Stream { get; set; }

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
        /// 图片尺寸
        /// </summary>
        public ImageSize? Size { get; set; }

        /// <summary>
        /// 图片链接
        /// </summary>
        public ImageUrl? Url { get; set; }

        /// <summary>
        /// 进度条颜色
        /// </summary>
        public Color ProgressColor => State switch
        {
            ImageUploadState.Failed => Color.Error,
            ImageUploadState.Success => Color.Tertiary,
            _ => Color.Info
        };

        public ImageUploadModel(string name, string contentType, string previewUrl, Stream stream, int progress = 0, string? message = null, ImageUploadState state = ImageUploadState.UnStart, ImageSize? size = null, ImageUrl? url = null)
        {
            Name = name;
            ContentType = contentType;
            PreviewUrl = previewUrl;
            Stream = stream;
            Progress = progress;
            Message = message ?? "等待上传";
            State = state;
            Size = size;
            Url = url;
        }


        /// <summary>
        /// 模型转实体
        /// </summary>
        /// <param name="owner">图片所有者</param>
        /// <param name="storager">存储模块ID</param>
        /// <returns></returns>
        /// <exception cref="ArgumentException"></exception>
        public ImageEntity ToEntity(Owner owner, string storager)
        {
            return new ImageEntity(Name, Size ?? new ImageSize(0, 0, Length), owner, storager, Url ?? throw new ArgumentException($"{Name}'s url is null"));
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
