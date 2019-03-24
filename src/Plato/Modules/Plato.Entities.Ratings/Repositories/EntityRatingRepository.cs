﻿using System;
using System.Collections.Generic;
using System.Data;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Plato.Entities.Ratings.Models;
using Plato.Internal.Abstractions.Extensions;
using Plato.Internal.Data.Abstractions;

namespace Plato.Entities.Ratings.Repositories
{
    
    public class EntityRatingRepository : IEntityRatingsRepository<EntityRating>
    {

        private readonly IDbContext _dbContext;
        private readonly ILogger<EntityRatingRepository> _logger;

        public EntityRatingRepository(
            IDbContext dbContext,
            ILogger<EntityRatingRepository> logger)
        {
            _dbContext = dbContext;
            _logger = logger;
        }

        #region "Implementation"

        public async Task<EntityRating> InsertUpdateAsync(EntityRating rating)
        {
            if (rating == null)
            {
                throw new ArgumentNullException(nameof(rating));
            }

            var id = await InsertUpdateInternal(
                rating.Id,
                rating.Rating,
                rating.FeatureId,
                rating.EntityId,
                rating.EntityReplyId,
                rating.IpV4Address,
                rating.IpV6Address,
                rating.UserAgent,
                rating.CreatedUserId,
                rating.CreatedDate);

            if (id > 0)
            {
                // return
                return await SelectByIdAsync(id);
            }

            return null;
        }

        public async Task<EntityRating> SelectByIdAsync(int id)
        {
            EntityRating entityRating = null;
            using (var context = _dbContext)
            {
                entityRating = await context.ExecuteReaderAsync<EntityRating>(
                    CommandType.StoredProcedure,
                    "SelectEntityRatingById",
                    async reader =>
                    {
                        if ((reader != null) && (reader.HasRows))
                        {
                            await reader.ReadAsync();
                            entityRating = new EntityRating();
                            entityRating.PopulateModel(reader);
                        }

                        return entityRating;
                    },
                    id);

            }

            return entityRating;

        }

        public async Task<IPagedResults<EntityRating>> SelectAsync(params object[] inputParams)
        {
            IPagedResults<EntityRating> output = null;
            using (var context = _dbContext)
            {
                output = await context.ExecuteReaderAsync<IPagedResults<EntityRating>>(
                    CommandType.StoredProcedure,
                    "SelectEntityRatingsPaged",
                    async reader =>
                    {
                        if ((reader != null) && (reader.HasRows))
                        {
                            output = new PagedResults<EntityRating>();
                            while (await reader.ReadAsync())
                            {
                                var rating = new EntityRating();
                                rating.PopulateModel(reader);
                                output.Data.Add(rating);
                            }

                            if (await reader.NextResultAsync())
                            {
                                if (reader.HasRows)
                                {
                                    await reader.ReadAsync();
                                    output.PopulateTotal(reader);
                                }
                            }

                        }

                        return output;
                    },
                    inputParams);
            
            }

            return output;

        }

        public async Task<bool> DeleteAsync(int id)
        {
            if (_logger.IsEnabled(LogLevel.Information))
            {
                _logger.LogInformation($"Deleting entity rating with id: {id}");
            }

            var success = 0;
            using (var context = _dbContext)
            {
                success = await context.ExecuteScalarAsync<int>(
                    CommandType.StoredProcedure,
                    "DeleteEntityRatingById", id);
            }

            return success > 0 ? true : false;
        }


        public async Task<IEnumerable<EntityRating>> SelectEntityRatingsByEntityId(int entityId)
        {

            IList<EntityRating> output = null;
            using (var context = _dbContext)
            {
                output = await context.ExecuteReaderAsync<IList<EntityRating>>(
                    CommandType.StoredProcedure,
                    "SelectEntityRatingsByEntityId",
                    async reader =>
                    {
                        if ((reader != null) && (reader.HasRows))
                        {
                            output = new List<EntityRating>();
                            while (await reader.ReadAsync())
                            {
                                var entity = new EntityRating();
                                entity.PopulateModel(reader);
                                output.Add(entity);
                            }
                        }

                        return output;
                    },
                    entityId);
            
            }

            return output;

        }

        public async Task<IEnumerable<EntityRating>> SelectEntityRatingsByUserIdAndEntityId(int userId, int entityId)
        {

            IList<EntityRating> output = null;
            using (var context = _dbContext)
            {
                output = await context.ExecuteReaderAsync<IList<EntityRating>>(
                    CommandType.StoredProcedure,
                    "SelectEntityRatingsByUserIdAndEntityId",
                    async reader =>
                    {
                        if ((reader != null) && (reader.HasRows))
                        {
                            output = new List<EntityRating>();
                            while (await reader.ReadAsync())
                            {
                                var entity = new EntityRating();
                                entity.PopulateModel(reader);
                                output.Add(entity);
                            }
                        }

                        return output;
                    },
                    userId,
                    entityId);
            
            }

            return output;

        }

        #endregion

        #region "Private Methods"

        async Task<int> InsertUpdateInternal(
            int id,
            int rating,
            int featureId,
            int entityId,
            int entityReplyId,
            string ipV4Address,
            string ipV6Address,
            string userAgent,
            int createdUserId,
            DateTimeOffset? createdDate)
        {

            var emailId = 0;
            using (var context = _dbContext)
            {
                emailId = await context.ExecuteScalarAsync<int>(
                    CommandType.StoredProcedure,
                    "InsertUpdateEntityRating",
                    id,
                    rating,
                    featureId,
                    entityId,
                    entityReplyId,
                    ipV4Address.TrimToSize(20).ToEmptyIfNull(),
                    ipV6Address.TrimToSize(50).ToEmptyIfNull(),
                    userAgent.TrimToSize(255).ToEmptyIfNull(),
                    createdUserId,
                    createdDate.ToDateIfNull(),
                    new DbDataParameter(DbType.Int32, ParameterDirection.Output)
                );
            }

            return emailId;

        }

        #endregion

    }

}