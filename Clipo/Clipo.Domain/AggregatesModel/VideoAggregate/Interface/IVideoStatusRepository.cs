using Clipo.Domain.AggregatesModel.Base.Interface;

namespace Clipo.Domain.AggregatesModel.VideoAggregate.Interface
{
    public interface IVideoStatusRepository : IRepositoryBase<VideoStatus>
    {
        List<VideoStatus> GetAllByUser(Guid userId);
        Task<VideoStatus> GetStatusById(long id);
    }
}
