using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Jerry.ServiceDiscovery.ServiceProvider
{
    public interface IMyServiceProvider
    {
        Task<Dictionary<string, int>> GetServiceListAsync(string serviceName);
    }
}
