using Clipo.Domain.AggregatesModel.VideoAggregate;
using Clipo.Domain.AggregatesModel.VideoAggregate.Interface;
using Clipo.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Npgsql;
using System.Threading.Tasks;

namespace Clipo.Infrastructure.Repository
{
    public class VideoRepository : RepositoryBase<VideoStatus>, IVideoStatusRepository
    {
        public VideoRepository(ApplicationContext ctx,
                                 ILogger<RepositoryBase<VideoStatus>> log)
            : base(ctx, log) { }

        public List<VideoStatus> GetAllByUser(Guid userId) {
            try
            {
                return _set.AsNoTracking()
                          .Where(v => v.UserId == userId)
                          .OrderByDescending(v => v.CreatedAt)
                          .ToList();
            }
            catch (NpgsqlException ex)
            {
                _logger.LogError(ex, "PostgreSQL error while fetching videos for user {UserId}", userId);
                throw;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Unknown error while fetching videos for user {UserId}", userId);
                throw;
            }
        }

        public async Task<VideoStatus> GetStatusById(long id) {
            try {
                return await _set.AsNoTracking()
                          .FirstOrDefaultAsync(v => v.Id == id);
            }
            catch (NpgsqlException ex) {
                _logger.LogError(ex, "PostgreSQL error while fetching video for id {id}", id);
                throw;
            }
            catch (Exception ex) {
                _logger.LogError(ex, "PostgreSQL error while fetching video for id {id}", id);
                throw;
            }
        }
    }
}
