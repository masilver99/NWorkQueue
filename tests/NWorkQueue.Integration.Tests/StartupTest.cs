﻿using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.DependencyInjection;
using NWorkQueue.Common;
using NWorkQueue.Server;
using NWorkQueue.Sqlite;
using ProtoBuf.Grpc.Server;
using System;
using System.Collections.Generic;
using System.Text;

namespace NWorkQueue.Integration.Tests
{
    class StartupTest
    {
        /// <summary>
        /// This method gets called by the runtime. Use this method to add services to the container.
        /// For more information on how to configure your application, visit https://go.microsoft.com/fwlink/?LinkID=398940 .
        /// </summary>
        /// <param name="services">Service Collection passed in via runtime.</param>
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddCodeFirstGrpc(config =>
            {
                config.ResponseCompressionLevel = System.IO.Compression.CompressionLevel.Optimal;
            });

            services.AddSingleton<IStorage>(new StorageSqlLite(@"Data Source=SqlLiteTesting.db;"));
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            app.UseRouting();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapGrpcService<QueueApi>();
            });
        }
    }
}
