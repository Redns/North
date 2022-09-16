using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace North.Core.Entities
{
    public class PluginModuleEntity : BaseEntity
    {
        /// <summary>
        /// 模块名称
        /// </summary>
        [Required]
        [MaxLength(256)]
        public string Name { get; set; }

        /// <summary>
        /// 模块类型名称
        /// </summary>
        public string TypeName { get; set; }

        /// <summary>
        /// 模块类型（使用反射获取）
        /// </summary>
        [NotMapped]
        public Type? Type => Type.GetType(TypeName);

        /// <summary>
        /// 模块所属类别，同一类型模块可能属于不同类别（如：INode）
        /// </summary>
        [MaxLength(64)]
        public string Category { get; set; }

        /// <summary>
        /// 执行顺序
        /// </summary>
        [Required]
        public int Index { get; set; }

        /// <summary>
        /// 是否启用模块
        /// </summary>
        public bool IsEnabled { get; set; }


        #region 导航属性
        public PluginEntity Plugin { get; set; }
        #endregion


        #region 构造函数

        public PluginModuleEntity() : base(new Guid()) { }
        public PluginModuleEntity(Guid id) : base(id) { }
        
        #endregion
    }
}
