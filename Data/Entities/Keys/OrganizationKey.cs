namespace zervemedata.Data.Entities.Keys
{
    using zervemedata.Core.Contracts.Abstractions;

    public class OrganizationKey : IResourceKey
    {
        public Guid Id { get; set; }

        public bool Equals(OrganizationKey? other)
        {
            if (other == null)
            {
                return false;
            }

            return this.Id == other.Id;
        }
    }
}