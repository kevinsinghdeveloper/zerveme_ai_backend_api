namespace Core.Contracts.ContractModels;

public class FilterCondition
{
    public required string Col { get; set; }
    public required string Op { get; set; }
    public required List<string> Val { get; set; }
}