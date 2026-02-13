namespace RideSharing.Payments
{
    public interface IPaymentProcessor
    {
        void Pay(string paymentInfo, decimal amount);
        string GetPaymentMethod();
    }
}