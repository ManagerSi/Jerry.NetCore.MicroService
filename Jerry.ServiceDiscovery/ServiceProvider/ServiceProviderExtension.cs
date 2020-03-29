using Jerry.ServiceDiscovery.ServiceBuilder;
using System;
using System.Collections.Generic;
using System.Text;

namespace Jerry.ServiceDiscovery.ServiceProvider
{
    public static class ServiceProviderExtension
    {
        public static IServiceBuilder CreateServiceBuilder(this IMyServiceProvider serviceProvider,
            Action<IServiceBuilder> config)
        {

            var builder = new ServiceBuilder.ServiceBuilder(serviceProvider);
            config(builder);
            return builder;
        }
    }
}
