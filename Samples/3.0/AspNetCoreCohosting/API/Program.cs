using System;
using System.Net;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Orleans;
using Orleans.Configuration;
using Orleans.Hosting;
using Orleans.Statistics;

namespace ASPNetCoreHostedServices
{
    public class Program
    {
        public static Task Main(string[] args) =>
            Host.CreateDefaultBuilder(args)
                .ConfigureWebHostDefaults(webBuilder =>
                {
                    webBuilder.Configure((ctx, app) =>
                    {
                        if (ctx.HostingEnvironment.IsDevelopment())
                        {
                            app.UseDeveloperExceptionPage();
                        }

                        app.UseHttpsRedirection();
                        app.UseRouting();
                        app.UseAuthorization();
                        app.UseEndpoints(endpoints =>
                        {
                            endpoints.MapControllers();
                        });
                    });
                })
                .ConfigureServices(services =>
                {
                    services.AddControllers();
                })
                .UseOrleans(siloBuilder =>
                {
                    siloBuilder
                        // .UseLocalhostClustering()
                        .UseConsulClustering(option =>
                        {
                            option.Address = new Uri("http://localhost:8500");
                        })
                        .Configure<ClusterOptions>(opts =>
                        {
                            opts.ClusterId = "dev";
                            opts.ServiceId = "silo1";
                        })
                        .Configure<EndpointOptions>(opts =>
                        {
                            opts.AdvertisedIPAddress = IPAddress.Loopback;
                            // opts.GatewayListeningEndpoint = new IPEndPoint(IPAddress.Any, 30102);
                            opts.GatewayPort = 30002;
                            opts.SiloPort = 11112;
                        })
                        .ConfigureApplicationParts(parts => parts.AddFromApplicationBaseDirectory())
                        //.AddMemoryGrainStorage(name: "ArchiveStorage")
                        .AddAdoNetGrainStorage("ArchiveStorage", options =>
                        {
                            options.Invariant = "MySql.Data.MySqlClient";
                            options.ConnectionString = "Server=127.0.0.1;Database=orleans_test;Uid=root;Pwd=123456;";
                            options.UseJsonFormat = true;
                        })
                        .UseDashboard(options =>
                        {
                            options.Username = "USERNAME";
                            options.Password = "PASSWORD";
                            options.Host = "*";
                            options.Port = 8080;
                            options.HostSelf = true;
                            options.CounterUpdateIntervalMs = 20000;
                        })
                        .UsePerfCounterEnvironmentStatistics()
                        ;
                })
                .RunConsoleAsync();
    }
}