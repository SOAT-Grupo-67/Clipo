using Clipo.Domain.Enums;

namespace Clipo.Domain.AggregatesModel.Base
{
    public class BaseModel
    {
        public long Id { get; set; }
        public DateTime CreatedAt { get; private set; }
        public DateTime UpdatedAt { get; set; }
        public Status Status { get; set; }

        public BaseModel()
        {
            CreatedAt = DateTime.Now;
            Status = Status.Active;
        }
    }
}
