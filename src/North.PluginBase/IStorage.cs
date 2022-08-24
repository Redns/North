namespace North.PluginBase
{
    /// <summary>
    /// 图床存储模块
    /// </summary>
    /// <typeparam name="PluginSettings"></typeparam>
    public interface IStorage<PluginSettings> where PluginSettings : SettingBase
    {
        /// <summary>
        /// 上传图片
        /// </summary>
        /// <param name="settings">插件设置</param>
        /// <param name="stream">图片流</param>
        /// <returns>图片链接</returns>
        string Upload(in PluginSettings settings, Stream stream);
        ValueTask<string> UploadAsync(in PluginSettings settings, Stream stream);

        /// <summary>
        /// 下载图片
        /// </summary>
        /// <param name="settings">插件设置</param>
        /// <param name="url">图片链接</param>
        /// <returns>图片流</returns>
        Stream Download(in PluginSettings settings, string url);
        ValueTask<Stream> DownloadAsync(in PluginSettings settings, string url);

        /// <summary>
        /// 删除图片
        /// </summary>
        /// <param name="settings">插件设置</param>
        /// <param name="url">图片链接</param>
        /// <returns>删除成功返回 true，否则返回 false</returns>
        bool Delete(in PluginSettings settings, string url);
        ValueTask<bool> DeleteAsync(in PluginSettings settings, string url);
    }
}