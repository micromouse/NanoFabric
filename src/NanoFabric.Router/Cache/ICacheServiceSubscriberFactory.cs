namespace NanoFabric.Router.Cache {
    /// <summary>
    /// 缓存服务订阅器工厂
    /// </summary>
    public interface ICacheServiceSubscriberFactory {
        /// <summary>
        /// 建立缓存服务订阅器
        /// </summary>
        /// <param name="serviceSubscriber">服务订阅器</param>
        /// <returns>投票服务订阅器</returns>
        IPollingServiceSubscriber CreateSubscriber(IServiceSubscriber serviceSubscriber);
    }
}
