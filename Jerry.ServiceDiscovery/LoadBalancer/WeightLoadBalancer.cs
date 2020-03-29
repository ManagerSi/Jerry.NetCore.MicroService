using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Jerry.ServiceDiscovery.LoadBalancer
{
   public class WeightLoadBalancer : ILoadBalancer
    {
        private readonly Random _random = new Random();

        /// <summary>
        /// 
        /// </summary>
        /// <param name="services">key:url, value:weight</param>
        /// <returns></returns>
        public string Resolve(IDictionary<string, int> services)
        {
            if (services?.Count<=0)
            {
                return null;
            }

            var serviceList = new List<string>();
            foreach (var item in services)
            {
                for (int i = 0; i < item.Value; i++)
                {
                    serviceList.Add(item.Key);
                }
            }

            var index = _random.Next(0, serviceList.Count);
            return serviceList[index];
        }
    }
}
