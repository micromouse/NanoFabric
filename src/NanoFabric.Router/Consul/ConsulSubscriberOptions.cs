using System.Collections.Generic;

namespace NanoFabric.Router.Consul
{
    /// <summary>
    /// Consul订阅器选项
    /// </summary>
    public class ConsulSubscriberOptions
    {
        /// <summary>
        /// 缺省Consul订阅器选项
        /// </summary>
        public static readonly ConsulSubscriberOptions Default = new ConsulSubscriberOptions();

        /// <summary>
        /// 标签集合
        /// </summary>
        public List<string> Tags { get; set; }

        /// <summary>
        /// 是否仅仅Passing,缺省为是
        /// </summary>
        public bool PassingOnly { get; set; } = true;
    }
}