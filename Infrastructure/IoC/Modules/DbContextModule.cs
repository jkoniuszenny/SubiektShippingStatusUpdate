using System;
using System.Collections.Generic;
using System.Configuration;
using System.Text;
using Autofac;
using Infrastructure.Database;
using Infrastructure.Settings;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Infrastructure.IoC.Modules
{
    public class DbContextModule : Module
    {
        private readonly IConfiguration _configuration;

        public DbContextModule(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        protected override void Load(ContainerBuilder builder)
        {

            var dat = _configuration.GetSection("Database").GetSection("ConnectionString").Value;

            var optionsBuilder = new DbContextOptionsBuilder<DatabaseContext>();
            optionsBuilder.UseSqlServer(dat);

            builder.RegisterType<DatabaseContext>()
               .WithParameter("options", optionsBuilder.Options)
               .InstancePerDependency();


        }
    }
}