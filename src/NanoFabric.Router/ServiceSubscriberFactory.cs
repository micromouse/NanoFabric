using NanoFabric.Router.Cache;
using NanoFabric.Router.Consul;
using NanoFabric.Router.Throttle;

namespace NanoFabric.Router {
    /// <summary>
    /// 服务订阅器工厂
    /// </summary>
    public class ServiceSubscriberFactory : IServiceSubscriberFactory {
        private readonly IConsulServiceSubscriberFactory _consulServiceSubscriberFactory;
        private readonly ICacheServiceSubscriberFactory _cacheServiceSubscriberFactory;

        /// <summary>
        /// 初始化服务订阅器工厂
        /// </summary>
        /// <param name="consulServiceSubscriberFactory">Consul服务器订阅器工厂</param>
        /// <param name="cacheServiceSubscriberFactory">缓存服务订阅器工厂</param>
        public ServiceSubscriberFactory(IConsulServiceSubscriberFactory consulServiceSubscriberFactory, ICacheServiceSubscriberFactory cacheServiceSubscriberFactory) {
            _consulServiceSubscriberFactory = consulServiceSubscriberFactory;
            _cacheServiceSubscriberFactory = cacheServiceSubscriberFactory;
        }

        public IPollingServiceSubscriber CreateSubscriber(string servicName) {
            return CreateSubscriber(servicName, ConsulSubscriberOptions.Default, ThrottleSubscriberOptions.Default);
        }

        /// <summary>
        /// 建立投票服务订阅器
        /// </summary>
        /// <param name="serviceName">服务名</param>
        /// <param name="consulOptions">Consul选项</param>
        /// <param name="throttleOptions">限流选项</param>
        /// <returns>投票服务订阅器</returns>
        public IPollingServiceSubscriber CreateSubscriber(string serviceName, ConsulSubscriberOptions consulOptions, ThrottleSubscriberOptions throttleOptions) {
            //基于Consul服务订阅器创建限流服务订阅器
            var consulSubscriber = _consulServiceSubscriberFactory.CreateSubscriber(serviceName, consulOptions, true);
            var throttleSubscriber = new ThrottleServiceSubscriber(consulSubscriber, throttleOptions.MaxUpdatesPerPeriod, throttleOptions.MaxUpdatesPeriod);
            
            //基于限流服务订阅器建立缓存服务订阅器
            return _cacheServiceSubscriberFactory.CreateSubscriber(throttleSubscriber);
        }
    }
}
