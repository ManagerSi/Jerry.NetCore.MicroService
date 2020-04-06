using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using System.Xml;
using log4net;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace Jerry.NetCore.MicroService
{
    public class Program
    {
        private static readonly ILog _log = LogManager.GetLogger(typeof(Program));

        public static void Main(string[] args)
        {
            #region init log4net manually
            XmlDocument logConfig = new XmlDocument();
            logConfig.Load(File.OpenRead("log4net.config"));
            var logRepo = LogManager.CreateRepository(Assembly.GetEntryAssembly(), typeof(log4net.Repository.Hierarchy.Hierarchy));
            log4net.Config.XmlConfigurator.Configure(logRepo, logConfig["log4net"]);
            #endregion init log4net manually

            _log.Info("Jerry.NetCore.MicroService Application - Main is invoked.");

            CreateHostBuilder(args).Build().Run();
        }

        public static IHostBuilder CreateHostBuilder(string[] args) =>
            Host.CreateDefaultBuilder(args)
                //.SetBasePath(env.ContentRootPath)
                //.AddJsonFile("appsettings.json", optional: true, reloadOnChange: true)
                //.AddEnvironmentVariables()
                .ConfigureLogging((context, loggingbuilder) =>
                {
                    //该方法需要引入Microsoft.Extensions.Logging名称空间

                    loggingbuilder.AddFilter("System", LogLevel.Warning); //过滤掉系统默认的一些日志
                    loggingbuilder.AddFilter("Microsoft", LogLevel.Warning);//过滤掉系统默认的一些日志

                    //添加Log4Net
                    //var path = Directory.GetCurrentDirectory() + "\\log4net.config"; 
                    //不带参数：表示log4net.config的配置文件就在应用程序根目录下，也可以指定配置文件的路径
                    loggingbuilder.AddLog4Net();
                })

                .ConfigureWebHostDefaults(webBuilder =>
                {
                    webBuilder.UseStartup<Startup>();
                });
    }
}
