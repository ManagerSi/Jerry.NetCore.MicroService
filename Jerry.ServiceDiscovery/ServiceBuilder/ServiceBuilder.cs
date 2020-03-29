using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Jerry.ServiceDiscovery.LoadBalancer;
using Jerry.ServiceDiscovery.ServiceProvider;

namespace Jerry.ServiceDiscovery.ServiceBuilder
{
    public class ServiceBuilder:IServiceBuilder
    {
        public IMyServiceProvider ServiceProvider { get; set; }
        public string ServiceName { get; set; }
        public string UriScheme { get; set; }
        public ILoadBalancer LoadBalancer { get; set; }

        public ServiceBuilder(IMyServiceProvider serviceProvider)
        {
            ServiceProvider = serviceProvider;
        }

        public async Task<Uri> BuildAsync(string path)
        {
            var serviceList = await ServiceProvider.GetServiceListAsync(ServiceName);
            var service = LoadBalancer.Resolve(serviceList);
            
            var baseUri = new Uri($"{UriScheme}://{service}");
            var uri = new Uri(baseUri, path);
            return uri;
        }
    }
}
