using Dapper;
using System.Data;

namespace ExampleAPI.Common.Data;

public class DapperConnection : IDapperConnection {

    public readonly IDbConnection _connection;

    public DapperConnection(IDbConnection connection) {
        _connection = connection;
    }


    public Task<int> ExecuteAsync(string sql, object? param = null, IDbTransaction? transaction = null, int? commandTimeout = null, CommandType? commandType = null) {
        return _connection.ExecuteAsync(sql, param, transaction, commandTimeout, commandType);
    }

    public Task<IEnumerable<T>> QueryAsync<T>(string sql, object? param = null, IDbTransaction? transaction = null, int? commandTimeout = null, CommandType? commandType = null) {
        return _connection.QueryAsync<T>(sql, param, transaction, commandTimeout, commandType);
    }

    public Task<IEnumerable<T3>> QueryAsync<T1, T2, T3>(string sql, Func<T1, T2, T3> map, object? param = null, IDbTransaction? transaction = null, bool buffered = true, string splitOn = "Id", int? commandTimeout = null, CommandType? commandType = null) {
        return _connection.QueryAsync<T1, T2, T3>(sql, map, param, transaction, buffered, splitOn, commandTimeout, commandType);
    }

    public Task<T> QuerySingleOrDefaultAsync<T>(string sql, object? param = null, IDbTransaction? transaction = null, int? commandTimeout = null, CommandType? commandType = null) {
        return _connection.QuerySingleOrDefaultAsync<T>(sql, param, transaction, commandTimeout, commandType);
    }

}
