﻿using System;
using System.Text;
using System.Threading.Tasks;
using Plato.Internal.Data.Abstractions;
using Plato.Internal.Stores.Abstractions;
using Plato.Entities.Metrics.Models;

namespace Plato.Entities.Metrics.Stores
{

    #region "EntityMetricQuery"

    public class EntityMetricQuery : DefaultQuery<EntityMetric>
    {

        private readonly IStore<EntityMetric> _store;

        public EntityMetricQuery(IStore<EntityMetric> store)
        {
            _store = store;
        }

        public EntityMetricQueryParams Params { get; set; }

        public override IQuery<EntityMetric> Select<T>(Action<T> configure)
        {
            var defaultParams = new T();
            configure(defaultParams);
            Params = (EntityMetricQueryParams)Convert.ChangeType(defaultParams, typeof(EntityMetricQueryParams));
            return this;
        }

        public override async Task<IPagedResults<EntityMetric>> ToList()
        {

            var builder = new EntityMetricQueryBuilder(this);
            var populateSql = builder.BuildSqlPopulate();
            var countSql = builder.BuildSqlCount();

            var data = await _store.SelectAsync(
                PageIndex,
                PageSize,
                populateSql,
                countSql,
                Params.IpV4Address.Value,
                Params.IpV6Address.Value,
                Params.UserAgent.Value
            );

            return data;
        }


    }

    #endregion

    #region "EntityMetricQueryParams"

    public class EntityMetricQueryParams
    {


        private WhereInt _id;
        private WhereInt _entityId;

        private WhereString _ipV4Address;
        private WhereString _ipV6Address;
        private WhereString _userAgent;
        private WhereInt _createdUserId;

        public WhereInt Id
        {
            get => _id ?? (_id = new WhereInt());
            set => _id = value;
        }
        public WhereInt EntityId
        {
            get => _entityId ?? (_entityId = new WhereInt());
            set => _entityId = value;
        }
        
        public WhereString IpV4Address
        {
            get => _ipV4Address ?? (_ipV4Address = new WhereString());
            set => _ipV4Address = value;
        }

        public WhereString IpV6Address
        {
            get => _ipV6Address ?? (_ipV6Address = new WhereString());
            set => _ipV6Address = value;
        }

        public WhereString UserAgent
        {
            get => _userAgent ?? (_userAgent = new WhereString());
            set => _userAgent = value;
        }

        public WhereInt CreatedUserId
        {
            get => _createdUserId ?? (_createdUserId = new WhereInt());
            set => _createdUserId = value;
        }


    }

    #endregion

    #region "EntityMetricQueryBuilder"

    public class EntityMetricQueryBuilder : IQueryBuilder
    {
        #region "Constructor"

        private readonly string _entityMetricsTableName;
        private readonly string _entitiesTableName;
        private readonly string _usersTableName;
        private readonly EntityMetricQuery _query;

        public EntityMetricQueryBuilder(EntityMetricQuery query)
        {
            _query = query;
            _entityMetricsTableName = GetTableNameWithPrefix("EntityMetrics");
            _entitiesTableName = GetTableNameWithPrefix("Entities");
            _usersTableName = GetTableNameWithPrefix("Users");

        }

        #endregion

        #region "Implementation"

        public string BuildSqlPopulate()
        {

            var whereClause = BuildWhereClause();
            var orderBy = BuildOrderBy();

            var sb = new StringBuilder();
            sb.Append("SELECT ")
                .Append(BuildPopulateSelect())
                .Append(" FROM ")
                .Append(BuildTables());

            if (!string.IsNullOrEmpty(whereClause))
                sb.Append(" WHERE (").Append(whereClause).Append(")");
            sb.Append(" ORDER BY ")
                .Append(!string.IsNullOrEmpty(orderBy)
                    ? orderBy
                    : "Id ASC");
            sb.Append(" OFFSET @RowIndex ROWS FETCH NEXT @PageSize ROWS ONLY;");
            return sb.ToString();
        }

        public string BuildSqlCount()
        {
            var whereClause = BuildWhereClause();
            var sb = new StringBuilder();
            sb.Append("SELECT COUNT(m.Id) FROM ")
                .Append(BuildTables());
            if (!string.IsNullOrEmpty(whereClause))
                sb.Append(" WHERE (").Append(whereClause).Append(")");
            return sb.ToString();
        }

        string BuildPopulateSelect()
        {
            var sb = new StringBuilder();
            sb.Append("m.*, ")
                .Append("u.UserName,")
                .Append("u.DisplayName,")
                .Append("u.Alias,")
                .Append("u.PhotoUrl,")
                .Append("u.PhotoColor");
            return sb.ToString();

        }

        string BuildTables()
        {
            var sb = new StringBuilder();
            sb.Append(_entityMetricsTableName)
                .Append(" m ");

            // join user
            sb.Append("LEFT OUTER JOIN ")
                .Append(_usersTableName)
                .Append(" u ON m.CreatedUserId = u.Id ");
            return sb.ToString();

        }

        #endregion

        #region "Private Methods"

        private string GetTableNameWithPrefix(string tableName)
        {
            return !string.IsNullOrEmpty(_query.Options.TablePrefix)
                ? _query.Options.TablePrefix + tableName
                : tableName;
        }

        private string BuildWhereClause()
        {
            var sb = new StringBuilder();

            // Id
            if (_query.Params.Id.Value > -1)
            {
                if (!string.IsNullOrEmpty(sb.ToString()))
                    sb.Append(_query.Params.Id.Operator);
                sb.Append(_query.Params.Id.ToSqlString("em.Id"));
            }

            // EntityId
            if (_query.Params.EntityId.Value > -1)
            {
                if (!string.IsNullOrEmpty(sb.ToString()))
                    sb.Append(_query.Params.EntityId.Operator);
                sb.Append(_query.Params.EntityId.ToSqlString("em.EntityId"));
            }

            // IpV4Address
            if (!String.IsNullOrEmpty(_query.Params.IpV4Address.Value))
            {
                if (!string.IsNullOrEmpty(sb.ToString()))
                    sb.Append(_query.Params.IpV4Address.Operator);
                sb.Append(_query.Params.IpV4Address.ToSqlString("em.IpV4Address", "IpV4Address"));
            }

            // IpV6Address
            if (!String.IsNullOrEmpty(_query.Params.IpV6Address.Value))
            {
                if (!string.IsNullOrEmpty(sb.ToString()))
                    sb.Append(_query.Params.IpV6Address.Operator);
                sb.Append(_query.Params.IpV6Address.ToSqlString("em.IpV6Address", "IpV6Address"));
            }

            // UserAgent
            if (!String.IsNullOrEmpty(_query.Params.UserAgent.Value))
            {
                if (!string.IsNullOrEmpty(sb.ToString()))
                    sb.Append(_query.Params.UserAgent.Operator);
                sb.Append(_query.Params.UserAgent.ToSqlString("em.UserAgent", "UserAgent"));
            }

            // CreatedUserId
            if (_query.Params.CreatedUserId.Value > -1)
            {
                if (!string.IsNullOrEmpty(sb.ToString()))
                    sb.Append(_query.Params.CreatedUserId.Operator);
                sb.Append(_query.Params.CreatedUserId.ToSqlString("er.CreatedUserId"));
            }


            return sb.ToString();

        }

        string GetQualifiedColumnName(string columnName)
        {
            if (columnName == null)
            {
                throw new ArgumentNullException(nameof(columnName));
            }

            return columnName.IndexOf('.') >= 0
                ? columnName
                : "er." + columnName;
        }

        private string BuildOrderBy()
        {
            if (_query.SortColumns.Count == 0) return null;
            var sb = new StringBuilder();
            var i = 0;
            foreach (var sortColumn in _query.SortColumns)
            {
                sb.Append(GetQualifiedColumnName(sortColumn.Key));
                if (sortColumn.Value != OrderBy.Asc)
                    sb.Append(" DESC");
                if (i < _query.SortColumns.Count - 1)
                    sb.Append(", ");
                i += 1;
            }
            return sb.ToString();
        }

        #endregion

    }

    #endregion

}