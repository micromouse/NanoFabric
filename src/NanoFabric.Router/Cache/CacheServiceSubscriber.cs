﻿using NanoFabric.Core;
using NanoFabric.Router.Cache.Internal;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace NanoFabric.Router.Cache {
    /// <summary>
    /// 缓存服务订阅器
    /// </summary>
    public class CacheServiceSubscriber : IPollingServiceSubscriber {
        private bool _disposed;

        private readonly ICacheClient _cache;
        private readonly CancellationTokenSource _cts = new CancellationTokenSource();

        private readonly IServiceSubscriber _serviceSubscriber;

        private readonly string _id = Guid.NewGuid().ToString();

        private Task _subscriptionTask;
        private readonly SemaphoreSlim _mutex = new SemaphoreSlim(1, 1);

        public event EventHandler EndpointsChanged;

        /// <summary>
        /// 初始化缓存服务订阅器
        /// </summary>
        /// <param name="serviceSubscriber">服务订阅器</param>
        /// <param name="cache">缓存客户端</param>
        public CacheServiceSubscriber(IServiceSubscriber serviceSubscriber, ICacheClient cache) {
            _cache = cache;
            _serviceSubscriber = serviceSubscriber;
        }

        public async Task<List<RegistryInformation>> Endpoints(CancellationToken ct = default) {
            if (_disposed) {
                throw new ObjectDisposedException(nameof(CacheServiceSubscriber));
            }

            await StartSubscription(ct).ConfigureAwait(false);

            return _cache.Get<List<RegistryInformation>>(_id);
        }

        /// <summary>
        /// 开始订阅
        /// </summary>
        /// <param name="ct">取消Token</param>
        /// <returns>订阅任务</returns>
        public async Task StartSubscription(CancellationToken ct = default) {
            if (_subscriptionTask == null) {
                await _mutex.WaitAsync(ct).ConfigureAwait(false);
                try {
                    if (_subscriptionTask == null) {
                        var serviceUris = await _serviceSubscriber.Endpoints(ct).ConfigureAwait(false);
                        _cache.Set(_id, serviceUris);
                        _subscriptionTask = StartSubscriptionLoop(serviceUris);
                    }
                } finally {
                    _mutex.Release();
                }
            }
        }

        private Task StartSubscriptionLoop(List<RegistryInformation> previousEndpoints) {
            return Task.Run(async () =>
            {
                while (!_cts.IsCancellationRequested) {
                    try {
                        var currentEndpoints = await _serviceSubscriber.Endpoints(_cts.Token).ConfigureAwait(false);
                        if (!EndpointListsMatch(previousEndpoints, currentEndpoints)) {
                            _cache.Set(_id, currentEndpoints);
                            EndpointsChanged?.Invoke(this, EventArgs.Empty);
                            previousEndpoints = currentEndpoints;
                        }
                    } catch {
                        // ignore
                    }
                }

                throw new Exception("任务完成");
            }, _cts.Token);
        }

        private static bool EndpointListsMatch(ICollection<RegistryInformation> endpoints1, ICollection<RegistryInformation> endpoints2) {
            if (endpoints1.Count != endpoints2.Count) {
                return false;
            }

            var filteredSequence = endpoints1.Where(endpoints2.Contains);
            return filteredSequence.Count() == endpoints1.Count;
        }

        ~CacheServiceSubscriber() {
            Dispose(false);
        }

        public void Dispose() {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        protected virtual void Dispose(bool disposing) {
            if (_disposed) {
                return;
            }

            if (disposing) {
                if (!_cts.IsCancellationRequested) {
                    _cts.Cancel();
                }
                _cts.Dispose();
                _mutex.Dispose();
                _serviceSubscriber.Dispose();
            }

            _cache.Remove(_id);

            _disposed = true;
        }
    }
}
