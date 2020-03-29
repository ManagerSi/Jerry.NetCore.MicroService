using Jerry.ServiceDiscovery.ServiceProvider;
using System;
using System.Net.Http;
using System.Threading.Tasks;
using Jerry.ServiceDiscovery.LoadBalancer;

namespace Jerry.ServiceCustomer
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("service test!");

            var serviceProvider = new ConsulServiceProvider(new Uri("http://127.0.0.1:8500"));
            var myServiceA = serviceProvider.CreateServiceBuilder(builder =>
            {
                builder.ServiceName = "MyServiceA";
                // 指定负载均衡器
                builder.LoadBalancer = TypeLoadBalancer.RoundRobin;
                // 指定Uri方案
                builder.UriScheme = Uri.UriSchemeHttp;
            });

            //test

            // Polly如果再微服务里，主要是用在跨服务调用上，虽然他很很牛B，但还是把它当做try catch
            // 关于这节课，大家有收获 回复个666
            // Polly有BUG？你也可以点用其他API
            // Polly可以.NET Core里的HTTPFactory集合使用，用起来更爽了
            for (int i = 0; i < 100; i++)
            {
                Console.WriteLine($"-------------第{i}次请求-------------");
                try
                {
                    var uri = myServiceA.BuildAsync("health").Result;
                    Console.WriteLine($"{DateTime.Now} - 正在调用：{uri}");
                    var content = InvokeApi(uri.AbsoluteUri);
                    Console.WriteLine($"调用结果：{content}");
                }
                catch (Exception e)
                {
                    Console.WriteLine($"调用异常：{e.GetType()}");
                }
                Task.Delay(1000).Wait();
            }
            
        }



        public static string InvokeApi(string url)
        {
            using (HttpClient httpClient = new HttpClient())
            {
                HttpRequestMessage message = new HttpRequestMessage();
                message.Method = HttpMethod.Get;
                message.RequestUri = new Uri(url);
                var result = httpClient.SendAsync(message).Result;
                string content = result.Content.ReadAsStringAsync().Result;
                return content;
            }
        }
    }
}
