using System;
using RideSharing.Vehicles;

namespace RideSharing.Users
{
    public class Driver : User
    {
        public IVehicle Vehicle { get; set; }
        public bool IsAvailable { get; set; }

        public Driver(string id, string name, string phone, IVehicle vehicle)
            : base(id, name, phone)
        {
            Vehicle = vehicle;
            IsAvailable = true;
        }

        public override void DisplayInfo()
        {
            Console.WriteLine($"Driver ID: {ID}, Name: {Name}, Phone: {Phone}, Vehicle: {Vehicle.GetVehicleType()}, Available: {IsAvailable}");
        }

        public override string GetRole()
        {
            return "Driver";
        }
    }
}