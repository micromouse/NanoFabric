using Consul;
using System;
using System.Collections.Generic;
using System.Text;

namespace NanoFabric.Router.Consul
{
    /// <summary>
    /// Consul服务器订阅器工厂
    /// </summary>
    public class ConsulServiceSubscriberFactory : IConsulServiceSubscriberFactory
    {
        private readonly IConsulClient _consulClient;

        /// <summary>
        /// 初始化Consul服务订阅器工厂
        /// </summary>
        /// <param name="consulClient">IConsulClient</param>
        public ConsulServiceSubscriberFactory(IConsulClient consulClient)
        {
            _consulClient = consulClient;
        }

        /// <summary>
        /// 建立服务订阅器
        /// </summary>
        /// <param name="serviceName">服务名</param>
        /// <param name="consulOptions">Consul订阅器选项</param>
        /// <param name="watch">是否观察</param>
        /// <returns>Consul服务器订阅器</returns>
        public IServiceSubscriber CreateSubscriber(string serviceName, ConsulSubscriberOptions consulOptions, bool watch = false)
        {
            return new ConsulServiceSubscriber(_consulClient, serviceName, consulOptions, watch);
        }
    }
}
