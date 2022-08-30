using Microsoft.AspNetCore.Components.Forms;
using Microsoft.JSInterop;
using MudBlazor;
using North.Core.Entities;
using North.Core.Helper;
using System.Security.Claims;

namespace North.Models
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
        /// 图片缩略图链接
        /// </summary>
        public string ThumbnailUrl { get; private set; }

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
        /// 图片数据流
        /// </summary>
        public Stream Stream { get; private set; }

        /// <summary>
        /// 图片尺寸
        /// </summary>
        public ImageSize Size { get; set; }

        /// <summary>
        /// 图片存储信息
        /// </summary>
        public Storager Storager { get; set; }

        /// <summary>
        /// 进度条颜色
        /// </summary>
        public Color ProgressColor => State switch
        {
            ImageUploadState.Failed => Color.Error,
            ImageUploadState.Success => Color.Tertiary,
            _ => Color.Info
        };

        public ImageUploadModel(string name, string contentType, string thumbnailUrl, Stream stream, ImageSize size, Storager storager, int progress = 0, string message = "等待上传", ImageUploadState state = ImageUploadState.UnStart)
        {
            Name = name;
            ContentType = contentType;
            ThumbnailUrl = thumbnailUrl;
            Progress = progress;
            Message = message;
            State = state;
            Stream = stream;
            Size = size;
            Storager = storager;
        }

        public ImageEntity ToEntity(IHttpContextAccessor accessor)
        {
            return new ImageEntity(Name, Size, new Owner()
            {
                Id = accessor.HttpContext?.User.Claims.First(c => c.Type == ClaimTypes.SerialNumber).Value ?? throw new ArgumentException("Cannot get user_id in cookies"),
                Name = accessor.HttpContext?.User.Claims.First(c => c.Type == ClaimTypes.Name).Value ?? throw new ArgumentException("Cannot get user_name in cookies"),
                Email = accessor.HttpContext?.User.Claims.First(c => c.Type == ClaimTypes.SerialNumber).Value ?? throw new ArgumentException("Cannot get user_email in cookies")
            }, Storager);
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
