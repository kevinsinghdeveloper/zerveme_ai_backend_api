using zervemedata.Data.Enumerations;

namespace zervemedata.Data.DataModels.UpdateMeta
{
    using zervemedata.Core.Contracts.Abstractions;

    public class JobUpdateMetaData : IUpdateMetaData
    {
        public Guid Id { get; set; }
        public Guid? JobFreqTypeId { get; set; }
        public string CurrentUser { get; set; } = null!;
    }
}