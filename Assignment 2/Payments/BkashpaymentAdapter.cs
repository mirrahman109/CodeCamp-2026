namespace RideSharing.Payments
{
    public class BkashPaymentAdapter : IPaymentProcessor
    {
        private BkashPaymentGateway _gateway;

        public BkashPaymentAdapter()
        {
            _gateway = new BkashPaymentGateway();
        }

        public void Pay(string paymentInfo, decimal amount)
        {
            _gateway.MakePayment(paymentInfo, amount);
        }

        public string GetPaymentMethod() => "bKash";
    }
}