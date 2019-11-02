using NanoFabric.Router.Cache.Internal;
using System;
using System.Collections.Generic;
using System.Text;

namespace NanoFabric.Router.Cache
{
    /// <summary>
    /// 缓存服务订阅器工厂
    /// </summary>
    public class CacheServiceSubscriberFactory : ICacheServiceSubscriberFactory
    {
        private readonly ICacheClient _cacheClient;

        /// <summary>
        /// 初始化缓存服务订阅器工厂
        /// </summary>
        /// <param name="cacheClient">缓存客户端</param>
        public CacheServiceSubscriberFactory(ICacheClient cacheClient)
        {
            _cacheClient = cacheClient;
        }

        /// <summary>
        /// 建立缓存服务订阅器
        /// </summary>
        /// <param name="serviceSubscriber">服务订阅器</param>
        /// <returns>投票服务订阅器</returns>
        public IPollingServiceSubscriber CreateSubscriber(IServiceSubscriber serviceSubscriber)
        {
            return new CacheServiceSubscriber(serviceSubscriber, _cacheClient);
        }
    }
}
