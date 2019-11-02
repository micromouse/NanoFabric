using NanoFabric.Core;
using System.Threading;
using System.Threading.Tasks;

namespace NanoFabric.Router {
    /// <summary>
    /// 主机地址的轮播负载均衡器
    /// </summary>
    public class RoundRobinLoadBalancer : ILoadBalancer {
        private readonly IServiceSubscriber _subscriber;
        private readonly SemaphoreSlim _lock = new SemaphoreSlim(1, 1);
        private int _index;

        /// <summary>
        /// 初始化轮播负载均衡器
        /// </summary>
        /// <param name="subscriber">服务订阅器</param>
        public RoundRobinLoadBalancer(IServiceSubscriber subscriber) {
            _subscriber = subscriber;
        }

        /// <summary>
        /// 通过轮播方式获得端点服务注册信息
        /// </summary>
        /// <param name="ct">取消Token</param>
        /// <returns>服务注册信息</returns>
        public async Task<RegistryInformation> Endpoint(CancellationToken ct = default) {
            var endpoints = await _subscriber.Endpoints(ct).ConfigureAwait(false);
            if (endpoints.Count == 0) {
                return null;
            }

            await _lock.WaitAsync(ct).ConfigureAwait(false);
            try {
                if (_index >= endpoints.Count) {
                    _index = 0;
                }
                var uri = endpoints[_index];
                _index++;

                return uri;
            } finally {
                _lock.Release();
            }
        }
    }
}
