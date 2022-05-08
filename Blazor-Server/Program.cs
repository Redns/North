using Blazored.LocalStorage;
using ImageBed.Common;
using ImageBed.Data.Access;
using ImageBed.Data.Entity;
using NLog.Extensions.Logging;


var builder = WebApplication.CreateBuilder(args);


/// <summary>
/// 加载本地设置，启动系统资源记录定时器
/// </summary>
GlobalValues.appSetting = AppSetting.Parse();
GlobalValues.InitSysRecordTimer();


/// <summary>
/// 向容器中注入服务
/// </summary>
builder.Services.AddAntDesign();
builder.Services.AddControllers();
builder.Services.AddRazorPages();
builder.Services.AddHttpClient();
builder.Services.AddServerSideBlazor();
builder.Services.AddDbContext<OurDbContext>();

// Nlog日志服务
builder.Services.AddLogging(logger =>
{
    logger.ClearProviders();
    logger.AddConsole();
    logger.AddDebug();
    logger.AddEventSourceLogger();
    logger.AddNLog();
});

// LocalStorage鉴权服务
builder.Services.AddBlazoredLocalStorage();


/// <summary>
/// 启用组件
/// </summary>
var app = builder.Build();

app.UseStaticFiles();
app.Urls.Add("http://0.0.0.0:12121");

app.UseRouting();
app.MapControllers();
app.MapBlazorHub();
app.MapFallbackToPage("/_Host");

app.Run();