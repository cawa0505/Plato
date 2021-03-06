﻿using System;
using System.Data;
using Plato.Internal.Abstractions;
using Plato.Internal.Abstractions.Extensions;

namespace Plato.Internal.Models.Reputations
{

    public class UserReputation : IDbModel<UserReputation>
    {

        public int Id { get; set; }

        public int FeatureId { get; set; }

        public string Name { get; set; }

        public string Description { get; set; }
        
        public int Points { get; set; }

        public int CreatedUserId { get; set; }

        public DateTimeOffset? CreatedDate { get; set; }

        public void PopulateModel(IDataReader dr)
        {

            if (dr.ColumnIsNotNull("Id"))
                Id = Convert.ToInt32(dr["Id"]);

            if (dr.ColumnIsNotNull("FeatureId"))
                Id = Convert.ToInt32(dr["FeatureId"]);

            if (dr.ColumnIsNotNull("Name"))
                Name = Convert.ToString(dr["Name"]);
            
            if (dr.ColumnIsNotNull("Description"))
                Description = Convert.ToString(dr["Description"]);

            if (dr.ColumnIsNotNull("Points"))
                Points = Convert.ToInt32(dr["Points"]);
            
            if (dr.ColumnIsNotNull("CreatedUserId"))
                CreatedUserId = Convert.ToInt32(dr["CreatedUserId"]);
            
            if (dr.ColumnIsNotNull("CreatedDate"))
                CreatedDate = (DateTimeOffset)dr["CreatedDate"];

        }
        
    }

}
