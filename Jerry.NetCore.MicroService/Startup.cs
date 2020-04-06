using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using ConsulServiceRegistration;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Microsoft.OpenApi.Models;
using StackExchange.Redis;

namespace Jerry.NetCore.MicroService
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            #region swagger

            services.AddSwaggerGen(s =>
            {
                s.SwaggerDoc("V1",new OpenApiInfo()
                {
                    Title ="coreWebApi",
                    Version = "v1"
                });
            });
            #endregion
            services.AddControllersWithViews();

            services.AddHealthChecks();
            services.AddConsul(Path.Combine( AppDomain.CurrentDomain.BaseDirectory, "consulsettings.json"));

            var redis = Configuration["RedisConnection"];
            services.AddSession();
            services.AddDistributedRedisCache(options =>
            {
                options.InstanceName = "NetMicroServiceRedis";
                options.Configuration = "127.0.0.1:6379"; //Configuration[""];
            });//redis 分布式缓存
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env, ILoggerFactory loggerFactory, IOptions<ConsulServiceOptions> serviceOptions)
        {
            loggerFactory.AddLog4Net();

            app.UseSession();

            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            else
            {
                app.UseExceptionHandler("/Home/Error");
                // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
                app.UseHsts();
            }

            app.UseHealthChecks(serviceOptions.Value.HealthCheck);
            app.UseConsul();

            app.UseHttpsRedirection();
            app.UseStaticFiles();

            app.UseRouting();

            app.UseAuthentication();
            app.UseAuthorization();

            //访问方式：Ip:端口/swagger/index.html
            app.UseSwagger();
            app.UseSwaggerUI(s =>
            {
                //V1 必须和上面Doc的V1相同
                s.SwaggerEndpoint(
                    "/swagger/V1/swagger.json",
                    "Api" //Select a definition
                    );
            });

            app.UseEndpoints(endpoints =>
            {
               endpoints.MapControllerRoute(
                    name: "default",
                    pattern: "{controller=Home}/{action=Index}/{id?}");
               
            });
        }
    }
}
