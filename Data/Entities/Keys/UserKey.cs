namespace zervemedata.Data.Entities.Keys
{
    using zervemedata.Core.Contracts.Abstractions;
    
    public class UserKey : IResourceKey
    {
        public string? Id { get; set; } = null;

        public string CurrentUser { get; set; } = null!;
        
        public bool Equals(UserKey? other)
        {
            if (other == null)
            {
                return false;
            }

            return this.Id == other.Id;
        }
    }
}