using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.IO;

namespace MikaDiskCleaner
{
    class Program
    {
        static void Main(string[] args)
        {
            if (args.Length == 0 ||args == null)
            {
                args = new string[] { @"C:\Users\mgo\Desktop\Folder\TestFolder" };
            }

            var services = ConfigureServices();

            var serviceProvider = services.BuildServiceProvider();

            serviceProvider.GetService<DiskCleaner>().Run(args[0]);

        }

        public static IServiceCollection ConfigureServices()
        {
            IServiceCollection services = new ServiceCollection();

            var config = LoadConfiguration();

            services.AddSingleton(config);

            services.AddTransient<DiskCleaner>();
            services.AddTransient<FolderSorter>();

            return services;
        }

        static IConfiguration LoadConfiguration()
        {
            var builder = new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile("appsettings.json", optional: true, reloadOnChange: true);

            IConfigurationRoot configuration = builder.Build();
            Console.WriteLine(configuration.AsEnumerable());
            return configuration;
        }





    }


}
