using System;
using System.Threading;
using System.Threading.Tasks;

namespace NanoFabric.Router {
    /// <summary>
    /// 投票服务订阅器
    /// </summary>
    public interface IPollingServiceSubscriber : IServiceSubscriber {
        /// <summary>
        /// 开始服务订阅
        /// </summary>
        /// <param name="ct">取消Token</param>
        /// <returns>服务订阅任务</returns>
        Task StartSubscription(CancellationToken ct = default);

        /// <summary>
        /// 端点已改变事件
        /// </summary>
        event EventHandler EndpointsChanged;
    }
}
