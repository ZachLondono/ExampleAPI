using Npgsql;
using System.Data;

namespace ExampleAPI.Common;

public class NpgsqlOrderConnectionFactory : IDbConnectionFactory {

    private IConfiguration _config;

    public NpgsqlOrderConnectionFactory(IConfiguration config) {
        _config = config;
    }

    public IDbConnection CreateConnection() {
        string connectionString = _config.GetConnectionString("OrderDatabase");
        return new NpgsqlConnection(connectionString);
    }

}
