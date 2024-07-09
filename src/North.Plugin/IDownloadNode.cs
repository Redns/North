using Microsoft.AspNetCore.Http;
using North.Core.Entities;

namespace North.Plugin
{
    /// <summary>
    /// 图片下载流程节点
    /// </summary>
    public interface IDownloadNode
    {
        Task InvokeAsync(in HttpRequest request, in ImageEntity image);
    }
}