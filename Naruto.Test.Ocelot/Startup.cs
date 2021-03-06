﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Naruto.OcelotStore.EFCore;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Ocelot.DependencyInjection;
using Ocelot.Middleware;
using Autofac;
using Autofac.Extensions.DependencyInjection;

namespace Naruto.Test.Ocelot
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
            services.AddSingleton<Test2DefinedAggregator>();
            services.AddOcelot()
                .AddOcelotEFCache(options =>
            {
                options.EFOptions = ef => ef.ConfigureDbContext = context => context.UseMySql(Configuration.GetConnectionString("OcelotMysqlConnection"));
            }).AddRedisProvider(option =>
            {
                option.RedisOptions = redis =>
                {
                    redis.Connection = new string[] { "127.0.0.1:6379" };
                    redis.DefaultDataBase = 2;
                };
            }).AddSingletonDefinedAggregator<Test2DefinedAggregator>();//添加一个聚合器 用于请求聚合的时候

            //替换自带的DI
            ContainerBuilder builder = new ContainerBuilder();
            builder.Populate(services);
            return new AutofacServiceProvider(builder.Build());
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env)
        {
            app.UseOcelot().GetAwaiter().GetResult();
        }
    }
}
