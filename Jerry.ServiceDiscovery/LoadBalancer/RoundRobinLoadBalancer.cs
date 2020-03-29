using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Jerry.ServiceDiscovery.LoadBalancer
{
   public class RoundRobinLoadBalancer : ILoadBalancer
    {
        private readonly object _lock = new object();
        private int _index = 0;
        public string Resolve(IDictionary<string, int> services)
        {
            if (services?.Count<=0)
            {
                return null;
            }

            // 使用lock控制并发
            lock (_lock)
            {
                if (_index >= services.Count)
                {
                    _index = 0;
                }
                return services.ElementAt(_index++).Key;
            }
        }

        private int growIndex = 0;
        public string other(IDictionary<string, int> services)
        {
            if (services?.Count <= 0)
            {
                return null;
            }

            // 使用lock控制并发
            lock (_lock)
            {
                return services.ElementAt((growIndex++) % services.Count).Key;
            }
        }
    }
}
