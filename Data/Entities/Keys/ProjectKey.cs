namespace zervemedata.Data.Entities.Keys
{
    using zervemedata.Core.Contracts.Abstractions;

    public class ProjectKey : IResourceKey
    {
        public Guid? Id { get; set; }

        public string CurrentUser { get; set; } = null!;

        public bool Equals(ProjectKey? other)
        {
            if (other == null)
            {
                return false;
            }

            return this.Id == other.Id;
        }
    }
}