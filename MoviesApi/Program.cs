using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Hosting;
using MoviesApi.Common;

namespace MoviesApi
{
    public class Program
    {
        public static void Main(string[] args)
        {
            Log.Default.Debug("Application started");
            CreateHostBuilder(args).Build().Run();
        }

        public static IHostBuilder CreateHostBuilder(string[] args) =>
            Host.CreateDefaultBuilder(args)
                .ConfigureWebHostDefaults(webBuilder =>
                {
                    webBuilder.UseStartup<Startup>();
                });
    }
}
