using System.Data;

namespace ExampleAPI.Common.Data;

public interface IDbConnectionFactory {

    IDbConnection CreateConnection();

}
