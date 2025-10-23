using System.ComponentModel.DataAnnotations;
using zervemedata.Data.Entities;

namespace zervemedata.Data.DataModels.CreateMeta
{
    using zervemedata.Core.Contracts.Abstractions;
    using zervemedata.Data.Enumerations;

    public class JobCreationMetaData : ICreationMetaData
    {
        public Guid JobFreqTypeId { get; set; }
        public string CurrentUser { get; set; } = null!;
    }
}