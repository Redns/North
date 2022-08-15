using Microsoft.AspNetCore.Authentication.Cookies;
using MudBlazor;
using MudBlazor.Services;
using NLog.Extensions.Logging;
using North.Common;
using North.Core.Data.Access;
using North.Services.Logger;
using ILogger = North.Services.Logger.ILogger;

class Program
{
    private static WebApplication? app;
    private static WebApplicationBuilder? builder;
    private static readonly ILogger logger = new NLogger(GlobalValues.AppSettings.Log);

    static void Main(string[] args)
    {
        try
        {
            logger.Info("Application launching...");

            // 绑定应用程序域事件
            logger.Info("AppDomain event binding...");
            AppDomain.CurrentDomain.ProcessExit += OnProcessExit;
            AppDomain.CurrentDomain.UnhandledException += OnHandleExpection;
            logger.Info("AppDomain event bind success");

            // 创建容器
            builder = WebApplication.CreateBuilder(args);

            logger.Info("Application building...");
            builder.Services.AddRazorPages();
            builder.Services.AddHttpClient();
            builder.Services.AddControllers();
            builder.Services.AddHttpContextAccessor();
            builder.Services.AddLogging(logger =>
            {
                logger.ClearProviders();
                logger.AddDebug();
                logger.AddEventSourceLogger();
                logger.AddNLog();
            });
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
            builder.Services.AddScoped(context => new OurDbContext(GlobalValues.AppSettings.Storage.DataBase.ConnStr));
            builder.Services.AddSingleton<ILogger, NLogger>(logger => new NLogger(GlobalValues.AppSettings.Log));
            builder.Services.AddServerSideBlazor(option =>
            {
                option.DetailedErrors = false;
            });
            builder.Services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
                            .AddCookie(options =>
                            {
                                options.ExpireTimeSpan = TimeSpan.FromDays(3);
                            });
            logger.Info("Application build success");

            // 构建 web 应用
            app = builder.Build();

            app.UseRouting();
            app.UseAuthentication();
            app.UseAuthorization();
            app.UseStaticFiles();
            app.MapBlazorHub();
            app.MapControllers();
            app.MapFallbackToPage("/_Host");
            // TODO 修改绑定的 URL
            app.Urls.Add("http://0.0.0.0:12121");

            logger.Info("Application launch success");

            app.Run();
        }
        catch(Exception e)
        {
            logger.Error("Application abort", e);
        }
    }


    /// <summary>
    /// 处理未被捕获的异常
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="args"></param>
    static void OnHandleExpection(object sender, UnhandledExceptionEventArgs args)
    {
        
    }


    /// <summary>
    /// 处理应用退出事件
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="args"></param>
    static void OnProcessExit(object? sender, EventArgs args)
    {
        // 此处所有注入的服务均会被释放，因此无法获取 ILogger 实例打印日志
        // 重新创建 NLogger 实例也无法使用，因为其是从 LogManager 获取的
        // KLogger 基于 FileStream 实现，不受影响 
        using var logger = new KLogger(GlobalValues.AppSettings.Log);

        // 同步本地数据库
        logger.Info("Database syncing...");
        GlobalValues.MemoryDatabase.SyncDatabase(GlobalValues.AppSettings.Storage.DataBase.ConnStr);
        logger.Info("Database sync success");

        logger.Info("Application exit");
    }
}