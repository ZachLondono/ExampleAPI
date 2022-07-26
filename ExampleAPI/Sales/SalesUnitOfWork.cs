using ExampleAPI.Sales.Companies.Data;
using ExampleAPI.Sales.Orders.Data;
using MediatR;
using System.Data;

namespace ExampleAPI.Sales;

public class SalesUnitOfWork : ISalesUnitOfWork, IDisposable {

    private readonly IDbConnection _connection;
    private IDbTransaction _transaction;
    private IPublisher _publisher;
    private readonly Func<IDbConnection, IDbTransaction, IPublisher, IOrderRepository> _ordersFactory;
    private readonly Func<IDbConnection, IDbTransaction, IPublisher, ICompanyRepository> _companiesFactory;

    public IOrderRepository Orders { get; private set; }
    public ICompanyRepository Companies { get; private set; }

    // TODO: use abstract factory pattern to make the constructor a little cleaner
    public SalesUnitOfWork(NpgsqlSalesConnectionFactory factory,
                            IPublisher publisher,
                            Func<IDbConnection, IDbTransaction, IPublisher, IOrderRepository> ordersFactory,
                            Func<IDbConnection, IDbTransaction, IPublisher, ICompanyRepository> companiesFactory) {

        _connection = factory.CreateConnection();
        _connection.Open();
        _transaction = _connection.BeginTransaction();
        _publisher = publisher;
        _ordersFactory = ordersFactory;
        _companiesFactory = companiesFactory;

        Orders = _ordersFactory(_connection, _transaction, _publisher);
        Companies = _companiesFactory(_connection, _transaction, _publisher);
    }

    public Task CommitAsync() {

        try {
            
            _transaction.Commit();

        } catch {
            
            _transaction.Rollback();
            throw;

        } finally {
            
            _transaction.Dispose();
            _transaction = _connection.BeginTransaction();

            // Reset the repositories, removing any state that they might have had
            Orders = _ordersFactory(_connection, _transaction, _publisher);
            Companies = _companiesFactory(_connection, _transaction, _publisher);

        }

        return Task.CompletedTask;
    }


    public void Dispose() {
        _transaction.Dispose();
        _connection.Dispose();
    }

}