namespace Clipo.Domain.AggregatesModel.Base
{
    public sealed record PaginationParams(
    int PageNumber = 1,
    int PageSize = 20,
    string? Search = null);

    public sealed record PaginatedResult<T>(
    IReadOnlyList<T> Items,
    int TotalCount,
    int PageNumber,
    int PageSize);
}
