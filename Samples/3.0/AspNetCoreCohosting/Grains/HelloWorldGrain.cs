using System.Threading.Tasks;
using AspNetCoreCohosting.Interfaces;

namespace ASPNetCoreHostedServices.Grains
{
    public class HelloWorldGrain : Orleans.Grain, IHelloWorld
    {
        // private readonly ILogger<HelloWorldGrain> _logger;
        //
        // public HelloWorldGrain(ILogger<HelloWorldGrain> logger)
        // {
        //     this._logger = logger;
        // }

        public Task<string> SayHello() => Task.FromResult("Hello world!");
    }
}