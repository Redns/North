using SqlSugar;

namespace North.Core.Entities
{
    [SugarTable("Plugins")]
    public class PluginEntity : Entity
    {
        #region 插件基本信息
        /// <summary>
        /// 插件名称
        /// </summary>
        [SugarColumn(Length = 64)]
        public string Name { get; set; }

        /// <summary>
        /// 插件作者
        /// </summary>
        [SugarColumn(Length = 64)]
        public string Author { get; set; }

        /// <summary>
        /// 插件版本号
        /// </summary>
        [SugarColumn(Length = 32)]
        public string Version { get; set; }

        /// <summary>
        /// 插件下载量
        /// </summary>
        public long DownloadCount { get; set; }

        /// <summary>
        /// 插件图标
        /// </summary>
        [SugarColumn(Length = 128)]
        public string IconUrl { get; set; }

        /// <summary>
        /// 插件描述
        /// </summary>
        [SugarColumn(Length = 128)]
        public string Description { get; set; }
        #endregion

        /// <summary>
        /// 插件状态
        /// </summary>
        public PluginState State { get; set; }

        /// <summary>
        /// 安装文件夹（绝对路径）
        /// </summary>
        [SugarColumn(Length = 256)]
        public string InstallDir { get; set; }

        #region 导航属性
        [Navigate(NavigateType.OneToMany, nameof(PluginModuleEntity.PluginId))]
        public List<PluginModuleEntity> Modules { get; set; }
        #endregion
    }


    /// <summary>
    /// 插件状态
    /// </summary>
    public enum PluginState
    {
        UnInstall = 0,      // 未安装
        Installing,         // 安装中
        Enable,             // 已启用
        Disable             // 已禁用
    }
}
