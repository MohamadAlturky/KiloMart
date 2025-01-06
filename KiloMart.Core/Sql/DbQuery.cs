using System.Data;
using Dapper;

namespace KiloMart.Core.Sql;

public class DbQuery<T>(string sql, Dictionary<string, object>? parameters = null)
{
    protected string _sql = sql;
    protected List<string> _whereConditions = [];
    protected Dictionary<string, object> _parameters = parameters ?? [];
    protected string? _orderBy = null;
    protected long? _offset = null;
    protected int? _take = null;

    public DbQuery<T> Where(string condition, object parameter)
    {
        foreach (var prop in parameter.GetType().GetProperties())
        {
            var value = prop.GetValue(parameter);
            var paramName = $"@{prop.Name}";
            if (value is null)
            {
                return this;
            }
            _parameters[paramName] = value;
        }
        _whereConditions.Add(condition);
        return this;
    }

    public DbQuery<T> Where(string condition)
    {
        _whereConditions.Add(condition);
        return this;
    }

    public DbQuery<T> OrderBy(string orderBy)
    {
        _orderBy = orderBy;
        return this;
    }

    public DbQuery<T> Page(long? offset, int? take)
    {
        _offset = offset;
        _take = take;
        _orderBy ??= $"{typeof(T).GetProperties().FirstOrDefault()?.Name} DESC";
        return this;
    }

    #region Virtual

    public virtual List<T> ToList(IDbConnection connection, IDbTransaction? transaction = null)
    {
        var sql = BuildSql();
        System.Console.WriteLine(sql);
        return connection.Query<T>(sql, _parameters, transaction: transaction).ToList();
    }

    public virtual T? FirstOrDefault(IDbConnection connection, IDbTransaction? transaction = null)
    {
        var sql = BuildSql();
        System.Console.WriteLine(sql);
        return connection.QueryFirstOrDefault<T>(sql, _parameters, transaction: transaction);
    }

    public virtual async Task<List<T>> ToListAsync(IDbConnection connection, IDbTransaction? transaction = null)
    {
        var sql = BuildSql();
        System.Console.WriteLine(sql);
        var result = await connection.QueryAsync<T>(sql, _parameters, transaction: transaction);
        return result.ToList();
    }

    public virtual async Task<T?> FirstOrDefaultAsync(IDbConnection connection, IDbTransaction? transaction = null)
    {
        var sql = BuildSql();
        System.Console.WriteLine(sql);
        return await connection.QueryFirstOrDefaultAsync<T>(sql, _parameters, transaction: transaction);
    }
    #endregion 

    #region Count

    public async Task<long?> CountAsync(IDbConnection connection, IDbTransaction? transaction = null)
    {
        var sql = BuildCountSql();
        System.Console.WriteLine(sql);
        return await connection.ExecuteScalarAsync<long>(sql, _parameters, transaction: transaction);
    }

    public long? Count(IDbConnection connection, IDbTransaction? transaction = null)
    {
        var sql = BuildCountSql();
        System.Console.WriteLine(sql);

        return connection.ExecuteScalar<long>(sql, _parameters, transaction: transaction);
    }
    #endregion
    
    #region old
    // public string BuildSql()
    // {
    //     var whereClause = string.Empty;
    //     if (_whereConditions.Any())
    //     {
    //         whereClause = $"WHERE {string.Join(" AND ", _whereConditions)}";
    //     }

    //     var orderByClause = _orderBy != null ? $"ORDER BY {_orderBy}" : "ORDER BY (SELECT NULL)";

    //     // Main query
    //     var mainQuery = $"{_sql} {whereClause} {orderByClause}";

    //     // Wrap the main query in a CTE with row number
    //     var pagedSql = $@"
    //             WITH PagedQuery AS (
    //                 SELECT *,
    //                     ROW_NUMBER() OVER ({orderByClause}) AS RowNum
    //                 FROM ({mainQuery}) AS SubQuery
    //             )
    //             SELECT * FROM PagedQuery
    //             WHERE RowNum > @Offset AND RowNum <= @Take + @Offset";

    //     // Add parameters for offset and take
    //     _parameters[$"@Offset"] = _offset;
    //     _parameters[$"@Take"] = _take;

    //     return pagedSql;
    // }
    #endregion


    #region SQL
    public string BuildSql()
    {
        var whereClause = string.Empty;
        if (_whereConditions.Any())
        {
            whereClause = $"WHERE {string.Join(" AND ", _whereConditions)}";
        }

        var orderByClause = _orderBy != null ? $"ORDER BY {_orderBy}" : "";
        //"ORDER BY (SELECT NULL)";

        // Main query with OFFSET and FETCH NEXT
        if (_offset is not null && _take is not null)
        {
            var pagedSql = $"{_sql} {whereClause} {orderByClause} " +
               @"OFFSET @Offset ROWS
                FETCH NEXT @Take ROWS ONLY;";

            // Add parameters for offset and take
            _parameters["@Offset"] = _offset;
            _parameters["@Take"] = _take;
            return pagedSql;
        }
        return $"{_sql} {whereClause} {orderByClause}";
    }

    public string BuildCountSql()
    {
        var whereClause = string.Empty;
        if (_whereConditions.Any())
        {
            whereClause = $"WHERE {string.Join(" AND ", _whereConditions)}";
        }

        // Main query for counting
        var countQuery = $"{_sql} {whereClause}";

        // Wrap the main query in a count statement
        var countSql = $@"SELECT COUNT(*) FROM ({countQuery}) AS SubQuery";

        // Parameters for the where clause are already in _parameters

        return countSql;
    }
    #endregion
}

// public class DbQuery<TFirst, TSecond, TResult>
//     (string sql, Func<TFirst, TSecond, TResult> map ,Dictionary<string, object>? parameters = null) 
//         : DbQuery<TResult>(sql, parameters)
// {
//     public override TResult? FirstOrDefault(IDbConnection connection, IDbTransaction? transaction = null)
//     {
//          var sql = BuildSql();
//         System.Console.WriteLine(sql);
//         return connection.QueryFirstOrDefault<TFirst,TSecond,TResult>(sql, _parameters, transaction: transaction,map);
//     }
// }