﻿using System.Collections.Generic;
using Plato.Internal.Models.Reputations;
using Plato.Internal.Reputations.Abstractions;

namespace Plato.Articles.Star
{

    public class Reputations : IReputationsProvider<Reputation>
    {

        public static readonly Reputation StarArticle =
            new Reputation("Star Article", 1);

        public static readonly Reputation StarredArticle =
            new Reputation("Starred Article", 2);
        
        public IEnumerable<Reputation> GetReputations()
        {
            return new[]
            {
                StarArticle,
                StarredArticle
            };
        }

    }

}
