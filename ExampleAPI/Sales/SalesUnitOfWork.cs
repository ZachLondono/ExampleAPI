using ExampleAPI.Common.Data;
using ExampleAPI.Sales.Companies.Data;
using ExampleAPI.Sales.Orders.Data;
using MediatR;
using System.Data;

namespace ExampleAPI.Sales;

public class SalesUnitOfWork : ISalesUnitOfWork, IDisposable {

    private readonly IDbConnection _connection;
    private IDbTransaction _transaction;
    private IPublisher _publisher;

    public IOrderRepository Orders { get; private set; }
    public ICompanyRepository Companies { get; private set; }

    public SalesUnitOfWork(NpgsqlOrderConnectionFactory factory, IPublisher publisher) {

        _connection = factory.CreateConnection();
        _connection.Open();
        _transaction = _connection.BeginTransaction();
        _publisher = publisher;

        // TODO: Use Func<> delegat to create the repositories so the constructor is not responsible for directly creating it's dependencies, and so we can use the interfaces instead of concreate classes
        Orders = new OrderRepository(_connection, _transaction, _publisher);
        Companies = new CompanyRepository(_connection, _transaction, _publisher);
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
            Orders = new OrderRepository(_connection, _transaction, _publisher);
            Companies = new CompanyRepository(_connection, _transaction, _publisher);

        }

        return Task.CompletedTask;
    }


    public void Dispose() {
        _transaction.Dispose();
        _connection.Dispose();
    }

}