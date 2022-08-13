using Microsoft.AspNetCore.Components;
using MudBlazor;
using North.Core.Data.Access;
using North.Core.Data.Entities;
using North.Core.Helper;

namespace North.Pages.Auth
{
    partial class Verify
    {
        [Parameter]
        [SupplyParameterFromQuery]
        public string Id { get; set; } = string.Empty;

        [Parameter]
        [SupplyParameterFromQuery]
        public string Type { get; set; } = string.Empty;

        public int RemainTime { get; set; } = 5;
        public bool VerifyRunning { get; set; } = true;


        /// <summary>
        /// 验证用户信息
        /// </summary>
        /// <param name="firstRender"></param>
        /// <returns></returns>
        protected override async Task OnAfterRenderAsync(bool firstRender)
        {
            if (firstRender)
            {
                try
                {
                    switch (Type.ToLower())
                    {
                        case "register": await VerifyRegister(); break;
                        default: break;
                    }
                }
                catch (Exception e)
                {
                    _logger.Error($"Verify {Type} error", e);
                    _snackbar.Add("验证失败", Severity.Error);
                }
                finally
                {
                    VerifyRunning = false;
                    await InvokeAsync(() => StateHasChanged());

                    while (RemainTime > 0)
                    {
                        await Task.Delay(1000);
                        await InvokeAsync(() =>
                        {
                            RemainTime -= 1;
                            StateHasChanged();
                        });
                    }
                    _navigationManager.NavigateTo("login", true);
                }
            }
            await base.OnAfterRenderAsync(firstRender);
        }


        /// <summary>
        /// 用户注册验证
        /// </summary>
        /// <returns></returns>
        private async Task VerifyRegister()
        {
            var sqlUserData = new SqlUserData(_context);
            var sqlVerifyEmailData = new SqlVerifyEmailData(_context);

            var verifyEmail = sqlVerifyEmailData.Find(e => e.Id == Id);
            if ((verifyEmail is null) || (IdentifyHelper.TimeStamp > verifyEmail.ExpireTime))
            {
                _snackbar.Add("链接不存在或已过期", Severity.Error);
            }
            else
            {
                var user = await sqlUserData.FindAsync(u => u.Email == verifyEmail.Email);
                if ((user is not null) && (user.State is State.Checking))
                {
                    user.State = State.Normal;
                    await sqlUserData.UpdateAsync(user);

                    _snackbar.Add("验证成功", Severity.Success);
                }
                else
                {
                    _snackbar.Add("账户不存在或状态异常", Severity.Error);
                }
            }

            // 删除验证邮件
            if (verifyEmail is not null)
            {
                await sqlVerifyEmailData.RemoveAsync(verifyEmail);
            }
        }
    }
}
