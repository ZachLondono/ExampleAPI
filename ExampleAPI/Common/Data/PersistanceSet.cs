using ExampleAPI.Common.Domain;

namespace ExampleAPI.Common.Data;

public abstract class PersistanceSet<TEntity> : IDisposable where TEntity : Entity {

	//TODO: Need to track when entities are created/removed so they can be inserted/deleted
	protected List<TEntity> Entities { get; init; } = new();
	protected List<TEntity> RemovedEntities { get; init; } = new();

	public void Add(TEntity entity) => Entities.Add(entity);

	public void Remove(TEntity entity) {
		if (Entities.Remove(entity)) {
			RemovedEntities.Add(entity);
		}
	}

	public abstract Task<TEntity?> Get(Guid id);

	public abstract Task SaveChanges();

	public abstract void Dispose();
}