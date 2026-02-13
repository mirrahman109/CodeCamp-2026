namespace RideSharing.Pricing
{
    public class MidnightPricing : IPricingStrategy
    {
        public decimal CalculateFare(double distance, decimal baseFare)
        {
            return baseFare + (decimal)distance * 0.75m;
        }

        public string GetStrategyName() => "Midnight Pricing ($0.75/km)";
    }
}