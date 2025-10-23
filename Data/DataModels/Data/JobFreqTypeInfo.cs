using zervemedata.Data.Entities;
using zervemedata.Data.Enumerations;

namespace zervemedata.Data.DataModels.Data
{
    using zervemedata.Core.Contracts.Abstractions;


    public class JobFreqTypeInfo : IResource
    {
        public Guid Id { get; set; }
        public string? Name { get; set; }
    }
}