using Blazored.LocalStorage;
using ImageBed.Common;
using ImageBed.Data.Access;
using NLog.Extensions.Logging;

if((GlobalValues.AppSetting = AppSetting.Parse()) != null)
{
    var builder = WebApplication.CreateBuilder(args);

    builder.Services.AddAntDesign();
    builder.Services.AddControllers();
    builder.Services.AddRazorPages();
    builder.Services.AddHttpClient();
    builder.Services.AddServerSideBlazor();
    builder.Services.AddDbContext<OurDbContext>();

    // LocalStorage本地存储服务
    builder.Services.AddBlazoredLocalStorage();

    // Nlog日志服务
    builder.Services.AddLogging(logger =>
    {
        logger.ClearProviders();
        logger.AddConsole();
        logger.AddDebug();
        logger.AddEventSourceLogger();
        logger.AddNLog();
    });


    var app = builder.Build();

    app.UseStaticFiles();
    app.UseRouting();
    app.MapBlazorHub();
    app.MapControllers();
    app.MapFallbackToPage("/_Host");
    app.Urls.Add("http://0.0.0.0:12121");

    app.Run();
}