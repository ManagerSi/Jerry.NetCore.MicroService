
1 启动 Consul (Tools 目录下，或下载地址： https://www.consul.io/downloads.html)， 
  进入Tools\consul_1.6.1_windows_amd64 文件夹下 
  cmd -> consul agent -dev 启动，(确保8500端口没有被占用)
  此时可访问 http://localhost:8500 查看Consul Ui页面；

2 启动api服务
  进入目录Jerry.NetCore.MicroService\bin\Debug\netcoreapp3.1 
  打开cmd
  输入命令：dotnet Jerry.NetCore.MicroService.dll --urls=http://localhost:8889 --port=8889
  多开几个cmd，分别输入一下命令（启动多个服务实例）
  dotnet Jerry.NetCore.MicroService.dll --urls=http://localhost:8880 --port=8880
  dotnet Jerry.NetCore.MicroService.dll --urls=http://localhost:8881 --port=8881
  dotnet Jerry.NetCore.MicroService.dll --urls=http://localhost:8882 --port=8882

3 启动Ocelot （监听自己的端口 5000）
  直接运行项目Jerry.Ocelot（非iisexpress）
  浏览器中输入 http://localhost:5000/home，根据ocelot.json配置，ocelot会将请求转发到相应DownstreamPathTemplate中 

3 启动nginx（监听80端口）
  Tools\nginx-1.17.9 
  打开 cmd-> start nginx.exe (确保80端口没被占用，否则报错：An attempt was made to access a socket in a way forbidden)
  调整配置文件：Tools\nginx-1.17.9\conf\nginx.conf
	查找：Microservice，修改下面的服务地址即可
  
  