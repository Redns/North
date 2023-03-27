using Krins.Nuget;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Mvc.ApplicationParts;
using Microsoft.AspNetCore.Mvc.Infrastructure;
using MudBlazor;
using MudBlazor.Services;
using North.Common;
using North.Core.Services.Logger;
using North.Core.Services.Poster;
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
            // ��������
            builder = WebApplication.CreateBuilder(args);

            builder.Services.AddRazorPages();
            builder.Services.AddControllers();
            builder.Services.AddHttpContextAccessor();
            builder.Services.AddMudServices(config =>
            {
                // Snackbar ����
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
                // ��δ������쳣����ʱ���Ƿ�����ϸ�Ĵ�����Ϣ�� Javascript
                // ��������Ӧ���رգ�������ܱ�©������Ϣ
                option.DetailedErrors = false;
                // JS �����ó�ʱ�¼�����
                option.JSInteropDefaultCallTimeout = TimeSpan.FromSeconds(10);
            });
            // ���ݿ��������
            builder.Services.AddScoped<ISqlSugarClient>(client =>
            {
                return new SqlSugarClient(Array.ConvertAll(GlobalValues.AppSettings.General.DataBase.Databases, db => new ConnectionConfig
                {
                    ConfigId = db.Name,
                    DbType = db.Type,
                    ConnectionString = db.ConnectionString,
                    IsAutoCloseConnection = db.IsAutoCloseConnection,
                }).ToList());
            });
            builder.Services.AddLogging(logging =>
            {
                // ���Ĭ����־���
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
                                // TODO �����Զ���ת
                                // ȷ�����ڴ��� Cookie ������
                                options.Cookie = new CookieBuilder()
                                {
                                    // SameSite �����û����Ƶ����� Cookie�����ٰ�ȫ���գ���������ֵ���ο���http://www.ruanyifeng.com/blog/2019/09/cookie-samesite.html��
                                    // Strict: ��ȫ��ֹ������ Cookie����վ�㣨����������������ͬ��ʱ���ᷢ�� Cookie
                                    // Lax: ��������Ҳ�ǲ����͵����� Cookie�����ǵ�����Ŀ����ַ�� Get �������
                                    // None: ��ʽ�ر� SameSite ���ԣ�������ͬʱ���� Secure ���ԣ�Cookie ֻ��ͨ�� HTTPS Э�鷢�ͣ�
                                    SameSite = SameSiteMode.Lax,
                                    // ����Ϊ false ����ֹ JS ���֡��޸� Cookie
                                    HttpOnly = true,
                                    // �����ṩ Cookie �� URI �����ͣ�HTTP/HTTPS��������������ʱ������ʱЯ�� Cookie����������ֵ
                                    // Always: Ҫ���¼ҳ��֮��������Ҫ�����֤��ҳ���Ϊ HTTPS
                                    // None: ��¼ҳΪ HTTPS�������� HTTP ҳҲ��Ҫ�����֤��Ϣ
                                    // SameAsRequest: ���ṩ Cookie �� URI Ϊ HTTPS����ֻ���ں��� HTTPS �����Ͻ� Cookie ���ط����������ṩ Cookie �� URI Ϊ HTTP������ں��� HTTP �� HTTPS �����Ͻ� Cookie ���ط�������
                                    SecurePolicy = CookieSecurePolicy.SameAsRequest
                                };
                                options.Events = new CookieAuthenticationEvents
                                {
                                    
                                };
                            });
            builder.Services.AddSingleton<IPoster, MineKitPoster>(poster => new MineKitPoster());
            builder.Services.AddSingleton(loginIdentify => new Dictionary<string, ClaimsIdentity>());
            // ��̬���ؿ�����
            builder.Services.AddSingleton(NorthActionDescriptorChangeProvider.Instance);
            builder.Services.AddSingleton<IActionDescriptorChangeProvider>(NorthActionDescriptorChangeProvider.Instance);
            // Nuget ����
            builder.Services.AddSingleton(nugetEngine => new NugetEngine());
            // �������
            builder.Services.AddSingleton(pluginContext =>
            {
                var logger = app.Services.GetRequiredService<ILogger>();
                var applicationPartManager = app.Services.GetRequiredService<ApplicationPartManager>();
                return new PluginsContext(applicationPartManager, NorthActionDescriptorChangeProvider.Instance)
                {
                    OnRefreshControllers = (applicationParts) =>
                    {
                        NorthActionDescriptorChangeProvider.Instance.HasChanged = true;
                        NorthActionDescriptorChangeProvider.Instance.TokenSource.Cancel();
                    }
                };
            });


            // ���� web Ӧ��
            app = builder.Build();
            // TODO ����м��
            // app.Use(PluginsContext.Middlewares);
            app.UseStaticFiles();
            app.UseRouting();
            app.UseAuthentication();
            app.UseAuthorization();
            app.UseEndpoints(endpoint =>
            {
                if (!GlobalValues.IsApplicationInstalled)
                {
                    // δ��װʱǿ����ת����װҳ��
                    endpoint.MapGet("/", context =>
                    {
                        return Task.Run(() =>
                        {
                            context.Response.Redirect("/install");
                        });
                    });
                }
                else
                {
                    // Ӧ���Ѱ�װ���޷����ʰ�װҳ��
                    endpoint.MapGet("/install", context =>
                    {
                        return Task.Run(() =>
                        {
                            context.Response.Redirect("/");
                        });
                    });
                }
                endpoint.MapBlazorHub();
                endpoint.MapControllers();
                endpoint.MapFallbackToPage("/_Host");
            });
            // TODO ����ҳ�����Ӷ˿��޸Ĺ���
            app.Urls.Add(GlobalValues.AppSettings.General.ApplicationUrl);

            app.Run();
        }
    }
}