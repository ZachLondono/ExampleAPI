using Npgsql;
using System.Data;

namespace ExampleAPI.Sales;

public class NpgsqlSalesConnectionFactory : IDbConnectionFactory {

    private IConfiguration _config;

    public NpgsqlSalesConnectionFactory(IConfiguration config) {
        _config = config;
    }

    public IDbConnection CreateConnection() {
        string connectionString = _config.GetConnectionString("SalesDatabase");
        return new NpgsqlConnection(connectionString);
    }

}
