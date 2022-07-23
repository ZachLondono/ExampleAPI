namespace ExampleAPI.Common;

public interface IRepository<T> where T : Entity {

    // TODO: Possible improvment; The create method takes T as a parameter, the method itself will deal only with 'inserting' the data into the datastore while the entity itself is responsible for initilizing itself into a valid 'default' state
    Task<T> Create();

    Task<T?> Get(Guid id);

    Task Save(T entity);

    Task Remove(T entity);

    Task<IEnumerable<T>> GetAll();

}
