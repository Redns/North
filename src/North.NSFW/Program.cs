using NsfwSpyNS;

namespace North.NSFW
{
    class Program
    {
        static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            // Add services to the container.
            builder.Services.AddControllers();
            builder.Services.AddScoped<INsfwSpy, NsfwSpy>(nsfw => new NsfwSpy());

            var app = builder.Build();

            // Configure the HTTP request pipeline.
            app.UseHttpsRedirection();
            app.UseAuthorization();
            app.Urls.Add("http://0.0.0.0:12122");
            app.MapControllers();

            app.Run();
        }
    }
}