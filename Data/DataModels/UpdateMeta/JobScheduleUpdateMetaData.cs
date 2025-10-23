using zervemedata.Data.Enumerations;

namespace zervemedata.Data.DataModels.UpdateMeta
{
    using zervemedata.Core.Contracts.Abstractions;

    public class JobScheduleUpdateMetaData : IUpdateMetaData
    {
        public Guid JobId { get; set; }
        public string CurrentUser { get; set; } = null!;
        public ScheduleType ScheduleType { get; set; }
    }
}