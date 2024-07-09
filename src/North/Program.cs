using IP2Region.Net.Abstractions;
using IP2Region.Net.XDB;
using Krins.Nuget;
using Masuit.Tools.Core.AspNetCore;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Mvc.ApplicationParts;
using Microsoft.AspNetCore.Mvc.Infrastructure;
using MudBlazor;
using MudBlazor.Services;
using North.Common;
using North.Core.Common;
using North.Core.Entities;
using North.Core.Repository;
using North.Core.Services.AuthService;
using North.Core.Services.ImageService;
using North.Core.Services.Logger;
using SqlSugar;
using System.Security.Claims;
using ILogger = North.Core.Services.Logger.ILogger;

namespace North
{
    public class Program
    {
        private static WebApplication app;
        private static WebApplicationBuilder builder;

        public static void Main(string[] args)
        {
            try
            {
                /**
                 * 创建容器
                 */
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
                // 应用设置
                builder.Services.AddSingleton(GlobalValues.AppSettings);
                // 数据库对象配置
                builder.Services.AddScoped<ISqlSugarClient>(client => new SqlSugarClient(GlobalValues.AppSettings.General.DataBase.DatabaseConnectionConfigs));
                // 清除系统默认日志组件
                builder.Services.AddLogging(logging =>
                {
                    logging.ClearProviders();
                });
                // 日志服务
                builder.Services.AddSingleton<ILogger, NLogger>(logger => new NLogger(GlobalValues.AppSettings.Log));
                // Cookie验证
                builder.Services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
                                .AddCookie(options =>
                                {
                                    // TODO 配置自动跳转
                                    options.LoginPath = new PathString("/login");
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
                                        SecurePolicy = CookieSecurePolicy.SameAsRequest
                                    };
                                    options.SlidingExpiration = true;
                                    options.Events = new CookieAuthenticationEvents
                                    {
                                    };
                                });
                // 用户登录信息暂存
                builder.Services.AddSingleton(loginIdentify => new Dictionary<string, ClaimsIdentity>());
                // 用户授权服务
                builder.Services.AddScoped<IAuthService<UserDTOEntity>>(auth =>
                {
                    var appSetting = app.Services.GetRequiredService<AppSetting>();
                    var sqlSlient = app.Services.GetRequiredService<ISqlSugarClient>();
                    var repository = new UserRepository(sqlSlient, appSetting.General.DataBase.EnabledName);

                    return new NorthAuthService(repository, GlobalValues.WithoutAuthenticationPages);
                });
                builder.Host.UseDefaultServiceProvider(options =>
                {
                    options.ValidateScopes = false;
                });
                // 动态加载控制器
                builder.Services.AddSingleton(NorthActionDescriptorChangeProvider.Instance);
                builder.Services.AddSingleton<IActionDescriptorChangeProvider>(NorthActionDescriptorChangeProvider.Instance);
                // Nuget 引擎
                builder.Services.AddSingleton(nugetEngine => new NugetEngine());
                // 插件程序集
                builder.Services.AddSingleton(pluginContext =>
                {
                    var applicationPartManager = app.Services.GetRequiredService<ApplicationPartManager>();
                    return new PluginsContext(applicationPartManager, NorthActionDescriptorChangeProvider.Instance)
                    {
                        OnRefreshRazorPages = (assemblies) =>
                        {
                            throw new NotImplementedException();
                        },
                        OnRefreshControllers = (applicationParts) =>
                        {
                            NorthActionDescriptorChangeProvider.Instance.HasChanged = true;
                            NorthActionDescriptorChangeProvider.Instance.TokenSource.Cancel();
                        }
                    };
                });
                // IP 物理地址查询
                // TODO 模式和路径可配置、数据文件可更新
                builder.Services.AddSingleton<ISearcher>(new Searcher(CachePolicy.Content, "Data/ip2region.xdb"));
                // 压缩解压工具
                builder.Services.AddSevenZipCompressor();
                // 图片服务
                builder.Services.AddSingleton<IImageService>(imageService => new NetVipsImageService());


                /**
                 * 构建 web 应用
                 */
                app = builder.Build();
                app.Use(app.Services.GetRequiredService<PluginsContext>().Middleware);
                app.UseStaticFiles();
                app.UseRouting();
                app.UseAuthentication();
                app.UseAuthorization();
                // TODO 设置页面增加端口修改功能
                app.Urls.Add(GlobalValues.AppSettings.General.ApplicationUrl);
                app.MapBlazorHub(options =>
                {
                    // Cookie 到期后自动断开 SignalR 连接
                    // 若设置为 false 则 Cookie 过期时除非手动刷新页面，否则不会更新当前状态，即 context.User.Identity?.IsAuthenticated 始终为 true
                    options.CloseOnAuthenticationExpiration = true;
                });
                app.MapFallbackToPage("/_Host");
                app.Run();
            }
            catch (Exception e)
            {
                Console.WriteLine($"{e.Message}\n{e.StackTrace}");
                Console.ReadKey();
            }
        }
    }
}