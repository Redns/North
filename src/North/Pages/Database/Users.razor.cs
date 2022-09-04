using Microsoft.AspNetCore.Components.Web;
using MudBlazor;
using North.Core.Entities;
using North.Data.Access;
using System.Data;

namespace North.Pages.Database
{
    partial class Users
    {
        private bool ShowInTable { get; set; } = true;
        private bool DataLoading { get; set; } = true;
        private string SearchText { get; set; } = string.Empty;
        private int ListLeftCount  => UsersShow.Count / 2 + UsersShow.Count % 2;
        private int ListRightCount => UsersShow.Count - ListLeftCount;
        private SqlUserData? SqlUserData { get; set; } = null;
        private List<UserEntity> UsersAll { get; set; } = new();
        private List<UserEntity> UsersShow { get; set; } = new();
        private HashSet<UserEntity> UsersSelected{ get; set; } = new();


        /// <summary>
        /// 加载用户数据
        /// </summary>
        /// <param name="firstRender"></param>
        /// <returns></returns>
        protected override async Task OnAfterRenderAsync(bool firstRender)
        {
            if (firstRender)
            {

                UsersAll.AddRange(await (SqlUserData = new SqlUserData(_context)).GetAsync());
                UsersShow.AddRange(UsersAll);
                await InvokeAsync(() =>
                {
                    DataLoading = false;
                    StateHasChanged();
                });
            }
            await base.OnAfterRenderAsync(firstRender);
        }


        /// <summary>
        /// 检索用户
        /// </summary>
        /// <param name="args"></param>
        private async Task SearchUsers()
        {
            // 清空之前检索的用户
            await InvokeAsync(() =>
            {
                UsersShow.Clear();
                DataLoading = true;
                StateHasChanged();
            });

            // 检索用户
            UsersShow.AddRange(string.IsNullOrWhiteSpace(SearchText) ? UsersAll : UsersAll.FindAll(user => user.Name.Contains(SearchText) || user.Email.Contains(SearchText)));
            DataLoading = false;
        }


        /// <summary>
        /// 删除用户
        /// </summary>
        /// <param name="user">待删除的用户</param>
        /// <returns></returns>
        private async Task RemoveUser(UserEntity user)
        {
            try
            {
                // 清空本地用户信息
                await InvokeAsync(() =>
                {
                    UsersAll.Remove(user);
                    UsersShow.Remove(user);
                    UsersSelected.Remove(user);

                    DataLoading = true;
                    StateHasChanged();
                });

                // 清除用户数据表中的信息
                if(SqlUserData is null || !await SqlUserData.RemoveAsync(user))
                {
                    _snackbar.Add("删除失败", Severity.Error);
                }

                // 清除图片信息
                
            }
            catch
            {

            }
            finally
            {
                DataLoading = false;
            }
        }


        /// <summary>
        /// 切换视图
        /// </summary>
        private void SwitchView()
        {
            ShowInTable = !ShowInTable;
            UsersSelected.Clear();
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
