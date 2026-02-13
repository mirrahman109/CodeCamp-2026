using System;

namespace RideSharing.Vehicles
{
    public class Bike : IVehicle
    {
        public string GetVehicleType()
        {
            return "Bike";
        }

        public decimal GetBaseFare()
        {
            return 20.00m; // Base fare for bike rides
        }
    }
}