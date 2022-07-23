namespace ExampleAPI.Common;

public interface IRepository<T> where T : Entity {

    Task<T> Create();

    Task<T?> Get(Guid id);

    Task Save(T entity);

    Task Remove(T entity);

    Task<IEnumerable<T>> GetAll();

}
