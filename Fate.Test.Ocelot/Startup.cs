﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Fate.Common.OcelotStore.EFCore;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Ocelot.DependencyInjection;
using Ocelot.Middleware;
using Microsoft.Extensions.Options;
using Ocelot.Cache;
using Ocelot.Configuration.File;
using Autofac;
using Autofac.Extensions.DependencyInjection;

namespace Fate.Test.Ocelot
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }
        public IConfiguration Configuration { get; }
        // This method gets called by the runtime. Use this method to add services to the container.
        // For more information on how to configure your application, visit https://go.microsoft.com/fwlink/?LinkID=398940
        public IServiceProvider ConfigureServices(IServiceCollection services)
        {
            services.AddOcelot().AddEFCache(options =>
            {
                options.EFOptions = ef => ef.ConfigureDbContext = context => context.UseMySql(Configuration.GetConnectionString("OcelotMysqlConnection"));
            });
            //替换自带的DI
            ContainerBuilder builder = new ContainerBuilder();
            builder.Populate(services);
            return new AutofacServiceProvider(builder.Build());
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public async void Configure(IApplicationBuilder app, IHostingEnvironment env)
        {
            //调用此中间件 重新设置网关的配置信息
            app.ResetEFConfiguration(new PathString("/test"));
            await app.UseOcelot();

        }
    }
}
