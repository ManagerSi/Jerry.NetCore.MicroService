using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Jerry.ServiceDiscovery.LoadBalancer
{
   public class RandomLoadBalancer:ILoadBalancer
    {
        private readonly  Random _random = new Random();
        public string Resolve(IDictionary<string, int> services)
        {
            if (services?.Count>0)
            {
                var index = _random.Next(0, services.Count);
                return services.ElementAt(index).Key;
            }

            return null;
        }
    }
}
