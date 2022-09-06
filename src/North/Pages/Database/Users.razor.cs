using Microsoft.EntityFrameworkCore;
using MudBlazor;
using North.Core.Entities;

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
                var users = _context.Users.AsNoTracking().Include(u => u.Images).ToArray();
                var images = _context.Images.Include(i => i.Owner).ToArray();

                // 加载数据
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
            UsersShow.AddRange(string.IsNullOrWhiteSpace(SearchText) ? UsersAll : UsersAll.FindAll(user => UserFilter(user)));
            DataLoading = false;
        }


        private bool UserFilter(UserEntity user) => user.Name.Contains(SearchText) || user.Email.Contains(SearchText);


        /// <summary>
        /// 添加用户
        /// </summary>
        /// <param name="user"></param>
        /// <returns></returns>
        private async Task AddUser()
        {
            var r = new Random(DateTime.Now.Millisecond);
            var id = Guid.NewGuid().ToString();
            var user = new UserEntity 
            {
                Name = id[0..5],
                Email = $"{r.Next()}@163.com",
                Password = id[10..15],
                Avatar = id[15..20],
                State = r.NextDouble() switch
                        {
                            < 0.2 => UserState.Checking,
                            < 0.9 => UserState.Normal,
                            _ => UserState.Forbidden
                        },
                Permission = r.NextDouble() switch
                             {
                                 < 0.1 => UserPermission.Administrator,
                                 _ => UserPermission.User
                             },
                IsApiAvailable = r.NextDouble() > 0.8,
                SingleMaxUploadNums = r.Next(10) + 1,
                SingleMaxUploadCapacity = r.Next(100) + 1,
                MaxUploadNums = r.Next(100) + 11,
                MaxUploadCapacity = r.Next(1000) + 101
            };
            await SqlUserData.AddAsync(user);
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
        private Color PermissionChipColor(UserPermission permission) => permission switch
        {
            UserPermission.System => Color.Success,
            UserPermission.Administrator => Color.Primary,
            _ => Color.Info
        };

        /// <summary>
        /// 解析用户权限
        /// </summary>
        /// <param name="permission"></param>
        /// <returns></returns>
        private string PermissionChipText(UserPermission permission) => permission switch
        {
            UserPermission.System => "系 统",
            UserPermission.Administrator => "管理员",
            UserPermission.User => "用 户",
            _ => "未 知"
        };

        /// <summary>
        /// 使用不同的颜色标记用户状态
        /// </summary>
        /// <param name="state"></param>
        /// <returns></returns>
        private Color StateChipColor(UserState state) => state switch
        {
            UserState.Checking => Color.Warning,
            UserState.Normal => Color.Success,
            _ => Color.Error
        };

        /// <summary>
        /// 解析用户状态
        /// </summary>
        /// <param name="state"></param>
        /// <returns></returns>
        private string StateChipText(UserState state) => state switch
        {
            UserState.Checking => "待验证",
            UserState.Normal => "正 常",
            UserState.Forbidden => "已封禁",
            _ => "异 常"
        };
    }
}
