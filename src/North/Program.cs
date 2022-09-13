using Microsoft.AspNetCore.Authentication.Cookies;
using MudBlazor;
using MudBlazor.Services;
using North.Common;
using North.Core.Entities;
using North.Core.Services.Logger;
using North.Core.Services.Poster;
using System.Security.Claims;
using ILogger = North.Core.Services.Logger.ILogger;

class Program
{
    public static WebApplication? app;
    public static WebApplicationBuilder? builder;

    public static void Main(string[] args)
    {
        // 创建容器
        builder = WebApplication.CreateBuilder(args);

        builder.Services.AddRazorPages();
        builder.Services.AddControllers();
        builder.Services.AddHttpContextAccessor();
        builder.Services.AddMudServices(config =>
        {
            // Snackbar 配置
            config.SnackbarConfiguration.PositionClass = Defaults.Classes.Position.TopCenter;
            config.SnackbarConfiguration.PreventDuplicates = true;
            config.SnackbarConfiguration.SnackbarVariant = Variant.Text;
            config.SnackbarConfiguration.ShowCloseIcon = false;
            config.SnackbarConfiguration.VisibleStateDuration = 1500;
            config.SnackbarConfiguration.HideTransitionDuration = 200;
            config.SnackbarConfiguration.ShowTransitionDuration = 200;
        });
        builder.Services.AddServerSideBlazor(option =>
        {
            // 当未处理的异常产生时，是否发送详细的错误信息至 Javascript
            // 生产环境应当关闭，否则可能暴漏程序信息
            option.DetailedErrors = false;
            // JS 互调用超时事件设置
            option.JSInteropDefaultCallTimeout = TimeSpan.FromSeconds(10);
        });
        builder.Services.AddDbContextPool<NorthDbContext>(builder =>
        {
            var enabledDatabaseName = GlobalValues.AppSettings.General.DataBase.EnabledName;
            var enabledDatabase = GlobalValues.AppSettings.General.DataBase.Databases.FirstOrDefault(d => d.Name == enabledDatabaseName);
            if(enabledDatabase is null)
            {
                throw new KeyNotFoundException("Cannot load database, please check appsettings.json");
            }
            else
            {
                enabledDatabase.InitDbContextBuilder(builder);
            }
        });
        builder.Services.AddLogging(logging =>
        {
            // 清除默认日志组件
            logging.ClearProviders();
        });
        builder.Services.AddSingleton<ILogger, NLogger>(logger => new NLogger(GlobalValues.AppSettings.Log));
        builder.Services.AddAuthentication(options =>
                        {
                            options.DefaultScheme = CookieAuthenticationDefaults.AuthenticationScheme;
                            options.RequireAuthenticatedSignIn = false;
                        })
                        .AddCookie(options =>
                        {
                            // 确定用于创建 Cookie 的设置
                            options.Cookie = new CookieBuilder()
                            {
                                // SameSite 属性用户限制第三方 Cookie，减少安全风险，它有三个值（参考：http://www.ruanyifeng.com/blog/2019/09/cookie-samesite.html）
                                // Strict: 完全禁止第三方 Cookie，跨站点（顶级、二级域名不同）时不会发送 Cookie
                                // Lax: 大多数情况也是不发送第三方 Cookie，但是导航到目标网址的 Get 请求除外
                                // None: 显式关闭 SameSite 属性，但必须同时设置 Secure 属性（Cookie 只能通过 HTTPS 协议发送）
                                SameSite = SameSiteMode.Lax,
                                // 设置为 false 可阻止 JS 发现、修改 Cookie
                                HttpOnly = true,
                                // 根据提供 Cookie 的 URI 的类型（HTTP/HTTPS）来决定后续何时在请求时携带 Cookie，它有三个值
                                // Always: 要求登录页及之后所有需要身份验证的页面均为 HTTPS
                                // None: 登录页为 HTTPS，但其他 HTTP 页也需要身份验证信息
                                // SameAsRequest: 若提供 Cookie 的 URI 为 HTTPS，则只会在后续 HTTPS 请求上将 Cookie 返回服务器；若提供 Cookie 的 URI 为 HTTP，则会在后续 HTTP 和 HTTPS 请求上将 Cookie 返回服务器。
                                SecurePolicy = CookieSecurePolicy.SameAsRequest,
                            };
                            // 订阅 Cookie 身份验证期间发生的事件
                            options.Events = new CookieAuthenticationEvents
                            {
                                // 登录完成后调用
                                // 此时实际上并没有完成 Cookie 写入，因此无法通过 context.HttpContext.User.Claims 获取用户信息
                                OnSignedIn = (context) =>
                                {
                                    var email = app?.Services
                                                   ?.GetService<Dictionary<string, ClaimsIdentity>>()
                                                   ?.GetValueOrDefault(context.Request.Query.First(q => q.Key == "key").Value)
                                                   ?.FindFirst(ClaimTypes.Email)
                                                   ?.Value;
                                    if (!string.IsNullOrWhiteSpace(email))
                                    {
                                        app?.Services.GetRequiredService<ILogger>().Info($"{email} login");
                                    }
                                    return Task.CompletedTask;
                                },
                                // 注销时调用
                                OnSigningOut = (context) =>
                                {
                                    var email = context.HttpContext
                                                       .User
                                                       .Identities
                                                       .First()
                                                       .FindFirst(ClaimTypes.Email)
                                                       ?.Value;
                                    if (!string.IsNullOrWhiteSpace(email))
                                    {
                                        app?.Services.GetRequiredService<ILogger>().Info($"{email} logout");
                                    }
                                    return Task.CompletedTask;
                                }
                            };
                        });
        builder.Services.AddSingleton<IPoster, MineKitPoster>(poster => new MineKitPoster());
        builder.Services.AddSingleton(loginIdentify => new Dictionary<string, ClaimsIdentity>());


        // 构建 web 应用
        app = builder.Build();

        app.UseRouting();
        app.UseStaticFiles();
        app.UseAuthentication();
        app.UseAuthorization();
        app.UseEndpoints(endpoint =>
        {
            endpoint.MapBlazorHub();
            endpoint.MapControllers();
            endpoint.MapFallbackToPage("/_Host");
        });
        app.Urls.Add("http://*:12121");

        app.Run();
    }
}