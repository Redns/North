using MudBlazor;
using North.Core.Entities;
using North.Data.Access;

namespace North.Pages.Database
{
    partial class Users
    {
        private bool DataLoading { get; set; } = true;
        private string SearchText { get; set; } = string.Empty;
        private List<UserEntity> UsersAll { get; set; } = new List<UserEntity>();
        private List<UserEntity> UsersShow { get; set; } = new List<UserEntity>();

        /// <summary>
        /// 使用不同的颜色标记用户权限
        /// </summary>
        /// <param name="permission"></param>
        /// <returns></returns>
        private Color PermissionChipColor(Permission permission) => permission switch
        {
            Permission.System => Color.Success,
            Permission.Administrator => Color.Info,
            _ => Color.Primary
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

        protected override async Task OnAfterRenderAsync(bool firstRender)
        {
            if (firstRender)
            {
                UsersAll.AddRange(await new SqlUserData(_context).GetAsync());
                UsersShow.AddRange(UsersAll);
                await InvokeAsync(() =>
                {
                    DataLoading = false;
                    StateHasChanged();
                });
            }
            await base.OnAfterRenderAsync(firstRender);
        }


        private bool UserFilter(UserEntity user)
        {
            return string.IsNullOrWhiteSpace(SearchText) || user.Name.Contains(SearchText) || user.Email.Contains(SearchText);
        }
    }
}
