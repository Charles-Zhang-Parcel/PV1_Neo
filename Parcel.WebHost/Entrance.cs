using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Parcel.Shared;
using Parcel.WebHost.Models;
using Parcel.WebHost.Utils;

namespace Parcel.WebHost
{
    public static class Entrance
    {
        public static void SetupAndRunWebHost()
        {
            var config = new ApplicationConfiguration();
            config.InitializeDefault();
            SetupAndRunWebHost(config);
        }

        public static void SetupAndRunWebHost(ApplicationConfiguration configuration)
        {
            // Configure web host explicitly
            int port = configuration.ServerPort ?? NetworkHelper.FindFreeTcpPort();
            string hostAddress = $"http://{configuration.ServerAddress ?? "localhost"}:{port}";
            if (WebHostRuntime.Singleton == null)
                new WebHostRuntime()
                {
                    Port = port,
                    Address = hostAddress,
                    ShouldLog = configuration.ServerDebugPrint,
                };

            new Thread(() =>
            {
                Thread.CurrentThread.IsBackground = true;

                // Build host
                IHost host = Host.CreateDefaultBuilder()
                    .ConfigureWebHostDefaults(webBuilder =>
                    {
                        webBuilder.UseUrls(WebHostRuntime.Singleton.Address);
                        // webBuilder.UseWebRoot("wwwroot"); // TODO: Potential Path-Breaking
                        webBuilder.UseKestrel();
                        webBuilder.UseStartup<Startup>();
                    })
                    .ConfigureLogging((context, logging) =>
                    {
                        logging.ClearProviders();
                        if (WebHostRuntime.Singleton.ShouldLog)
                        {
                            logging.AddConsole();
                            logging.AddDebug();
                        }
                    })
                    .UseContentRoot(Directory.GetCurrentDirectory())    // TODO: Potential Path-Breaking; Might require manual copying
                    .Build();
                // Start host
                host.Run();
            }).Start();
        }
    }
}