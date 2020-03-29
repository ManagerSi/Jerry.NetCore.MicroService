using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;
using Microsoft.Extensions.Caching.Memory;
using Polly;
using Polly.Caching;
using Polly.Caching.MemoryCache;
using Polly.CircuitBreaker;
using Polly.Timeout;

namespace PollyDemo
{
    class Program
    {
        static void Main(string[] args)
        { 
            // // 三步
            // // 1. 定义故障，当发生了ArgumentException异常的时候，就会触发策略
            // // 2. 指定策略
            // // 3. 执行策略
            // Policy.Handle<Exception>()
            //     .Fallback(() =>
            //     {
            //         Console.WriteLine("Polly Fallback!");
            //     })
            //     .Execute(() =>
            //     {
            //         // 业务，跨服务的调用（是可以和HttpClient结合使用）
            //         Console.WriteLine("DoSomething");
            //         throw new ArgumentException("Hello Polly");
            //     });
            //
            // // 单个异常的故障
            // Policy.Handle<Exception>();
            //
            // // 带条件的异常类型
            // Policy.Handle<Exception>(ex => ex.Message == "Hello");
            //
            // // 多个异常类型
            // Policy.Handle<HttpRequestException>()
            //     .Or<ArgumentException>()
            //     .Or<AggregateException>();
            //
            // // 多个异常类型带条件
            // Policy.Handle<HttpRequestException>(ex => ex.Message == "11")
            //     .Or<ArgumentException>()
            //     .Or<AggregateException>();
            //
            // // Polly故障处理库，有些异常你需要处理，服务A 访问 服务B（请求出错）
            // // 业务代码出了问题（还需要策略？异常捕捉还是要的，记录日志）
            //
            // // 弹性策略 响应性策略，重试、断路器
            // // 默认重试一次
            // Policy.Handle<Exception>().Retry();
            // // 重试N次
            // Policy.Handle<Exception>().Retry(3 );
            // // 一直重试，直到成功，非高并发
            // Policy.Handle<Exception>().RetryForever();
            //
            // // 重试且等待
            // Policy.Handle<Exception>().WaitAndRetry(new []
            // {
            //     TimeSpan.FromSeconds(1),
            //     TimeSpan.FromSeconds(2),
            //     TimeSpan.FromSeconds(3),
            // });
            //
            // Policy.Handle<Exception>().WaitAndRetry(3, i => TimeSpan.FromSeconds(Math.Pow(2, i)));
            //
            // // 每一种策略，最后一个参数都是一个委托，这个委托就是异常委托。
            //
            // // 普通断路器，非常实用的策略，也是必备的
            // // 连续触发了指定（3）次数的故障后，就开启断路器（OPEN），进入熔断状态，1分钟
            // var breaker =  Policy.Handle<Exception>()
            //     .CircuitBreaker(2, 
            //         TimeSpan.FromMinutes(1),
            //         (exception, span) => {},// OPEN
            //         () =>{} ); // ClOSE
            // // 断路器有三种状态，OPEN、CLOSED、HALF-OPEN
            // // breaker.CircuitState == CircuitState.Closed;
            // // breaker.CircuitState == CircuitState.HalfOpen;
            // //
            // // breaker.CircuitState == CircuitState.Open;
            // //
            // // // 这个不是断路器模式的状态，这个是Polly断路器策略里的一种特殊状态/
            // // // 手动的打开断路器，断路器：手动开启状态
            // // breaker.CircuitState == CircuitState.Isolated;
            // //
            // // // 手段开启断路器，断路器默认是关闭的
            // // breaker.Isolate();
            // // breaker.Reset();
            //
            // // 高级断路器
            // // 如果在故障采样持续时间内，发生故障的比例超过故障阈值，则发生熔断
            // // 前提是在此期间，通过断路器的操作的数量至少是最小吞吐量
            // Policy.Handle<Exception>()
            //     .AdvancedCircuitBreaker(
            //         0.5, // 故障阈值，50%
            //         TimeSpan.FromSeconds(10), // 故障采样时间,10秒
            //         8,  // 最小吞吐量，10秒内最少执行了8次操作
            //         TimeSpan.FromSeconds(30) // 熔断时间，30秒
            //         );
            // // half-open，半开启状态，断路器会尝试着释放（1次）操作，尝试去请求，
            // // 如果成功，就变成close，如果失败，断路器打开open（30秒）
            //
            // // 超时！服务调用，他也没有挂，就是慢！！慢本身就已经是一种故障，定义2秒。
            //
            // // 最后不会用单独的策略，只会用策略组合
            //
            // // Policy.Timeout(3, (context, span, arg3, arg4) =>);
            //
            //
            // // 舱壁隔离,通过控制并发数量来管理负载，超过12的都拒绝掉。知道是什么是并发。。。
            // Policy.Bulkhead(12, 20);
            // // 缓存比较复杂， 依赖其他的库，集合redis
            // // 回退策略


            // 策略包装，策略组合

            // 降级策略
            var fallback = Policy.Handle<Exception>()
                .Fallback(() => { Console.WriteLine("Polly Fallback!"); });

            // 重试策略
            var retry = Policy.Handle<Exception>().Retry(3, (exception, i) =>
            {
                Console.WriteLine($"retryCount:{i}");
            });

            // 如果重试3次，仍然发生故障，就降级
            // 从右到左
            var policy = Policy.Wrap(fallback, retry);
            policy.Execute(() =>
            {
                Console.WriteLine("Polly Begin");
                throw new Exception("Error");
            });

            // Polly里所有的策略，除了缓存，其它的都讲了，然后咱们实战一下
            // 服务A，服务A是一个集群（2个节点），有一个请求进来

            // 首先调用其中一个节点的服务，如果失败或者超时，则用轮询的方式进行重试调用。
            // 重试多次仍然失败，则直接返回一个替代数据（服务降级）。之后一段时间，该服务被熔断。
            // 在熔断时间内，所有对该服务的调用都以替代数据返回；熔断时间过后，尝试再次调用，如果成功，则关闭熔断器，服务通道打通.
            // 否则，继续熔断这个服务一段时间。

            // 要用到几种策略？超时策略、重试策略、断路器策略、回退策略、策略组合
        }

    }
}
