﻿using NanoFabric.Core;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace NanoFabric.Router.Throttle {
    /// <summary>
    /// 限流服务订阅器
    /// </summary>
    public class ThrottleServiceSubscriber : IServiceSubscriber {
        private bool _disposed;
        private readonly IServiceSubscriber _serviceSubscriber;
        private readonly TimeSpan _maxPeriod;
        private readonly SemaphoreSlim _throttleActions, _throttlePeriods;

        /// <summary>
        /// 初始化限流服务订阅器
        /// </summary>
        /// <param name="serviceSubscriber">服务订阅器</param>
        /// <param name="throttleOptions">限流选项</param>
        public ThrottleServiceSubscriber(IServiceSubscriber serviceSubscriber, ThrottleSubscriberOptions throttleOptions)
            : this(serviceSubscriber, throttleOptions.MaxUpdatesPerPeriod, throttleOptions.MaxUpdatesPeriod) { 
        }

        /// <summary>
        /// 初始化限流服务订阅器
        /// </summary>
        /// <param name="serviceSubscriber">服务订阅器</param>
        /// <param name="maxActions">最大Action</param>
        /// <param name="maxPeriod">最大周期</param>
        public ThrottleServiceSubscriber(IServiceSubscriber serviceSubscriber, int maxActions, TimeSpan maxPeriod) {
            _serviceSubscriber = serviceSubscriber;
            _throttleActions = new SemaphoreSlim(maxActions, maxActions);
            _throttlePeriods = new SemaphoreSlim(maxActions, maxActions);
            _maxPeriod = maxPeriod;
        }

        public async Task<List<RegistryInformation>> Endpoints(CancellationToken ct = default) {
            if (_disposed) {
                throw new ObjectDisposedException(nameof(Endpoints));
            }

            await _throttleActions.WaitAsync(ct).ConfigureAwait(false);
            try {
                await _throttlePeriods.WaitAsync(ct).ConfigureAwait(false);

                // Release after period
                // - Allow bursts up to maxActions requests at once
                // - Do not allow more than maxActions requests per period
                #pragma warning disable 4014
                Task.Delay(_maxPeriod, ct).ContinueWith(tt =>
                {
                    _throttlePeriods.Release(1);
                }, ct).ConfigureAwait(false);

                return await _serviceSubscriber.Endpoints(ct).ConfigureAwait(false);
            } finally {
                _throttleActions.Release(1);
            }
        }

        protected virtual void Dispose(bool disposing) {
            if (_disposed) {
                return;
            }

            if (disposing) {
                _throttleActions.Dispose();
                _throttlePeriods.Dispose();
                _serviceSubscriber.Dispose();
            }

            _disposed = true;
        }

        public void Dispose() {
            Dispose(true);
        }
    }
}
