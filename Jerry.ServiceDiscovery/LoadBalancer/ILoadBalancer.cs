using System;
using System.Collections.Generic;
using System.Text;

namespace Jerry.ServiceDiscovery.LoadBalancer
{
    public interface ILoadBalancer
    {
        string Resolve(IDictionary<string, int> services);
    }
}
