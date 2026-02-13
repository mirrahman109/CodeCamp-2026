using System;
using System.Collections.Generic;
using RideSharing.Users;
using RideSharing.Pricing;
using RideSharing.Observers;

namespace RideSharing.Rides
{
    public enum RideStatus
    {
        Requested,
        Accepted,
        InProgress,
        Completed
    }

    public class Ride
    {
        public string ID { get; set; }
        public Rider Rider { get; set; }
        public Driver Driver { get; set; }
        public double Distance { get; set; }
        public RideStatus Status { get; private set; }
        
        private IPricingStrategy _pricingStrategy;
        private List<IRideObserver> _observers;

        public Ride(string id, Rider rider, Driver driver, double distance)
        {
            ID = id;
            Rider = rider;
            Driver = driver;
            Distance = distance;
            Status = RideStatus.Requested;
            _pricingStrategy = new StandardPricing();
            _observers = new List<IRideObserver>();
        }

        public void SetPricingStrategy(IPricingStrategy strategy)
        {
            _pricingStrategy = strategy;
        }

        public decimal CalculateFare()
        {
            decimal baseFare = Driver.Vehicle.GetBaseFare();
            return _pricingStrategy.CalculateFare(Distance, baseFare);
        }

        public void AddObserver(IRideObserver observer)
        {
            _observers.Add(observer);
        }

        public void RemoveObserver(IRideObserver observer)
        {
            _observers.Remove(observer);
        }

        public void SetStatus(RideStatus status)
        {
            Status = status;
            NotifyObservers();
        }

        private void NotifyObservers()
        {
            foreach (var observer in _observers)
            {
                observer.Update(ID, Status.ToString());
            }
        }
    }
}