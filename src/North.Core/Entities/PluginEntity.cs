using NuGet.Protocol.Core.Types;
using System.ComponentModel.DataAnnotations;

namespace North.Core.Entities
{
    public class PluginEntity : Entity
    {
        /// <summary>
        /// 名称
        /// </summary>
        [Required]
        [MaxLength(256)]
        public string Name { get; set; }

        /// <summary>
        /// 状态
        /// </summary>
        public PluginState State { get; set; }

        #region 插件基本信息

        /// <summary>
        /// 作者
        /// </summary>
        public string Authors { get; set; }

        /// <summary>
        /// 版本号
        /// </summary>
        public string Version { get; set; }

        /// <summary>
        /// 下载量
        /// </summary>
        public long DownloadCount { get; set; }

        /// <summary>
        /// 图标
        /// </summary>
        public string IconUrl { get; set; }

        /// <summary>
        /// 描述
        /// </summary>
        public string Description { get; set; }

        #endregion

        #region 导航属性

        public ICollection<PluginModuleEntity> Modules { get; set; }
        
        #endregion
    }


    /// <summary>
    /// 插件状态
    /// </summary>
    public enum PluginState
    {
        UnInstall = 0,      // 未安装
        Enable,             // 已启用
        Disable             // 已禁用
    }
}
