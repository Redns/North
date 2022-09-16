using North.Core.Models;

namespace North.PluginBase
{
    /// <summary>
    /// 插件节点
    /// </summary>
    public interface INode
    {
        void Invoke(IServiceProvider services, SettingBase settings, in ImageUploadModel image);
        Task InvokeAsync(IServiceProvider services, SettingBase settings, in ImageUploadModel image);
    }
}