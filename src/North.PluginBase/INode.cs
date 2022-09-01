using North.Core.Models;

namespace North.PluginBase
{
    /// <summary>
    /// North 插件节点
    /// </summary>
    /// <typeparam name="T">插件设置类型</typeparam>
    public interface INode<T> where T : SettingBase 
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="Services"></param>
        /// <param name="settings"></param>
        /// <param name="image"></param>
        void Invoke(in IServiceProvider Services, in T settings, in ImageUploadModel image);
        Task InvokeAsync(in IServiceProvider Services, in T settings, in ImageUploadModel image);
    }
}