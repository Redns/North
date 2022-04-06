using ImageBed.Data;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;
using ImageBed.Data.Access;

var builder = WebApplication.CreateBuilder(args);


/// <summary>
/// 向容器中注入服务
/// </summary>
builder.Services.AddAntDesign();
builder.Services.AddControllers();
builder.Services.AddRazorPages();
builder.Services.AddHttpClient();
builder.Services.AddServerSideBlazor();
builder.Services.AddDbContext<OurDbContext>();


var app = builder.Build();


app.UseHttpsRedirection();
app.Urls.Add("http://0.0.0.0:12121");
app.UseStaticFiles();

/// <summary>
/// 启用路由
/// </summary>
app.UseRouting();
app.MapControllers();
app.MapBlazorHub();
app.MapFallbackToPage("/_Host");

app.Run();