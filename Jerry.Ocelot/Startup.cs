using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Ocelot.Cache;
using Ocelot.Cache.CacheManager;
using Ocelot.DependencyInjection;
using Ocelot.Middleware;
using Ocelot.Provider.Consul;
using Ocelot.Provider.Polly;

namespace Jerry.Ocelot
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
            //var config = new ConfigurationBuilder().AddJsonFile("ocelot2.json").Build();
            //services.AddOcelot(config);

            //with consul
            var config = new ConfigurationBuilder().AddJsonFile("ocelot-consul.json").Build();
            services.AddOcelot(config).AddConsul()
                .AddCacheManager(c =>  //����
                {
                    c.WithDictionaryHandle(); //ʹ��Ĭ���ֵ�洢
                })
                .AddPolly()
                ;
            ////�����Զ��建��CustomerCache
            //services.AddSingleton<IOcelotCache<CachedResponse>, CustomerCache>();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            app.UseOcelot().Wait();
        }
    }
}
