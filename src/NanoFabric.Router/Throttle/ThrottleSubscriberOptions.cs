using System;

namespace NanoFabric.Router.Throttle {
    /// <summary>
    /// 限流订阅器选项
    /// </summary>
    public class ThrottleSubscriberOptions {
        /// <summary>
        /// 缺省限流订阅器选项
        /// </summary>
        public static readonly ThrottleSubscriberOptions Default = new ThrottleSubscriberOptions();

        /// <summary>
        /// 每周期最大更新
        /// </summary>
        public int MaxUpdatesPerPeriod { get; set; } = 5;

        /// <summary>
        /// 最大更新周期
        /// </summary>
        public TimeSpan MaxUpdatesPeriod { get; set; } = TimeSpan.FromSeconds(10);
    }
}