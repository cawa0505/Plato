﻿using System;
using System.Threading.Tasks;
using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Logging;
using Plato.Abstractions.Collections;
using Plato.Abstractions.Query;
using Plato.Models.Roles;
using Plato.Repositories.Roles;

namespace Plato.Stores.Roles
{
    public class PlatoRoleStore : IPlatoRoleStore
    {
        #region "Constructor"

        public PlatoRoleStore(
            IRoleRepository<Role> roleRepository,
            IMemoryCache memoryCache,
            IDistributedCache distributedCache,
            ILogger<PlatoRoleStore> logger)
        {
            _roleRepository = roleRepository;
            _memoryCache = memoryCache;
            _distributedCache = distributedCache;
            _logger = logger;
        }

        #endregion

        #region "Private Variables"

        private readonly string _key = CacheKeys.Roles.ToString();

        private readonly IRoleRepository<Role> _roleRepository;
        private readonly IMemoryCache _memoryCache;
        private readonly IDistributedCache _distributedCache;
        private readonly ILogger<PlatoRoleStore> _logger;

        #endregion

        #region "Implementation"

        public async Task<Role> CreateAsync(Role model)
        {
            return await _roleRepository.InsertUpdateAsync(model);
        }

        public async Task<Role> UpdateAsync(Role model)
        {
            _memoryCache.Remove(_key);
            return await _roleRepository.InsertUpdateAsync(model);
        }

        public async Task<bool> DeleteAsync(Role model)
        {
            _memoryCache.Remove(_key);
            return await _roleRepository.DeleteAsync(model.Id);
        }

        public async Task<Role> GetByIdAsync(int id)
        {
            Role role;
            if (!_memoryCache.TryGetValue(_key, out role))
            {
                role = await _roleRepository.SelectByIdAsync(id);
                if (role != null)
                {
                    if (_logger.IsEnabled(LogLevel.Debug))
                        _logger.LogDebug("Adding entry to cache of type {0}. Entry key: {1}.",
                            _memoryCache.GetType().Name, _key);
                    _memoryCache.Set(_key, role);
                }
            }
            return role;
        }

        public async Task<Role> GetByName(string name)
        {
            Role role;
            if (!_memoryCache.TryGetValue(_key, out role))
            {
                role = await _roleRepository.SelectByNameAsync(name);
                if (role != null)
                {
                    if (_logger.IsEnabled(LogLevel.Debug))
                        _logger.LogDebug("Adding entry to cache of type {0}. Entry key: {1}.",
                            _memoryCache.GetType().Name, _key);
                    _memoryCache.Set(_key, role);
                }
            }
            return role;
        }

        public async Task<Role> GetByNormalizedName(string nameNormalized)
        {
            Role role;
            if (!_memoryCache.TryGetValue(_key, out role))
            {
                role = await _roleRepository.SelectByNormalizedNameAsync(nameNormalized);
                if (role != null)
                {
                    if (_logger.IsEnabled(LogLevel.Debug))
                        _logger.LogDebug("Adding entry to cache of type {0}. Entry key: {1}.",
                            _memoryCache.GetType().Name, _key);
                    _memoryCache.Set(_key, role);
                }
            }
            return role;
        }

        public IQuery QueryAsync()
        {
            throw new NotImplementedException();
        }

        public async Task<IPagedResults<T>> SelectAsync<T>(params object[] args) where T : class
        {
            IPagedResults<T> roles;
            if (!_memoryCache.TryGetValue(_key, out roles))
            {
                roles = await _roleRepository.SelectAsync<T>(args);
                if (roles != null)
                {
                    if (_logger.IsEnabled(LogLevel.Debug))
                        _logger.LogDebug("Adding entry to cache of type {0}. Entry key: {1}.",
                            _memoryCache.GetType().Name, _key);
                    _memoryCache.Set(_key, roles);
                }
            }
            return roles;
        }

        #endregion
    }
}