using ExampleAPI.Common.Data;
using ExampleAPI.Sales.Companies.Data;
using ExampleAPI.Sales.Orders.Data;
using MediatR;
using System.Data;

namespace ExampleAPI.Sales;

public static class ConfigureServices {

    public static IServiceCollection AddSales(this IServiceCollection services) {

        services.AddSingleton<Func<IDbConnection, IDbTransaction, IOrderRepository>>(s => (c,t) => new OrderRepository(new DapperConnection(c), t));
        services.AddSingleton<Func<IDbConnection, IDbTransaction, ICompanyRepository>>(s => (c, t) => new CompanyRepository(new DapperConnection(c), t));

        services.AddTransient<ISalesUnitOfWork, SalesUnitOfWork>();

        services.AddTransient<NpgsqlSalesConnectionFactory>();

        return services;

    }

}
