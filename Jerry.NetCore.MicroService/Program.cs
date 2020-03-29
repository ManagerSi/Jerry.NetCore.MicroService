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
                    //�÷�����Ҫ����Microsoft.Extensions.Logging���ƿռ�

                    loggingbuilder.AddFilter("System", LogLevel.Warning); //���˵�ϵͳĬ�ϵ�һЩ��־
                    loggingbuilder.AddFilter("Microsoft", LogLevel.Warning);//���˵�ϵͳĬ�ϵ�һЩ��־

                    //���Log4Net

                    //var path = Directory.GetCurrentDirectory() + "\\log4net.config"; 
                    //������������ʾlog4net.config�������ļ�����Ӧ�ó����Ŀ¼�£�Ҳ����ָ�������ļ���·��
                    loggingbuilder.AddLog4Net();
                })

                .ConfigureWebHostDefaults(webBuilder =>
                {
                    webBuilder.UseStartup<Startup>();
                });
    }
}
