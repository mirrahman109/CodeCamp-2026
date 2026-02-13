using System;
using System.Collections.Generic;
using RideSharing.Users;

namespace RideSharing.Management
{
    public sealed class RideManager
    {
        private static RideManager? _instance;
        private static readonly object _lock = new object();
        private List<Driver> _drivers;

        private RideManager()
        {
            _drivers = new List<Driver>();
        }

        public static RideManager Instance
        {
            get
            {
                lock (_lock)
                {
                    if (_instance == null)
                    {
                        _instance = new RideManager();
                    }
                    return _instance;
                }
            }
        }

        public void RegisterDriver(Driver driver)
        {
            _drivers.Add(driver);
        }

        public List<Driver> GetAllDrivers()
        {
            return _drivers;
        }

        public List<Driver> GetAvailableDrivers(string vehicleType)
        {
            List<Driver> availableDrivers = new List<Driver>();
            foreach (var driver in _drivers)
            {
                if (driver.IsAvailable && 
                    driver.Vehicle.GetVehicleType().Equals(vehicleType, StringComparison.OrdinalIgnoreCase))
                {
                    availableDrivers.Add(driver);
                }
            }
            return availableDrivers;
        }
    }
}