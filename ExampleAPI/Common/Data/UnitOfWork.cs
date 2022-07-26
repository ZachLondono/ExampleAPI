
namespace ExampleAPI.Common.Data;

public abstract class UnitOfWork {

    protected IPersistanceContext Context { get; init; }

    public UnitOfWork(IPersistanceContext context) {
        Context = context;
    }

    public async Task Complete() {
        await Context.SaveChanges();
    }

}