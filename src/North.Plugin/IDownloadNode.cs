using Microsoft.AspNetCore.Http;

namespace North.Plugin
{
    /// <summary>
    /// 图片下载流程节点
    /// </summary>
    public interface IDownloadNode
    {
        Task InvokeAsync(HttpRequest request);
    }
}