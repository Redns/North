using MudBlazor.Services;

class Program
{
    public static void Main(string[] args)
    {
        // 创建容器并注入服务
        var builder = WebApplication.CreateBuilder(args);

        builder.Services.AddRazorPages();
        builder.Services.AddMudServices();
        builder.Services.AddServerSideBlazor();

        // 构建应用
        var app = builder.Build();

        app.UseRouting();
        app.UseStaticFiles();
        
        app.MapBlazorHub();
        app.MapFallbackToPage("/_Host");
        app.UseExceptionHandler("/Error");
        app.Urls.Add("http://0.0.0.0:12121");

        app.Run();
    }
}