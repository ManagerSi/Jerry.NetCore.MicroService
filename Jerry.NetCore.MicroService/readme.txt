.netcore3.1 + consul + ocelot + nginx 
session -> redis

1 启动 Consul (Tools 目录下，或下载地址： https://www.consul.io/downloads.html)， 
  进入Tools\consul_1.6.1_windows_amd64 文件夹下 
  cmd -> consul agent -dev 启动，(确保8500端口没有被占用)
  此时可访问 http://localhost:8500 查看Consul Ui页面；

2 启动redis实例（默认无密码）netcore的session使用，分布式session，所有请求统一请求redis

3 启动api服务
  进入目录Jerry.NetCore.MicroService\bin\Debug\netcoreapp3.1 
  打开cmd
  输入命令：dotnet Jerry.NetCore.MicroService.dll --urls=http://localhost:8889 --port=8889
  多开几个cmd，分别输入一下命令（启动多个服务实例）
  dotnet Jerry.NetCore.MicroService.dll --urls=http://localhost:8880 --port=8880
  dotnet Jerry.NetCore.MicroService.dll --urls=http://localhost:8881 --port=8881
  dotnet Jerry.NetCore.MicroService.dll --urls=http://localhost:8882 --port=8882
