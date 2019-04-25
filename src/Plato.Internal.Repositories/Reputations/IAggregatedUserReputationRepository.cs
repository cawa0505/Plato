﻿using System;
using System.Threading.Tasks;
using Plato.Internal.Models.Metrics;
using Plato.Internal.Repositories.Metrics;

namespace Plato.Internal.Repositories.Reputations
{
    public interface IAggregatedUserReputationRepository : IAggregatedRepository
    {

        Task<AggregatedResult<int>> SelectSummedByInt(
            string groupBy,
            DateTimeOffset start,
            DateTimeOffset end);

    }

}
