namespace zervemedata.Data.Entities.Keys
{
    using zervemedata.Core.Contracts.Abstractions;

    public class JobKey : IResourceKey
    {
        public Guid Id { get; set; }

        public string CurrentUser { get; set; } = null!;

        public string? StatusLog { get; set; } // used for status changing
        
        public bool Equals(JobKey? other)
        {
            if (other == null)
            {
                return false;
            }

            return this.Id == other.Id;
        }
    }
}