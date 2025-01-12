﻿using DnsClient;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using NanoFabric.Router;
using NanoFabric.Router.Consul;
using System;
using System.IO;
using System.Net.Http;

internal class Program {
    private readonly IDnsQuery _dns;
    private static ServiceProvider _serviceProvider;

    private static void Main(string[] args) {
        Initialize();
        IServiceSubscriberFactory subscriberFactory = _serviceProvider.GetRequiredService<IServiceSubscriberFactory>();
        // 创建ConsoleLogProvider并根据日志类目名称（CategoryName）生成Logger实例
        var logger = _serviceProvider.GetService<ILoggerFactory>().AddConsole().CreateLogger("App");

        var throttleSubscriberOptions = new NanoFabric.Router.Throttle.ThrottleSubscriberOptions() {
            MaxUpdatesPeriod = TimeSpan.FromSeconds(30),
            MaxUpdatesPerPeriod = 20
        };
        var serviceSubscriber = subscriberFactory.CreateSubscriber("SampleService_Kestrel", ConsulSubscriberOptions.Default, throttleSubscriberOptions);
        serviceSubscriber.StartSubscription().ConfigureAwait(false).GetAwaiter().GetResult();
        serviceSubscriber.EndpointsChanged += async (sender, eventArgs) =>
        {
            // Reset connection pool, do something with this info, etc
            var endpoints = await serviceSubscriber.Endpoints();
            var servicesInfo = string.Join(",", endpoints);
            logger.LogInformation($"Received updated subscribers [{servicesInfo}]");
        };
        ILoadBalancer loadBalancer = new RoundRobinLoadBalancer(serviceSubscriber);
        var endPoint = loadBalancer.Endpoint().ConfigureAwait(false).GetAwaiter().GetResult();
        var httpClient = new HttpClient();
        var traceid = Guid.NewGuid().ToString();
        httpClient.DefaultRequestHeaders.Add("ot-traceid", traceid);
        var content = httpClient.GetStringAsync($"{endPoint.ToUri()}api/values").Result;
        Console.WriteLine($"{traceid} content: {content }");
        System.Console.ReadLine();
    }

    private static void Initialize() {
        var configuration = new ConfigurationBuilder()
            .SetBasePath(Directory.GetCurrentDirectory())
            .AddJsonFile("appsettings.json", optional: true, reloadOnChange: true)
            .Build();

        _serviceProvider = new ServiceCollection()
            .AddNanoFabricConsulRouter(configuration)
            .AddLogging()
            .BuildServiceProvider();
    }
}