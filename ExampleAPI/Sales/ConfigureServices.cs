using ExampleAPI.Common.Data;
using ExampleAPI.Sales.Companies.Data;
using ExampleAPI.Sales.Orders.Data;
using MediatR;
using System.Data;

namespace ExampleAPI.Sales;

public static class ConfigureServices {

    public static IServiceCollection AddSales(this IServiceCollection services) {

        services.AddSingleton<Func<IDbConnection, IDbTransaction, IPublisher, IOrderRepository>>(s => (c,t,p) => new OrderRepository(new DapperConnection(c), t,p));
        services.AddSingleton<Func<IDbConnection, IDbTransaction, IPublisher, ICompanyRepository>>(s => (c, t, p) => new CompanyRepository(new DapperConnection(c), t, p));

        services.AddTransient<ISalesUnitOfWork, SalesUnitOfWork>();

        services.AddTransient<NpgsqlSalesConnectionFactory>();

        return services;

    }

}
