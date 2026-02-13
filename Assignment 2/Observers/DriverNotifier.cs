using System;

namespace RideSharing.Observers
{
    public class DriverNotifier : IRideObserver
    {
        private string _driverName;
        private string _driverPhone;

        public DriverNotifier(string driverName, string driverPhone)
        {
            _driverName = driverName;
            _driverPhone = driverPhone;
        }

        public void Update(string rideId, string status)
        {
            Console.WriteLine($"[App Notification to {_driverName} ({_driverPhone})]: Ride {rideId} status changed to: {status}");
        }
    }
}