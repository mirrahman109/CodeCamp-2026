using System;

namespace RideSharing.Payments
{
    public class CreditCardProcessor : IPaymentProcessor
    {
        public void Pay(string paymentInfo, decimal amount)
        {
            Console.WriteLine($"[Credit Card]: Payment of ${amount:F2} charged to card {paymentInfo} completed.");
        }

        public string GetPaymentMethod() => "Credit Card";
    }
}