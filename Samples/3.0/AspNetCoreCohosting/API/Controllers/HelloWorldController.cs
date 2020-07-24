using System.Threading.Tasks;
using AspNetCoreCohosting.Interfaces;
using ASPNetCoreHostedServices.Interfaces;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using Orleans;

namespace ASPNetCoreHostedServices.Controllers
{
    [ApiController]
    [Route("api/hello")]
    public class HelloWorldController : ControllerBase
    {
        private readonly IGrainFactory _client;
        private readonly IHelloWorld _grain;
        private readonly ITestGrain _testGrain;

        public HelloWorldController(IGrainFactory client)
        {
            _client = client;
            _grain = _client.GetGrain<IHelloWorld>(0);
            _testGrain = _client.GetGrain<ITestGrain>(0);
        }

        [HttpGet]
        public Task<string> SayHello() => this._grain.SayHello();


        [HttpGet("test/{id}")]
        public Task<string> test(string id) => this._testGrain.SayHi(id);


        [HttpGet("TestAll")]
        public async Task<string> TestAll() => JsonConvert.SerializeObject(await _testGrain.ShowTime());

        [HttpGet("Clear")]
        public async Task<string> Clear()
        {
            await _testGrain.Clear();
            return "Clear";
        }
    }
}