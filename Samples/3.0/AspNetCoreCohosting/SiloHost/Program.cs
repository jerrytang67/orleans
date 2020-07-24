using System;
using System.Net;
using System.Threading.Tasks;
using ASPNetCoreHostedServices.Grains;
using ASPNetCoreHostedServices.Interfaces;
using Microsoft.Extensions.Logging;
using Orleans;
using Orleans.Configuration;
using Orleans.Hosting;

namespace ASPNetCoreHostedServices.SiloHost
{
    public class Program
    {
        public static int Main(string[] args)
        {
            return RunMainAsync().Result;
        }

        private static async Task<int> RunMainAsync()
        {
            try
            {
                var host = await StartSilo();
                Console.WriteLine("\n\n Press Enter to terminate...\n\n");
                Console.ReadLine();

                await host.StopAsync();

                return 0;
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
                return 1;
            }
        }

        private static async Task<ISiloHost> StartSilo()
        {
            // define the cluster configuration
            var builder = new SiloHostBuilder()
                    // .UseLocalhostClustering()
                    .UseConsulClustering(option =>
                    {
                        option.Address = new Uri("http://localhost:8500");
                    })
                    .Configure<ClusterOptions>(options =>
                    {
                        options.ClusterId = "dev";
                        options.ServiceId = "silo2";
                    })
                    .Configure<EndpointOptions>(opts =>
                    {
                        opts.AdvertisedIPAddress = IPAddress.Loopback;
                        // opts.GatewayListeningEndpoint = new IPEndPoint(IPAddress.Any, 30103);
                        opts.GatewayPort = 30003;
                        opts.SiloPort = 11113;
                    })
                    .ConfigureApplicationParts(
                        parts => parts
                            .AddApplicationPart(typeof(TestGrain).Assembly)
                            .WithReferences())
                    //.ConfigureApplicationParts(parts => parts.AddFromApplicationBaseDirectory())
                    .ConfigureLogging(logging => logging.AddConsole())
                ;

            var host = builder.Build();
            await host.StartAsync();
            return host;
        }
    }
}