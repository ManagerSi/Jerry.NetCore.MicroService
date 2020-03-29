using System;
using System.Collections.Generic;
using System.Linq;
using Consul;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting.Server.Features;
using Microsoft.AspNetCore.Http.Features;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Options;

namespace ConsulServiceRegistration
{
    // Consul配置模型类
    public static class ConsulRegistrationExtensions
    {
        public static void AddConsul(this IServiceCollection service, string jsonConfigPath)
        {
            // 读取服务配置文件
            var config = new ConfigurationBuilder().AddJsonFile(jsonConfigPath).Build();
            service.Configure<ConsulServiceOptions>(config);
        }

        public static IApplicationBuilder UseConsul(this IApplicationBuilder app)
        {
            // 获取服务配置项
            var serviceOptions = app.ApplicationServices.GetRequiredService<IOptions<ConsulServiceOptions>>().Value;
            // 服务ID必须保证唯一
            serviceOptions.ServiceId = Guid.NewGuid().ToString();

            //consul对象
            var consulClient = new ConsulClient(consulConfig => { consulConfig.Address = new Uri(serviceOptions.ConsulAddress); });


            // 使用参数配置服务注册地址
            var config = app.ApplicationServices.GetRequiredService<IConfiguration>();
            var address = "";//config["urls"];
            if (string.IsNullOrEmpty(address))
            {
                // 获取当前服务地址和端口
                var features = app.Properties["server.Features"] as FeatureCollection;
                address = features?.Get<IServerAddressesFeature>().Addresses.FirstOrDefault();
            }
            Console.WriteLine("--------------address:" + address);
            var uri = new Uri(address);

            var readom = new Random();
            // 节点服务注册对象
            var registration = new AgentServiceRegistration()
            {
                ID = serviceOptions.ServiceId,
                Name = serviceOptions.ServiceName,// 服务名
                Address = uri.Host,

                Port = uri.Port, // 服务端口
                Tags = new string[] {uri.Port.ToString()},
                Meta = new Dictionary<string, string>()
                {
                    { serviceOptions.ServiceName, serviceOptions.ServiceId }, 
                    { "Weight", readom.Next(10).ToString() }
                },

                Check = new AgentServiceCheck
                {
                    // 注册超时
                    Timeout = TimeSpan.FromSeconds(5),
                    // 服务停止多久后注销服务
                    DeregisterCriticalServiceAfter = TimeSpan.FromSeconds(5),
                    // 健康检查地址
                    HTTP = $"{uri.Scheme}://{uri.Host}:{uri.Port}{serviceOptions.HealthCheck}",
                    // 健康检查时间间隔
                    Interval = TimeSpan.FromSeconds(10),
                }
            };

            // 注册服务
            consulClient.Agent.ServiceRegister(registration).Wait();

            // 获取主机生命周期管理接口
            var lifetime = app.ApplicationServices.GetRequiredService<IHostApplicationLifetime>();
            // 应用程序终止时，注销服务
            lifetime.ApplicationStopping.Register(() =>
            {
                consulClient.Agent.ServiceDeregister(serviceOptions.ServiceId).Wait();
            });

            return app;

        }
    }
}
