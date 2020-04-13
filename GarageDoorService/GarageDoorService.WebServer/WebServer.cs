namespace GarageDoorService.WebServer
{
    using Microsoft.AspNetCore;
    using Microsoft.AspNetCore.Hosting;
    using Microsoft.Extensions.DependencyInjection;
    using Shared;

    public class WebServer
    {
        public static void StartServer(IGarageDoorService service)
        {
            CreateWebHostBuilder(new string[0]).ConfigureServices(services =>
            {
                services.AddSingleton<IGarageDoorService>(service);
            }).UseUrls("http://*:5000").UseKestrel().Build().Run();
        }

        public static IWebHostBuilder CreateWebHostBuilder(string[] args) =>
            WebHost.CreateDefaultBuilder(args).UseStartup<Startup>();
    }
}
