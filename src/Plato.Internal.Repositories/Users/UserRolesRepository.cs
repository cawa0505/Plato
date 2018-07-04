﻿using System;
using System.Collections.Generic;
using System.Data;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Plato.Internal.Abstractions.Extensions;
using Plato.Internal.Data.Abstractions;
using Plato.Internal.Models.Roles;
using Plato.Internal.Models.Users;
using Plato.Internal.Repositories.Roles;

namespace Plato.Internal.Repositories.Users
{
    public class UserRolesRepository : IUserRolesRepository<UserRole>
    {
        #region "Constructor"

        private readonly IDbContext _dbContext;
        private readonly IRoleRepository<Role> _rolesRepository;
        private readonly ILogger<UserSecretRepository> _logger;

        public UserRolesRepository(
            IDbContext dbContext,
            IRoleRepository<Role> rolesRepository,
            ILogger<UserSecretRepository> logger)
        {
            _dbContext = dbContext;
            _rolesRepository = rolesRepository;
            _logger = logger;
        }

        #endregion
        
        #region "Implementation"
        
        public async Task<bool> DeleteAsync(int id)
        {
            bool success;
            using (var context = _dbContext)
            {
                success = await context.ExecuteScalarAsync<bool>(
                    CommandType.StoredProcedure,
                    "DeleteUserRoleById", id);
            }
            return success;
        }

        public async Task<UserRole> InsertUpdateAsync(UserRole userRole)
        {
            var id = 0;
            id = await InsertUpdateInternal(
                userRole.Id,
                userRole.UserId,
                userRole.RoleId);

            if (id > 0)
                return await SelectByIdAsync(id);

            return null;
        }

        public async Task<UserRole> SelectByIdAsync(int id)
        {
            UserRole userRole = null;
            using (var context = _dbContext)
            {
                var reader = await context.ExecuteReaderAsync(
                    CommandType.StoredProcedure,
                    "SelectUserRoleById", id);

                if ((reader != null) && (reader.HasRows))
                {
                    await reader.ReadAsync();
                    userRole = new UserRole();
                    userRole.PopulateModel(reader);
                }
            }

            return userRole;
        }

        public async Task<IEnumerable<UserRole>> SelectUserRolesByUserId(int userId)
        {
            var userRoles = new List<UserRole>();
            using (var context = _dbContext)
            {
                var reader = await context.ExecuteReaderAsync(
                    CommandType.StoredProcedure,
                    "SelectUserRolesByUserId", userId);

                if ((reader != null) && (reader.HasRows))
                {
                    await reader.ReadAsync();
                    var userRole = new UserRole();
                    userRole.PopulateModel(reader);
                    userRoles.Add(userRole);
                }
            }

            return userRoles;
        }
        
        public async Task<IEnumerable<UserRole>> InsertUserRolesAsync(
            int userId, IEnumerable<string> roleNames)
        {

            List<UserRole> userRoles = null;
            foreach (var roleName in roleNames)
            {
                var role = _rolesRepository.SelectByNameAsync(roleName);
                if (role != null)
                {
                    var userRole = await InsertUpdateAsync(new UserRole()
                    {
                        UserId = userId,
                        RoleId = role.Id
                    });
                    if (userRoles == null)
                        userRoles = new List<UserRole>();
                    userRoles.Add(userRole);
                }
            }

            return userRoles;

        }

        public async Task<IEnumerable<UserRole>> InsertUserRolesAsync(int userId, IEnumerable<int> roleIds)
        {
            List<UserRole> userRoles = null;
            foreach (var roleId in roleIds)
            {
                var role = _rolesRepository.SelectByIdAsync(roleId);
                if (role != null)
                {
                    var userRole = await InsertUpdateAsync(new UserRole()
                    {
                        UserId = userId,
                        RoleId = role.Id
                    });
                    if (userRoles == null)
                        userRoles = new List<UserRole>();
                    userRoles.Add(userRole);
                }
            }

            return userRoles;
        }
        
        public async Task<bool> DeletetUserRolesAsync(int userId)
        {
            bool success;
            using (var context = _dbContext)
            {
                success = await context.ExecuteScalarAsync<bool>(
                    CommandType.StoredProcedure,
                    "DeleteUserRolesByUserId", userId);
            }
            return success;
        }
        
        public async Task<bool> DeletetUserRole(int userId, int roleId)
        {

            var success = 0;
            using (var context = _dbContext)
            {
                success = await context.ExecuteScalarAsync<int>(
                    CommandType.StoredProcedure,
                    "DeleteUserRoleByUserIdAndRoleId",
                    userId,
                    roleId);
            }

            return success > 0 ? true : false;

        }
        
        public Task<IPagedResults<TModel>> SelectAsync<TModel>(params object[] inputParams) where TModel : class
        {
            throw new NotImplementedException();
        }

        public Task<IPagedResults<UserRole>> SelectAsync(params object[] inputParams)
        {
            throw new NotImplementedException();
        }


        #endregion

        #region "Private Methods"

        private async Task<int> InsertUpdateInternal(
            int id,
            int userId,
            int roleId)
        {
            using (var context = _dbContext)
            {
                return await context.ExecuteScalarAsync<int>(
                    CommandType.StoredProcedure,
                    "InsertUpdateUserRole",
                    id,
                    userId,
                    roleId);
            }
        }


        #endregion
    }
}