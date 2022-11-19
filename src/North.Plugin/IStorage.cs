using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using North.Core.Models;

namespace North.Plugin
{
    /// <summary>
    /// 图片存储接口
    /// </summary>
    public interface IStorage
    {
        /// <summary>
        /// 上传图片
        /// </summary>
        /// <param name="image"></param>
        /// <returns></returns>
        Task UploadAsync(ImageUploadModel image);


        /// <summary>
        /// 下载图片
        /// </summary>
        /// <param name="request">下载请求</param>
        /// <returns>图片</returns>
        ValueTask<FileContentResult> DownloadAsync(HttpRequest request);
    }
}