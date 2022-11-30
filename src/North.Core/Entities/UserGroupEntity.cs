using SqlSugar;

namespace North.Core.Entities
{
    /// <summary>
    /// 用户组实体
    /// </summary>
    [SugarTable("UserGroups")]
    public class UserGroupEntity : Entity
    {
        /// <summary>
        /// 用户组名
        /// </summary>
        public string Name { get; set; }

        #region 导航属性
        /// <summary>
        /// 用户组成员
        /// </summary>
        [Navigate(NavigateType.OneToMany, nameof(UserEntity.UserGroupId))]
        public List<UserEntity> Users { get; set; }
        #endregion
    }
}
