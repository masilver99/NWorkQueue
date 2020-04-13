﻿using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.DependencyInjection;
using NWorkQueue.Common;
using NWorkQueue.Server.Common;
using NWorkQueue.Sqlite;
using System;
using System.Collections.Generic;
using System.Text;
using NWorkQueue.GrpcServer;
using NWorkQueue.GrpcServer.Interceptors;
using Microsoft.Extensions.Configuration;
using System.IO;

namespace NWorkQueue.Integration.Tests
{
    class StartupGrpc
    {
        /// <summary>
        /// This method gets called by the runtime. Use this method to add services to the container.
        /// For more information on how to configure your application, visit https://go.microsoft.com/fwlink/?LinkID=398940 .
        /// </summary>
        /// <param name="services">Service Collection passed in via runtime.</param>
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddGrpc(config =>
            {
                config.Interceptors.Add<ExceptionInterceptor>();

                //config.ResponseCompressionLevel = System.IO.Compression.CompressionLevel.Optimal;
            });

            services.AddSingleton<IStorage>(new StorageSqlite(@"Data Source=SqliteTesting.db;"));
            services.AddSingleton(typeof(InternalApi));

            var configBuilder = new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile("appsettings.json", optional: true);
            var config = configBuilder.Build();

            services.Configure<QueueOptions>(config);
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            app.UseRouting();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapGrpcService<NWorkQueueService>();
            });
        }
    }
}
