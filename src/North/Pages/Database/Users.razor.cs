using MudBlazor;
using North.Core.Entities;
using North.Data.Access;

namespace North.Pages.Database
{
    partial class Users
    {
        private bool ShowInTable { get; set; } = true;
        private bool DataLoading { get; set; } = true;
        private string SearchText { get; set; } = string.Empty;
        private SqlUserData? SqlUserData { get; set; } = null;
        private List<UserEntity> UsersAll { get; set; } = new List<UserEntity>();
        private List<UserEntity> UsersShow { get; set; } = new List<UserEntity>();

        /// <summary>
        /// 加载用户数据
        /// </summary>
        /// <param name="firstRender"></param>
        /// <returns></returns>
        protected override async Task OnAfterRenderAsync(bool firstRender)
        {
            if (firstRender)
            {
                // UsersAll.AddRange(await new SqlUserData(_context).GetAsync());
                // UsersShow.AddRange(UsersAll);
                SqlUserData = new SqlUserData(_context);
                var r = new Random(DateTime.Now.Millisecond);
                for (int i = 0; i < 300; i++)
                {
                    var id = Core.Helpers.IdentifyHelper.Generate();
                    UsersShow.Add(new UserEntity(id, id[0..5], $"{r.Next()}@163.com", id[10..15], id[15..20],
                                                 r.NextDouble() switch
                                                 {
                                                     < 0.6 => State.Checking,
                                                     < 0.9 => State.Normal,
                                                     _ => State.Forbidden
                                                 },
                                                 r.NextDouble() switch
                                                 {
                                                     < 0.05 => Permission.Administrator,
                                                     _ => Permission.User
                                                 }, r.NextDouble() > 0.8, r.Next(), r.Next(), r.Next(), r.Next()));
                }
                await InvokeAsync(() =>
                {
                    DataLoading = false;
                    StateHasChanged();
                });
            }
            await base.OnAfterRenderAsync(firstRender);
        }


        /// <summary>
        /// 判断用户是否符合搜索条件
        /// </summary>
        /// <param name="user">待判别的用户</param>
        /// <returns></returns>
        private bool UserFilter(UserEntity user)
        {
            return string.IsNullOrWhiteSpace(SearchText) || user.Name.Contains(SearchText) || user.Email.Contains(SearchText);
        }


        private async ValueTask<bool> RemoveUser(UserEntity user)
        {
            UsersShow.Remove(user);
            // UsersAll.Remove(user);
            return true;
            // return SqlUserData is not null && await SqlUserData.RemoveAsync(user);
        }


        /// <summary>
        /// 使用不同的颜色标记用户权限
        /// </summary>
        /// <param name="permission"></param>
        /// <returns></returns>
        private Color PermissionChipColor(Permission permission) => permission switch
        {
            Permission.System => Color.Success,
            Permission.Administrator => Color.Primary,
            _ => Color.Info
        };

        /// <summary>
        /// 解析用户权限
        /// </summary>
        /// <param name="permission"></param>
        /// <returns></returns>
        private string PermissionChipText(Permission permission) => permission switch
        {
            Permission.System => "系 统",
            Permission.Administrator => "管理员",
            Permission.User => "用 户",
            _ => "未 知"
        };

        /// <summary>
        /// 使用不同的颜色标记用户状态
        /// </summary>
        /// <param name="state"></param>
        /// <returns></returns>
        private Color StateChipColor(State state) => state switch
        {
            State.Checking => Color.Warning,
            State.Normal => Color.Success,
            _ => Color.Error
        };

        /// <summary>
        /// 解析用户状态
        /// </summary>
        /// <param name="state"></param>
        /// <returns></returns>
        private string StateChipText(State state) => state switch
        {
            State.Checking => "待验证",
            State.Normal => "正 常",
            State.Forbidden => "已封禁",
            _ => "异 常"
        };
    }
}
