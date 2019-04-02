using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Common.Redis;
using Microsoft.AspNetCore;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.SignalR;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using RedisStudy.Hub;

namespace RedisStudy.MVC
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var host = CreateWebHostBuilder(args).Build();
            string connectstr = new ConfigurationBuilder()
                                .SetBasePath(Directory.GetCurrentDirectory())
                                .AddJsonFile("appsettings.json", optional: true, reloadOnChange: true)
                                .Build().GetSection("RedisServer")["Url"];
            RedisHelper redisHelper = new RedisHelper(connectstr, 1);

            redisHelper.Subscribe("SSS", (channel, value) => {
                using (var serviceScope = host.Services.CreateScope())
                {
                    var services = serviceScope.ServiceProvider;

                    try
                    {
                        string v = value;
                        var serviceContext = services.GetRequiredService<IHubContext<MessageHub>>();
                        // Use the context here
                        Task t = serviceContext.Clients.All.SendAsync("OnMessage", v);
                    }
                    catch (Exception ex)
                    {
                        var logger = services.GetRequiredService<ILogger<Program>>();
                        logger.LogError(ex, "An error occurred.");
                    }
                }

            });
            host.Run();
        }

        public static IWebHostBuilder CreateWebHostBuilder(string[] args) =>
            WebHost.CreateDefaultBuilder(args)
                .UseStartup<Startup>();
    }
}
