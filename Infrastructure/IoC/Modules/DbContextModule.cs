using System;
using System.Collections.Generic;
using System.Text;
using Autofac;
using Infrastructure.Database;
using Infrastructure.Settings;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.IoC.Modules
{
    public class DbContextModule : Module
    {
       
        protected override void Load(ContainerBuilder builder)
        {
            var optionsBuilder = new DbContextOptionsBuilder<DatabaseContext>();
            optionsBuilder.UseSqlServer("Server=localhost; Database=neopak2; Trusted_Connection=True; MultipleActiveResultSets=true");

            builder.RegisterType<DatabaseContext>()
               .WithParameter("options", optionsBuilder.Options)
               .InstancePerDependency();


        }
    }
}