using System;

namespace RideSharing.Observers
{
    public class RiderNotifier : IRideObserver
    {
        private string _riderName;
        private string _riderPhone;

        public RiderNotifier(string riderName, string riderPhone)
        {
            _riderName = riderName;
            _riderPhone = riderPhone;
        }

        public void Update(string rideId, string status)
        {
            Console.WriteLine($"[SMS to {_riderName} ({_riderPhone})]: Your ride {rideId} status changed to: {status}");
        }
    }
}