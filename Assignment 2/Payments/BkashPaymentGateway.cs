using System;

namespace RideSharing.Payments
{
    // Simulated external bKash payment gateway
    public class BkashPaymentGateway
    {
        public void MakePayment(string accountNumber, decimal amount)
        {
            Console.WriteLine($"[bKash Gateway]: Payment of ${amount:F2} from account {accountNumber} completed.");
        }
    }
}