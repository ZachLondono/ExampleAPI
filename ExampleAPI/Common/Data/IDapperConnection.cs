using System.Data;

namespace ExampleAPI.Common.Data;

public interface IDapperConnection {
    Task<int> ExecuteAsync(string sql, object? param = null, IDbTransaction? transaction = null, int? commandTimeout = null, CommandType? commandType = null);
    Task<IEnumerable<T>> QueryAsync<T>(string sql, object? param = null, IDbTransaction? transaction = null, int? commandTimeout = null, CommandType? commandType = null);
    Task<T> QuerySingleOrDefaultAsync<T>(string sql, object? param = null, IDbTransaction? transaction = null, int? commandTimeout = null, CommandType? commandType = null);
    Task<IEnumerable<T3>> QueryAsync<T1, T2, T3>(string sql, Func<T1, T2, T3> map, object? param = null, IDbTransaction? transaction = null, bool buffered = true, string splitOn= "Id", int? commandTimeout = null, CommandType? commandType = null);
}
