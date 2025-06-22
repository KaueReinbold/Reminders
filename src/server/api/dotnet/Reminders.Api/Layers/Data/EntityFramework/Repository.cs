namespace Reminders.Infrastructure.Data.EntityFramework;

public class Repository<TEntity>
    : IRepository<TEntity>
    where TEntity : Entity<Guid>
{
    protected readonly DbContext Context;
    protected readonly DbSet<TEntity> DbSet;
    private bool disposed;

    public Repository(IUnitOfWork unitOfWork)
    {
        Context = unitOfWork.GetContext<DbContext>() ?? throw new ArgumentNullException(nameof(unitOfWork));
        DbSet = Context.Set<TEntity>();
    }

    public virtual TEntity Add(TEntity obj) =>
        DbSet.Add(obj).Entity;

    public virtual TEntity Update(TEntity obj) =>
        DbSet.Update(obj).Entity;

    public virtual void Remove(Guid id)
    {
        var entity = DbSet.Find(id);

        if (entity is not null)
        {
            DbSet.Remove(entity);
        }
    }

    public virtual TEntity? Get(Guid id) =>
        DbSet.Find(id);

    public virtual TEntity? GetAsNoTracking(Guid id)
    {
        var entity = Context.Set<TEntity>().Find(id);

        if (entity is null)
            return null;

        Context.Entry(entity).State = EntityState.Detached;

        return entity;
    }

    public bool Exists(Guid id)
    {
        var entity = GetAsNoTracking(id);

        if (entity is null)
            return false;

        return !entity.IsDeleted;
    }

    public virtual IQueryable<TEntity> Get() =>
        DbSet;
    public virtual IQueryable<TEntity> GetAsNoTracking() =>
        DbSet.AsNoTracking();

    public void Dispose()
    {
        Dispose(true);
        GC.SuppressFinalize(this);
    }

    protected virtual void Dispose(bool disposing)
    {
        if (disposed) return;

        if (disposing) Context.Dispose();

        disposed = true;
    }
}
