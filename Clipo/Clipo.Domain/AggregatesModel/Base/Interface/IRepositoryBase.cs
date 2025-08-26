namespace Clipo.Domain.AggregatesModel.Base.Interface
{
    public interface IRepositoryBase<T> where T : BaseModel
    {
        Task<T?> GetByIdAsync(long id, CancellationToken ct = default);

        Task<PaginatedResult<T>> GetAllAsync(PaginationParams pageParams, CancellationToken ct = default);

        Task<T> AddAsync(T entity, CancellationToken ct = default);

        Task<T?> UpdateAsync(T entity, CancellationToken ct = default);

        Task<bool> DeleteAsync(long id, CancellationToken ct = default);
    }
}
