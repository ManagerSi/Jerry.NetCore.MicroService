using Consul;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Jerry.ServiceDiscovery.ServiceProvider
{
    public class ConsulServiceProvider:IMyServiceProvider
    {
        private readonly ConsulClient _consulClient;

        public ConsulServiceProvider(Uri uri)
        {
            _consulClient = new ConsulClient(client =>
            {
                client.Address = uri;
                client.Datacenter = "dc1";
            });
        }
        
        /// <summary>
        /// 
        /// </summary>
        /// <param name="serviceName"></param>
        /// <returns>key:url; value:weight</returns>
        public async Task<Dictionary<string,int>> GetServiceListAsync(string serviceName)
        {
            var services = new Dictionary<string, int>();

            var queryResult = await _consulClient.Health.Service(serviceName, tag: "", passingOnly: true);
            foreach (var item in queryResult.Response)
            {
                var weightStr = item.Service.Meta?["Weight"] ?? "1";
                int.TryParse(weightStr, out var weight);

                services.Add($"{item.Service.Address}:{item.Service.Port}",weight);
            }

            return services;
        }
    }
}
