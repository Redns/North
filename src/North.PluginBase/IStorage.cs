using Microsoft.AspNetCore.Http;
using North.Core.Entities;
using North.Core.Models;

namespace North.PluginBase
{
    /// <summary>
    /// 图床存储模块
    /// </summary>
    /// <typeparam name="PluginSettings"></typeparam>
    public interface IStorage
    {
        /// <summary>
        /// 上传图片
        /// </summary>
        /// <param name="services">已注册的服务</param>
        /// <param name="settings">插件设置</param>
        /// <param name="image">待上传的图片</param>
        /// <returns>(图片尺寸, 图片链接)</returns>
        (ImageSize, ImageUrl) Upload(IServiceProvider services, SettingBase settings, in ImageUploadModel image);
        ValueTask<(ImageSize, ImageUrl)> UploadAsync(IServiceProvider services, SettingBase settings, in ImageUploadModel image);


        /// <summary>
        /// 下载图片
        /// </summary>
        /// <param name="services">已注册的服务</param>
        /// <param name="settings">插件设置</param>
        /// <param name="request">HTTP 请求信息</param>
        /// <param name="url">图片链接</param>
        /// <returns>图片数据流</returns>
        Stream Download(IServiceProvider services, SettingBase settings, string url, in HttpRequest? request = null);
        ValueTask<Stream> DownloadAsync(IServiceProvider services, SettingBase setting, string url, in HttpRequest? request = null);


        bool Rename(IServiceProvider services, SettingBase settings);


        /// <summary>
        /// 删除图片
        /// </summary>
        /// <param name="services">已注册的服务</param>
        /// <param name="settings">插件设置</param>
        /// <param name="url">图片链接</param>
        /// <returns>删除成功返回 true，否则返回 false</returns>
        bool Delete(IServiceProvider services, SettingBase settings, string url);
        ValueTask<bool> DeleteAsync(IServiceProvider services, SettingBase settings, string url);
    }
}