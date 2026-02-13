namespace RideSharing.Pricing
{
    public class RushHourPricing : IPricingStrategy
    {
        public decimal CalculateFare(double distance, decimal baseFare)
        {
            return baseFare + (decimal)distance * 1.00m;
        }

        public string GetStrategyName() => "Rush Hour Pricing ($1.00/km)";
    }
}