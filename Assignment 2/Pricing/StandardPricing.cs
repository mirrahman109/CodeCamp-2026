namespace RideSharing.Pricing
{
    public class StandardPricing : IPricingStrategy
    {
        public decimal CalculateFare(double distance, decimal baseFare)
        {
            return baseFare + (decimal)distance * 0.50m;
        }

        public string GetStrategyName() => "Standard Pricing ($0.50/km)";
    }
}