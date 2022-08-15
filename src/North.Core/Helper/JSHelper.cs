using Microsoft.JSInterop;

namespace North.Core.Helper
{
    /// <summary>
    /// .NET 与 JS 互操作辅助类
    /// </summary>
    /// <remarks>不建议使用 JS.InvokeVoidAsync</remarks>
    public static class JSHelper
    {
        /// <summary>
        /// 复制文本至剪贴板
        /// </summary>
        /// <param name="JS"></param>
        /// <param name="content">待复制的文本</param>
        /// <returns></returns>
        public static async ValueTask<string> CopyToClipboard(this IJSRuntime JS, string content)
        {
            return string.IsNullOrEmpty(content) ? string.Empty : await JS.InvokeAsync<string>("copyTextToClipboard", content);
        }


        /// <summary>
        /// 设置页面焦点
        /// </summary>
        /// <param name="JS"></param>
        /// <param name="id">控件 id</param>
        /// <returns></returns>
        public static async ValueTask<string> SetFocus(this IJSRuntime JS, string id)
        {
            return await JS.InvokeAsync<string>("setFocus", id);
        }


        /// <summary>
        /// 获取屏幕尺寸
        /// </summary>
        /// <param name="JS"></param>
        /// <returns></returns>
        public static async ValueTask<(double, double)> GetScreenSize(this IJSRuntime JS)
        {
            var sizes = (await JS.InvokeAsync<string>("getScreenSize")).Split(',');
            if(sizes.Length is not 2)
            {
                return (0, 0);
            }
            else
            {
                return (double.Parse(sizes[0]), double.Parse(sizes[1]));
            }
        }


        /// <summary>
        /// 上传文件至浏览器 Blob
        /// </summary>
        /// <param name="JS"></param>
        /// <param name="stream">待上传文件数据流</param>
        /// <param name="contentType">文件类型（如："image/png"）</param>
        /// <returns>文件 Blob 链接</returns>
        public static async ValueTask<string> UploadToBlob(this IJSRuntime JS, Stream stream, string contentType)
        {
            using var streamRef = new DotNetStreamReference(stream);
            return await JS.InvokeAsync<string>("upload", streamRef, contentType);
        }


        /// <summary>
        /// 上传文件至浏览器 Blob
        /// </summary>
        /// <param name="JS"></param>
        /// <param name="path">待上传文件路径</param>
        /// <param name="contentType">文件类型（如："image/png"）</param>
        /// <returns>文件 Blob 链接</returns>
        public static async ValueTask<string> UploadToBlob(this IJSRuntime JS, string path, string contentType)
        {
            using var fileReadStream = File.OpenRead(path);
            return await UploadToBlob(JS, fileReadStream, contentType);
        }


        /// <summary>
        /// 调用浏览器下载文件
        /// </summary>
        /// <param name="JS"></param>
        /// <param name="filename">保存的文件名称</param>
        /// <param name="url">文件链接</param>
        /// <returns></returns>
        public static async ValueTask<string> Download(this IJSRuntime JS, string filename, string url)
        {
            return await JS.InvokeAsync<string>("download", filename, url);
        }


        /// <summary>
        /// 下载 Blob 文件
        /// </summary>
        /// <param name="JS"></param>
        /// <param name="url">Blob 文件链接</param>
        /// <param name="maxAllowedSize">.NET 与 JS 交互最大内存占用（单位：字节）</param>
        /// <returns>Blob 文件数据流</returns>
        public static async ValueTask<Stream> DownloadBlob(this IJSRuntime JS, string url, long maxAllowedSize = 512000)
        {
            var downloadStreamRef = await JS.InvokeAsync<IJSStreamReference>("getBlobStream", url);
            return await downloadStreamRef.OpenReadStreamAsync(maxAllowedSize);
        }


        /// <summary>
        /// 下载 Blob 文件
        /// </summary>
        /// <param name="JS"></param>
        /// <param name="url">Blob 文件链接</param>
        /// <param name="path">文件保存路径</param>
        /// <param name="maxAllowedSize">.NET 与 JS 交互最大内存占用（单位：字节）</param>
        /// <returns></returns>
        public static async ValueTask DownloadBlob(this IJSRuntime JS, string url, string path, long maxAllowedSize = 512000)
        {
            using var fileWriteStream = File.OpenWrite(path);
            using var fileReadStream = await JS.DownloadBlob(url, maxAllowedSize);
            await fileReadStream.CopyToAsync(fileWriteStream);
        }


        /// <summary>
        /// 销毁 Blob 文件
        /// </summary>
        /// <param name="JS"></param>
        /// <param name="url">Blob 文件链接</param>
        /// <returns></returns>
        public static async ValueTask<string> DestroyBlob(this IJSRuntime JS, string url)
        {
            return await JS.InvokeAsync<string>("destroy", url);
        }
    }
}
