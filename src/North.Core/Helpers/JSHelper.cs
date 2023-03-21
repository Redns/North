using Microsoft.JSInterop;

namespace North.Core.Helpers
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
        public static async ValueTask<string> CopyToClipboardAsync(this IJSRuntime JS, string content)
        {
            return string.IsNullOrEmpty(content) ? string.Empty : await JS.InvokeAsync<string>("copyTextToClipboard", content);
        }


        /// <summary>
        /// 设置页面焦点
        /// </summary>
        /// <param name="JS"></param>
        /// <param name="id">控件 id</param>
        /// <returns></returns>
        public static async ValueTask<string> SetFocusAsync(this IJSRuntime JS, string id)
        {
            return await JS.InvokeAsync<string>("setFocus", id);
        }


        /// <summary>
        /// 设置页面 Body 样式
        /// </summary>
        /// <param name="JS"></param>
        /// <param name="backgroundColor">背景颜色</param>
        /// <param name="filter">过滤器</param>
        /// <returns></returns>
        public static async ValueTask<string> SetBodyStyleAsync(this IJSRuntime JS, string backgroundColor, string filter)
        {
            return await JS.InvokeAsync<string>("setBodyStyle", backgroundColor, filter);
        }


        /// <summary>
        /// 获取设备信息
        /// </summary>
        /// <param name="JS"></param>
        /// <returns></returns>
        public static async ValueTask<DeviceInfo> GetDeviceInfoAsync(this IJSRuntime JS)
        {
            return await JS.InvokeAsync<DeviceInfo>("getDeviceInfo");
        }


        /// <summary>
        /// 上传文件至浏览器 Blob
        /// </summary>
        /// <param name="JS"></param>
        /// <param name="stream">待上传文件数据流</param>
        /// <param name="contentType">文件类型（如："image/png"）</param>
        /// <returns>文件 Blob 链接</returns>
        public static async ValueTask<string> UploadToBlobAsync(this IJSRuntime JS, Stream stream, string contentType)
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
        public static async ValueTask<string> UploadToBlobAsync(this IJSRuntime JS, string path, string contentType)
        {
            using var fileReadStream = File.OpenRead(path);
            return await JS.UploadToBlobAsync(fileReadStream, contentType);
        }


        /// <summary>
        /// 调用浏览器下载文件
        /// </summary>
        /// <param name="JS"></param>
        /// <param name="filename">保存的文件名称</param>
        /// <param name="url">文件链接</param>
        /// <returns></returns>
        public static async ValueTask<string> DownloadAsync(this IJSRuntime JS, string filename, string url)
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
        public static async ValueTask<Stream> DownloadBlobAsync(this IJSRuntime JS, string url, long maxAllowedSize = 512000)
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
        public static async ValueTask DownloadBlobAsync(this IJSRuntime JS, string url, string path, long maxAllowedSize = 512000)
        {
            using var fileWriteStream = File.OpenWrite(path);
            using var fileReadStream = await JS.DownloadBlobAsync(url, maxAllowedSize);
            await fileReadStream.CopyToAsync(fileWriteStream);
        }


        /// <summary>
        /// 销毁 Blob 文件
        /// </summary>
        /// <param name="JS"></param>
        /// <param name="url">Blob 文件链接</param>
        /// <returns></returns>
        public static async ValueTask<string> DestroyBlobAsync(this IJSRuntime JS, string url)
        {
            return await JS.InvokeAsync<string>("destroy", url);
        }
    }


    public class DeviceInfo
    {
        /// <summary>
        /// 操作系统
        /// </summary>
        public string Os { get; set; }

        /// <summary>
        /// 设备标识
        /// </summary>
        public string Description { get; set; }

        /// <summary>
        /// 屏幕尺寸
        /// </summary>
        public Screen Screen { get; set; }
    }


    /// <summary>
    /// 屏幕尺寸
    /// </summary>
    public class Screen
    {
        /// <summary>
        /// 宽度（px）
        /// </summary>
        public int Width { get; set; }

        /// <summary>
        /// 高度（px）
        /// </summary>
        public int Height { get; set; }

    }
}
