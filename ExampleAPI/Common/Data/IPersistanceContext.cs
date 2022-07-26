namespace ExampleAPI.Common.Data;

public interface IPersistanceContext {
		
	public abstract Task SaveChanges();

}