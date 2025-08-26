using Clipo.Domain.AggregatesModel.VideoAggregate;
using Clipo.Domain.AggregatesModel.VideoAggregate.Interface;
using Clipo.Infrastructure.Data;
using Microsoft.Extensions.Logging;

namespace Clipo.Infrastructure.Repository
{
    public class VideoRepository : RepositoryBase<VideoStatus>, IVideoStatusRepository
    {
        public VideoRepository(ApplicationContext ctx,
                                 ILogger<RepositoryBase<VideoStatus>> log)
            : base(ctx, log) { }
    }
}
