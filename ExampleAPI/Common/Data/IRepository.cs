using ExampleAPI.Common.Domain;

namespace ExampleAPI.Common.Data;

public interface IRepository<T> where T : Entity {

    /// <summary>
    /// Add an entity to the repository
    /// </summary>
    void Add(T entity);

    /// <summary>
    /// Get a specific entity, given its id
    /// </summary>
    Task<T?> Get(Guid id);

    /// <summary>
    /// Remove the entity from the repository
    /// </summary>
    void Remove(T entity);

}