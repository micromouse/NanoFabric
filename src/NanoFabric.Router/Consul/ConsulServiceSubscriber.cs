using Consul;
using NanoFabric.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace NanoFabric.Router.Consul
{
    /// <summary>
    /// Consul服务订阅器
    /// </summary>
    public class ConsulServiceSubscriber : IServiceSubscriber
    {
        private readonly IConsulClient _client;
        private readonly List<string> _tags;
        private readonly string _serviceName;
        private readonly bool _passingOnly;
        private readonly bool _watch;
        public ulong WaitIndex;

        /// <summary>
        /// 初始化Consul服务订阅器
        /// </summary>
        /// <param name="client">IConsulClient</param>
        /// <param name="serviceName">服务名</param>
        /// <param name="consulOptions">Consul选项</param>
        /// <param name="watch">是否观察</param>
        public ConsulServiceSubscriber(IConsulClient client, string serviceName, ConsulSubscriberOptions consulOptions,
            bool watch) : this(client, serviceName, consulOptions.Tags, consulOptions.PassingOnly, watch)
        {

        }

        public ConsulServiceSubscriber(IConsulClient client, string serviceName, List<string> tags,
            bool passingOnly, bool watch)
        {
            _client = client;

            _serviceName = serviceName;
            _tags = tags ?? new List<string>();
            _passingOnly = passingOnly;

            _watch = watch;
        }

        public async Task<List<RegistryInformation>> Endpoints(CancellationToken ct = default(CancellationToken))
        {
            // Consul doesn't support more than one tag in its service query method.
            // https://github.com/hashicorp/consul/issues/294
            // Hashicorp suggest prepared queries, but they don't support blocking.
            // https://www.consul.io/docs/agent/http/query.html#execute
            // If we want blocking for efficiency, we must filter tags manually.
            var tag = string.Empty;
            if (_tags.Count > 0)
            {
                tag = _tags[0];
            }

            var queryOptions = new QueryOptions
            {
                WaitIndex = WaitIndex
            };
            var servicesTask = await
                _client.Health.Service(_serviceName, tag, _passingOnly, queryOptions, ct)
                    .ConfigureAwait(false);

            if (_tags.Count > 1)
            {
                servicesTask.Response = FilterByTag(servicesTask.Response, _tags);
            }

            if (_watch)
            {
                WaitIndex = servicesTask.LastIndex;
            }

            return servicesTask.Response.Select(service => service.ToEndpoint()).ToList();
        }

        private static ServiceEntry[] FilterByTag(IEnumerable<ServiceEntry> entries, IReadOnlyCollection<string> tags)
        {
            return entries
                .Where(x => tags.All(x.Service.Tags.Contains))
                .ToArray();
        }

        #region IDisposable Support
        private bool disposedValue = false; // To detect redundant calls

        /// <summary>
        /// Dispose
        /// </summary>
        /// <param name="disposing">是否释放资源</param>
        protected virtual void Dispose(bool disposing) {
            if (!disposedValue) {
                if (disposing) {
                    _client.Dispose();
                }

                disposedValue = true;
            }
        }

        /// <summary>
        /// Dispose
        /// </summary>
        public void Dispose() {
            Dispose(true);
        }
        #endregion


    }
}
