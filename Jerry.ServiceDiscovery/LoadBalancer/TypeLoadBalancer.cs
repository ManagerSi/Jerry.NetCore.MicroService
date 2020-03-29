using System;
using System.Collections.Generic;
using System.Text;

namespace Jerry.ServiceDiscovery.LoadBalancer
{
    public static class TypeLoadBalancer
    {
        public static ILoadBalancer RandomLoad = new RandomLoadBalancer();
        public static ILoadBalancer RoundRobin = new RoundRobinLoadBalancer();
    }
}
