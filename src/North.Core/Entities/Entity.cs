using SqlSugar;

namespace North.Core.Entities
{
    /// <summary>
    /// 实体基类
    /// </summary>
    public class Entity
    {
        [SugarColumn(IsPrimaryKey = true)]
        public Guid Id { get; set; } = Guid.NewGuid();
    }
}
