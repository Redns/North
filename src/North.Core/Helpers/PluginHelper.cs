using North.Core.Common;
using North.Core.Entities;
using System.Reflection;
using System.Runtime.Loader;

namespace North.Core.Helpers
{
    public static class PluginHelper
    {
        /// <summary>
        /// 加载插件
        /// </summary>
        /// <param name="pluginPath">插件所在路径（绝对路径）</param>
        /// <returns></returns>
        /// <exception cref="ArgumentException"></exception>
        public static CollectibleAssemblyLoadContext Load(string pluginPath)
        {
            return new CollectibleAssemblyLoadContext(pluginPath);
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="context"></param>
        public static void UnLoad(CollectibleAssemblyLoadContext context)
        {
            context.Unload();
        }


        /// <summary>
        /// 安装插件
        /// </summary>
        /// <param name="plugin">待安装的插件</param>
        /// <param name="setting">插件设置</param>
        public static void Install(PluginEntity plugin, PluginSetting setting)
        {

        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="plugin"></param>
        /// <param name="setting"></param>
        public static void UnInstall(PluginEntity plugin, PluginSetting setting)
        {
            Directory.Delete(Path.Combine(setting.InstallDir, plugin.Name), true);
        }
    }


    public class CollectibleAssemblyLoadContext : AssemblyLoadContext
    {
        public Assembly Assembly { get; set; }

        public CollectibleAssemblyLoadContext(string assemblyPath) : base(isCollectible: true)
        {
            Assembly = LoadFromAssemblyPath(assemblyPath);
        }

        protected override Assembly? Load(AssemblyName assemblyName)
        {
            return null;
        }
    }
}
