using ExampleAPI.Common.Data;
using ExampleAPI.Sales.Companies.Domain;
using ExampleAPI.Sales.Data;

namespace ExampleAPI.Sales.Companies.Data;

public class CompanyRepository : IRepository<Company> {

    /*
     *  If I can figure out a clean way to have the PersistanceContext class get the correct PersistanceSet class with a function like Set<TEntity>()
     *  then this class would be obsolete, since it can be replaced with generics, and there can just be a generic `Repository` class
     *  ie ... Add(TEntity entity) => _context.Set<TEntity>(entity);
     */

    private readonly SalesPersistanceContext _context;

    public CompanyRepository(SalesPersistanceContext context) {
        _context = context;
    }

    public void Add(Company entity) => _context.Companies.Add(entity);

    public Task<Company?> Get(Guid id) => _context.Companies.Get(id);

    public void Remove(Company entity) => _context.Companies.Remove(entity);

}