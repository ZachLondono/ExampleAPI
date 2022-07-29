namespace ExampleAPI.Common.Domain;

public interface IRepository<T> where T : Entity {

    /// <summary>
    /// Add an entity to the repository
    /// </summary>
    Task AddAsync(T entity);

    /// <summary>
    /// Get a specific entity, given its id
    /// </summary>
    Task<T?> GetAsync(Guid id);

    /// <summary>
    /// Save the updated entity to the repository
    /// </summary>
    Task UpdateAsync(T entity);

    /// <summary>
    /// Remove the entity from the repository
    /// </summary>
    Task RemoveAsync(T entity);

    /// <summary>
    /// Get all of the entities in the repository
    /// </summary>
    Task<IEnumerable<T>> GetAllAsync();

}
