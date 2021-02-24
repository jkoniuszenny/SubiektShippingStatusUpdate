using Autofac;
using Infrastructure.Database;
using Infrastructure.IoC;
using Infrastructure.Services.Interfaces;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.ComponentModel;
using System.Threading.Tasks;

namespace SubiektShippingStatusUpdate
{
    class Program
    {
        public static IConfigurationRoot Configuration;
        public static Autofac.IContainer ApplicationContainer;

        static async Task Main(string[] args)
        {
            var builder = new ConfigurationBuilder()
                .AddJsonFile("appsettings.json", optional: true, reloadOnChange: true)
                .AddEnvironmentVariables()
                .AddCommandLine(args);

            Configuration = builder.Build();


            var collection = new ServiceCollection();
            var conBuilder = new ContainerBuilder();
            conBuilder.RegisterModule(new ContainerModule(Configuration));

            ApplicationContainer = conBuilder.Build();

            var scope = ApplicationContainer.BeginLifetimeScope();

         
            var plugin = scope.Resolve<IStartService>();

            
            await plugin.RunProgram();
        }
    }
}
