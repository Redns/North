using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.EntityFrameworkCore;
using North.Data.Access;
using North.Models.Auth;
using North.Services.Storage;

class Program
{
    public static void Main(string[] args)
    {
        // 创建容器并注入服务
        var builder = WebApplication.CreateBuilder(args);

        builder.Services.AddAntDesign();
        builder.Services.AddControllers();
        builder.Services.AddRazorPages();
        builder.Services.AddHttpClient();
        builder.Services.AddServerSideBlazor(option =>
        {
            option.DetailedErrors = false;
        });
        builder.Services.AddDbContext<OurDbContext>();
        builder.Services.AddHttpContextAccessor();
        builder.Services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
                        .AddCookie(options =>
                        {
                            options.ExpireTimeSpan = TimeSpan.FromDays(3);
                        });
        builder.Services.AddSingleton<IStorage<UnitLoginIdentify>, MemoryStorage<UnitLoginIdentify>>(identifies => new MemoryStorage<UnitLoginIdentify>());

        // 构建应用
        var app = builder.Build();

        app.UseRouting();
        app.UseAuthentication();
        app.UseAuthorization();
        app.UseStaticFiles();
        app.MapBlazorHub();
        app.MapControllers();
        app.MapFallbackToPage("/_Host");
        app.Urls.Add("http://0.0.0.0:12121");

        app.Run();
    }
}