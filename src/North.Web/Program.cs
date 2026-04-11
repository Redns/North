using Blazored.LocalStorage;
using Microsoft.AspNetCore.Components.Web;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using MudBlazor;
using MudBlazor.Services;
using North.Web.Layout;
using North.Web.Services;
using Toolbelt.Blazor.Extensions.DependencyInjection;

namespace North.Web
{
    public class Program
    {
        public static async Task Main(string[] args)
        {
            var builder = WebAssemblyHostBuilder.CreateDefault(args);
            builder.RootComponents.Add<App>("#app");
            builder.RootComponents.Add<HeadOutlet>("head::after");

            builder.Services.AddHttpClient<MainLayout>(
                (services, client) =>
                {
                    client.BaseAddress = new Uri(builder.HostEnvironment.BaseAddress);
                }
            );
            builder.Services.AddScoped(sp => new HttpClient
            {
                BaseAddress = new Uri(builder.HostEnvironment.BaseAddress),
            });
            builder.Services.AddMudServices(config =>
            {
                config.SnackbarConfiguration.PositionClass = Defaults.Classes.Position.TopCenter;
                config.SnackbarConfiguration.PreventDuplicates = true;
                config.SnackbarConfiguration.SnackbarVariant = Variant.Text;
                config.SnackbarConfiguration.ShowCloseIcon = false;
                config.SnackbarConfiguration.VisibleStateDuration = 1500;
                config.SnackbarConfiguration.HideTransitionDuration = 200;
                config.SnackbarConfiguration.ShowTransitionDuration = 200;
            });
            builder.Services.AddPWAUpdater();
            builder.Services.AddBlazoredLocalStorage();
            builder.Services.AddScoped<AppThemeState>();

            await builder.Build().RunAsync();
        }
    }
}
