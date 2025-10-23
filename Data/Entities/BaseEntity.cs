namespace Data.Entities;

public interface IBaseEntity
{
    public DateTime? Updated { get; set; }

    public DateTime Created { get; set; }

    public DateTime? Deleted { get; set; }
}