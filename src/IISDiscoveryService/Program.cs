using System;
using System.Diagnostics;
using System.IO;
using System.Linq;
using Autofac.Extensions.DependencyInjection;
using Microsoft.AspNetCore;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;

namespace IISDiscoveryService
{
    public class Program
    {
        public IConfiguration Configuration { get; }

        public static void Main(string[] args)
        {
            bool isService = true;
            if (Debugger.IsAttached || args.Contains("--console"))
            {
                isService = false;
            }

            var pathToContentRoot = Directory.GetCurrentDirectory();
            if (isService)
            {
                var pathToExe = Process.GetCurrentProcess().MainModule.FileName;
                pathToContentRoot = Path.GetDirectoryName(pathToExe);
            }

            var configuration = new ConfigurationBuilder()
                .AddJsonFile("appsettings.json")
                .AddCommandLine(args)
                .Build();

            var host = BuildWebHost(args, configuration, pathToContentRoot);

            host.Run();
        }

        public static IWebHost BuildWebHost(string[] args, IConfigurationRoot configuration, string pathToContentRoot) =>
            WebHost.CreateDefaultBuilder(args)
                .UseStartup<Startup>()
                .UseContentRoot(pathToContentRoot)
                .ConfigureServices(services => services.AddAutofac())
                .UseUrls(configuration["Host"])
                .Build();
    }
}
