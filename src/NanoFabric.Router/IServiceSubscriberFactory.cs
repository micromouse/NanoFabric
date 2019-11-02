using NanoFabric.Router.Consul;
using NanoFabric.Router.Throttle;

namespace NanoFabric.Router {
    /// <summary>
    /// 服务订阅器工厂
    /// </summary>
    public interface IServiceSubscriberFactory {
        /// <summary>
        /// 建立投票服务订阅器
        /// </summary>
        /// <param name="serviceName">服务名</param>
        /// <returns>投票服务订阅器</returns>
        IPollingServiceSubscriber CreateSubscriber(string serviceName);

        /// <summary>
        /// 建立投票服务订阅器
        /// </summary>
        /// <param name="serviceName">服务名</param>
        /// <param name="consulOptions">Consul选项</param>
        /// <param name="throttleOptions">限流选项</param>
        /// <returns>投票服务订阅器</returns>
        IPollingServiceSubscriber CreateSubscriber(string serviceName, ConsulSubscriberOptions consulOptions, ThrottleSubscriberOptions throttleOptions);
    }
}
