namespace RideSharing.Pricing
{
    public interface IPricingStrategy
    {
        decimal CalculateFare(double distance, decimal baseFare);
        string GetStrategyName();
    }
}