using System;
using System.Collections.Generic;

namespace RideSharing.Vehicles
{
    public class VehicleFactory
    {
        private readonly Dictionary<string, Func<IVehicle>> _vehicleCreators;

        public VehicleFactory()
        {
            _vehicleCreators = new Dictionary<string, Func<IVehicle>>(StringComparer.OrdinalIgnoreCase)
            {
                { "bike", () => new Bike() },
                { "cng", () => new CNG() },
                { "car", () => new Car() }
            };
        }

        public void RegisterVehicle(string type, Func<IVehicle> creator)
        {
            _vehicleCreators[type.ToLower()] = creator;
        }

        public IVehicle CreateVehicle(string type)
        {
            if (_vehicleCreators.TryGetValue(type, out var creator))
            {
                return creator();
            }
            throw new ArgumentException($"Invalid vehicle type: {type}");
        }
    }
}