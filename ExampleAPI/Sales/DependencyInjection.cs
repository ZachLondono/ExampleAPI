using ExampleAPI.Common.Data;
using ExampleAPI.Sales.Companies.Data;
using ExampleAPI.Sales.Companies.Domain;
using ExampleAPI.Sales.Data;
using ExampleAPI.Sales.Orders.Data;
using ExampleAPI.Sales.Orders.Domain;

namespace ExampleAPI.Sales;

public static class DependencyInjection {

    public static IServiceCollection ConfigureSales(this IServiceCollection services) {

        services.AddTransient<SalesUnitOfWork>();
        services.AddTransient<SalesPersistanceContext>();

        services.AddTransient<OrderSet>();
        services.AddTransient<CompanySet>();

        services.AddTransient<PersistanceSet<Order>, OrderSet>();
        services.AddTransient<PersistanceSet<Company>, CompanySet>();

        services.AddSingleton<Func<SalesPersistanceContext, OrderRepository>>(s =>(spc) => new(spc));
        services.AddSingleton<Func<SalesPersistanceContext, CompanyRepository>>(s => (spc) => new(spc));

        return services;

    }

}
