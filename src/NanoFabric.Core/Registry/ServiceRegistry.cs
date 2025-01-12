﻿using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace NanoFabric.Core
{
    public class ServiceRegistry : IManageServiceInstances, IManageHealthChecks, IResolveServiceInstances
    {
        private readonly IRegistryHost _registryHost;
        private IResolveServiceInstances _serviceInstancesResolver;

        public ServiceRegistry(IRegistryHost registryHost)
        {
            _registryHost = registryHost;
        }

        #region IManageServiceInstances
        /// <summary>
        /// 注册服务实例
        /// </summary>
        /// <param name="serviceName">服务名称</param>
        /// <param name="version">版本号</param>
        /// <param name="uri">服务地址</param>
        /// <param name="healthCheckUri">健康检查url</param>
        /// <param name="tags">标签</param>
        /// <returns>注册信息</returns>
        public async Task<RegistryInformation> RegisterServiceAsync(string serviceName, string version, Uri uri, Uri healthCheckUri = null, IEnumerable<string> tags = null)
        {
            var registryInformation = await _registryHost.RegisterServiceAsync(serviceName, version, uri, healthCheckUri, tags);

            return registryInformation;
        }

        /// <summary>
        /// 注销服务实例
        /// </summary>
        /// <param name="serviceId"></param>
        /// <returns>是否成功注销</returns>
        public async Task<bool> DeregisterServiceAsync(string serviceId)
        {
            return await _registryHost.DeregisterServiceAsync(serviceId);
        }
        #endregion

        #region IManageHealthChecks
        /// <summary>
        /// 注册服务的健康检查
        /// </summary>
        /// <param name="serviceName">服务名称</param>
        /// <param name="serviceId">服务ID</param>
        /// <param name="checkUri">健康检查服务</param>
        /// <param name="interval">间隔时间</param>
        /// <param name="notes"></param>
        /// <returns></returns>
        public async Task<string> RegisterHealthCheckAsync(string serviceName, string serviceId, Uri checkUri, TimeSpan? interval = null, string notes = null)
        {
            return await _registryHost.RegisterHealthCheckAsync(serviceName, serviceId, checkUri, interval, notes);
        }

        /// <summary>
        /// 注销实例的健康检查服务
        /// </summary>
        /// <param name="checkId">实例的健康检查标识Id</param>
        /// <returns></returns>
        public async Task<bool> DeregisterHealthCheckAsync(string checkId)
        {
            return await _registryHost.DeregisterHealthCheckAsync(checkId);
        }
        #endregion

        #region IResolveServiceInstances
        public async Task<IList<RegistryInformation>> FindServiceInstancesAsync()
        {
            return _serviceInstancesResolver == null
                ? await _registryHost.FindServiceInstancesAsync()
                : await _serviceInstancesResolver.FindServiceInstancesAsync();
        }

        public async Task<IList<RegistryInformation>> FindServiceInstancesAsync(string name)
        {
            return _serviceInstancesResolver == null
                ? await _registryHost.FindServiceInstancesAsync(name)
                : await _serviceInstancesResolver.FindServiceInstancesAsync(name);
        }

        public async Task<IList<RegistryInformation>> FindServiceInstancesWithVersionAsync(string name, string version)
        {
            return _serviceInstancesResolver == null
                ? await _registryHost.FindServiceInstancesWithVersionAsync(name, version)
                : await _serviceInstancesResolver.FindServiceInstancesWithVersionAsync(name, version);
        }

        public async Task<IList<RegistryInformation>> FindServiceInstancesAsync(Predicate<KeyValuePair<string, string[]>> nameTagsPredicate,
            Predicate<RegistryInformation> registryInformationPredicate)
        {
            return _serviceInstancesResolver == null
                ? await _registryHost.FindServiceInstancesAsync(nameTagsPredicate, registryInformationPredicate)
                : await _serviceInstancesResolver.FindServiceInstancesAsync(nameTagsPredicate, registryInformationPredicate);
        }

        public async Task<IList<RegistryInformation>> FindServiceInstancesAsync(Predicate<KeyValuePair<string, string[]>> predicate)
        {
            return _serviceInstancesResolver == null
                ? await _registryHost.FindServiceInstancesAsync(predicate)
                : await _serviceInstancesResolver.FindServiceInstancesAsync(predicate);
        }

        public async Task<IList<RegistryInformation>> FindServiceInstancesAsync(Predicate<RegistryInformation> predicate)
        {
            return _serviceInstancesResolver == null
                ? await _registryHost.FindServiceInstancesAsync(predicate)
                : await _serviceInstancesResolver.FindServiceInstancesAsync(predicate);
        }

        public async Task<IList<RegistryInformation>> FindAllServicesAsync()
        {
            return _serviceInstancesResolver == null
                ? await _registryHost.FindAllServicesAsync()
                : await _serviceInstancesResolver.FindAllServicesAsync();
        }

        public async Task<RegistryInformation> AddTenantAsync(IRegistryTenant registryTenant, string serviceName, string version, Uri healthCheckUri = null, IEnumerable<string> tags = null)
        {
            var uri = registryTenant.Uri;
            return await RegisterServiceAsync(serviceName, version, uri, healthCheckUri, tags);
        }

        public async Task<string> AddHealthCheckAsync(string serviceName, string serviceId, Uri checkUri, TimeSpan? interval = null, string notes = null)
        {
            return await RegisterHealthCheckAsync(serviceName, serviceId, checkUri, interval, notes);
        } 

        public void ResolveServiceInstancesWith<T>(T serviceInstancesResolver)
            where T : IResolveServiceInstances
        {
            _serviceInstancesResolver = serviceInstancesResolver;
        }
        #endregion

    }
}
