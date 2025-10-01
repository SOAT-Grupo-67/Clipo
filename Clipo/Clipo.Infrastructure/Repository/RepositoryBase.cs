using Clipo.Domain.AggregatesModel.Base;
using Clipo.Domain.AggregatesModel.Base.Interface;
using Clipo.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Npgsql;

namespace Clipo.Infrastructure.Repository
{
    public class RepositoryBase<T> : IRepositoryBase<T> where T : BaseModel
    {
        protected readonly ApplicationContext _ctx;
        protected readonly ILogger<RepositoryBase<T>> _logger;
        protected readonly DbSet<T> _set;

        public RepositoryBase(ApplicationContext ctx, ILogger<RepositoryBase<T>> logger)
        {
            _ctx = ctx;
            _logger = logger;
            _set = ctx.Set<T>();
        }

        public virtual async Task<T?> GetByIdAsync(long id, CancellationToken ct = default)
        {
            try
            {
                _logger.LogInformation("Fetching {Entity} with id {Id}", typeof(T).Name, id);
                T? entity = await _set.AsNoTracking()
                                 .FirstOrDefaultAsync(e => e.Id == id, ct);

                return entity;
            }
            catch(NpgsqlException ex)
            {
                _logger.LogError(ex, "PostgreSQL error while fetching id {Id}", id);
                throw;
            }
            catch(Exception ex)
            {
                _logger.LogError(ex, "Unknown error while fetching id {Id}", id);
                throw;
            }
        }

        public virtual async Task<PaginatedResult<T>> GetAllAsync(
            PaginationParams p,
            CancellationToken ct = default)
        {
            try
            {
                _logger.LogInformation(
                    "Paginated fetch for {Entity}: page {Page}, size {Size}",
                    typeof(T).Name, p.PageNumber, p.PageSize);

                IQueryable<T> query = _set.AsNoTracking().Where(e => e.Status != 0);

                int total = await query.CountAsync(ct);

                List<T> items = await query
                    .Skip((p.PageNumber - 1) * p.PageSize)
                    .Take(p.PageSize)
                    .ToListAsync(ct);

                return new PaginatedResult<T>(items, total, p.PageNumber, p.PageSize);
            }
            catch(NpgsqlException ex)
            {
                _logger.LogError(ex, "PostgreSQL error while listing entities");
                throw;
            }
            catch(Exception ex)
            {
                _logger.LogError(ex, "Unknown error while listing entities");
                throw;
            }
        }

        public virtual async Task<T> AddAsync(T entity, CancellationToken ct = default)
        {
            try
            {
                _logger.LogInformation("Adding new {Entity}", typeof(T).Name);

                await _set.AddAsync(entity, ct);
                await _ctx.SaveChangesAsync(ct);

                return entity;
            }
            catch(NpgsqlException ex)
            {
                _logger.LogError(ex, "PostgreSQL error while adding a new entity");
                throw;
            }
            catch(Exception ex)
            {
                _logger.LogError(ex, "Unknown error while adding a new entity");
                throw;
            }
        }

        public virtual async Task<T?> UpdateAsync(T entity, CancellationToken ct = default)
        {
            try
            {
                _logger.LogInformation("Updating {Entity} with id {Id}", typeof(T).Name, entity.Id);

                T? tracked = await _set.FirstOrDefaultAsync(e => e.Id == entity.Id, ct);
                if(tracked is null)
                {
                    _logger.LogWarning("Entity id {Id} not found for update", entity.Id);
                    return null;
                }

                _ctx.Entry(tracked).CurrentValues.SetValues(entity);
                tracked.UpdatedAt = DateTime.UtcNow;

                await _ctx.SaveChangesAsync(ct);
                return tracked;
            }
            catch(NpgsqlException ex)
            {
                _logger.LogError(ex, "PostgreSQL error while updating id {Id}", entity.Id);
                throw;
            }
            catch(Exception ex)
            {
                _logger.LogError(ex, "Unknown error while updating id {Id}", entity.Id);
                throw;
            }
        }

        public virtual async Task<bool> DeleteAsync(long id, CancellationToken ct = default)
        {
            try
            {
                _logger.LogInformation(
                    "Logical delete of {Entity} with id {Id}", typeof(T).Name, id);

                T? entity = await _set.FirstOrDefaultAsync(e => e.Id == id && e.Status != 0, ct);
                if(entity is null)
                {
                    _logger.LogWarning("Entity id {Id} not found for delete", id);
                    return false;
                }

                entity.Status = 0;
                entity.UpdatedAt = DateTime.UtcNow;

                await _ctx.SaveChangesAsync(ct);
                return true;
            }
            catch(NpgsqlException ex)
            {
                _logger.LogError(ex, "PostgreSQL error while deleting id {Id}", id);
                throw;
            }
            catch(Exception ex)
            {
                _logger.LogError(ex, "Unknown error while deleting id {Id}", id);
                throw;
            }
        }
    }
}