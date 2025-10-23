namespace zervemedata.Data.Entities.Keys
{
    using zervemedata.Core.Contracts.Abstractions;

    public class ReportKey : IResourceKey
    {
        public Guid? Id { get; set; }

        public Guid? ProjectId { get; set; }

        public string CurrentUser { get; set; } = null!;

        public bool Equals(ReportKey? other)
        {
            if (other == null)
            {
                return false;
            }

            return this.Id == other.Id;
        }
    }
}