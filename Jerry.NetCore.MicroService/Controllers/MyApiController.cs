using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

namespace Jerry.NetCore.MicroService.Controllers
{
    //[Route("api/[controller]")]
    [Route("[Controller]/[Action]")]
    [ApiController]
    public class MyApiController : ControllerBase
    {
        private readonly ILogger<MyApiController> _logger;
        private readonly IConfiguration _configuration;

        public MyApiController(ILogger<MyApiController> logger, IConfiguration config)
        {
            _logger = logger;
            _configuration = config;
        }
        [HttpGet] // api/Timeout
        //[Route("MyApi", "Timeout")]
        public IEnumerable<string> Timeout()
        {
            _logger.LogInformation("Timeout invoke");

            Thread.Sleep(TimeSpan.FromSeconds(5));

            return new string[] { "Timeout", "Timeout" };
        }

        [HttpGet] // api/Timeout
        //[Route("MyApi", "Timeout")]
        public IEnumerable<string> ThrowException()
        {
            _logger.LogInformation("Timeout invoke");

            throw new Exception("this is exception");

        }


        /// <summary>
        /// 查询
        /// </summary>
        /// <returns>结果</returns>
        [HttpGet]   // api
        public IEnumerable<string> Get()
        {
            _logger.LogInformation("Get invoke");
            #region session
            string user = base.HttpContext.Session.GetString("CurrentUser");
            if (user == null)
            {// 获取当前服务地址和端口
                string port = (Request.HttpContext.Connection.LocalIpAddress.MapToIPv4().ToString() + ":" + Request.HttpContext.Connection.LocalPort);
                
                base.HttpContext.Session.SetString("CurrentUser",$"Jerry {port}");
                user = base.HttpContext.Session.GetString("CurrentUser");
            }

            #endregion

            return new string[] { "value1", "value2", user };
        }

        /// <summary>
        /// 查询详细
        /// </summary>
        /// <param name="id">id</param>
        /// <returns>结果：value</returns>
        [HttpGet("{id}", Name = "Get")]
        public string Get(int id)
        {

            return "value";
        }

        // POST: api/MyApi
        [HttpPost]
        public void Post([FromBody] string value)
        {
        }

        // PUT: api/MyApi/5
        [HttpPut("{id}")]
        public void Put(int id, [FromBody] string value)
        {
        }

        // DELETE: api/ApiWithActions/5
        [HttpDelete("{id}")]
        public void Delete(int id)
        {
        }
    }
}
