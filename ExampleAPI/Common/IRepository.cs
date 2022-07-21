namespace ExampleAPI.Common;

public interface IRepository<T> where T : Entity {

    Task<T> Create();

    Task<T?> Get(int id);

    Task<T> Save(T entity);

    Task Remove(T entity);

    Task<IEnumerable<T>> GetAll();

}
