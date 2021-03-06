﻿using System;
using System.Data;
using Plato.Internal.Abstractions;
using Plato.Internal.Abstractions.Extensions;

namespace Plato.Internal.Models.Abstract
{
    public class DictionaryEntry : IDbModel<DictionaryEntry>
    {
         
        #region "Public Properties"

        public int Id { get; set;  }

        public string Key { get; set; }

        public string Value { get; set; }

        public DateTimeOffset? CreatedDate { get; set; }

        public int CreatedUserId { get; set; }
        
        public DateTimeOffset? ModifiedDate { get; set; }

        public int ModifiedUserId { get; set; }

        #endregion

        #region "Public Methods"

        public void PopulateModel(IDataReader dr)
        {

            if (dr.ColumnIsNotNull("id"))
                Id = Convert.ToInt32(dr["Id"]);
                   
            if (dr.ColumnIsNotNull("Key"))
                Key = Convert.ToString(dr["Key"]);
            
            if (dr.ColumnIsNotNull("Value"))
                Value = Convert.ToString((dr["Value"]));
       
            if (dr.ColumnIsNotNull("CreatedUserId"))
                CreatedUserId = Convert.ToInt32(dr["CreatedUserId"]);

            if (dr.ColumnIsNotNull("CreatedDate"))
                CreatedDate = (DateTimeOffset)dr["CreatedDate"];

            if (dr.ColumnIsNotNull("ModifiedUserId"))
                ModifiedUserId = Convert.ToInt32((dr["ModifiedUserId"]));

            if (dr.ColumnIsNotNull("ModifiedDate"))
                ModifiedDate = (DateTimeOffset)dr["ModifiedDate"];

        }

        public void PopulateModel(Action<DictionaryEntry> model)
        {
            model(this);
        }

        public override string ToString()
        {
            return this.Key;
        }

        #endregion
        
    }
}
