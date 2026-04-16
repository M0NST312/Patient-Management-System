using System.Linq.Expressions;
using ClinicSystem.Application.Common.Interfaces;
using ClinicSystem.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace ClinicSystem.Infrastructure.Repositories;

public class Repository<T>(ApplicationDbContext context) : IRepository<T> where T : class
{
    protected readonly DbSet<T> DbSet = context.Set<T>();

    public virtual async Task<T?> GetByIdAsync(Guid id, CancellationToken ct = default) => await DbSet.FindAsync([id], ct);
    public virtual async Task<IEnumerable<T>> GetAllAsync(CancellationToken ct = default) => await DbSet.ToListAsync(ct);
    public virtual async Task<IEnumerable<T>> FindAsync(Expression<Func<T, bool>> predicate, CancellationToken ct = default) => await DbSet.Where(predicate).ToListAsync(ct);
    public virtual async Task<T?> FirstOrDefaultAsync(Expression<Func<T, bool>> predicate, CancellationToken ct = default) => await DbSet.FirstOrDefaultAsync(predicate, ct);
    public virtual async Task<int> CountAsync(Expression<Func<T, bool>>? predicate = null, CancellationToken ct = default) => predicate is null ? await DbSet.CountAsync(ct) : await DbSet.CountAsync(predicate, ct);
    public virtual async Task AddAsync(T entity, CancellationToken ct = default) => await DbSet.AddAsync(entity, ct);
    public virtual void Update(T entity) => DbSet.Update(entity);
    public virtual void Delete(T entity) => DbSet.Remove(entity);
    public virtual async Task<int> SaveChangesAsync(CancellationToken ct = default) => await context.SaveChangesAsync(ct);
}
