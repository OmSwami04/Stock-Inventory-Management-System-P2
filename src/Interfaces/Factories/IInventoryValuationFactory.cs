namespace InventoryManagement.Interfaces.Factories;

public enum ValuationMethod
{
    FIFO,
    LIFO,
    WeightedAverage
}

public interface IInventoryValuationStrategy
{
    Task<decimal> CalculateValuationAsync(Guid productId);
}

public interface IInventoryValuationFactory
{
    IInventoryValuationStrategy GetStrategy(ValuationMethod method);
}
