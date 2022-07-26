namespace ExampleAPI.Common.Domain;

public interface IRepository<T> where T : Entity {

    /// <summary>
    /// Add an entity to the repository
    /// </summary>
    Task Add(T entity);

    /// <summary>
    /// Get a specific entity, given its id
    /// </summary>
    Task<T?> Get(Guid id);

    /// <summary>
    /// Save the updated entity to the repository
    /// </summary>
    Task Save(T entity);

    /// <summary>
    /// Remove the entity from the repository
    /// </summary>
    Task Remove(T entity);

    /// <summary>
    /// Get all of the entities in the repository
    /// </summary>
    Task<IEnumerable<T>> GetAll();

}
