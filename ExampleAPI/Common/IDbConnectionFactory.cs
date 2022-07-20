using System.Data;

namespace ExampleAPI.Common;

public interface IDbConnectionFactory {

    IDbConnection CreateConnection();

}
