using System;
using System.Collections.Generic;
using RideSharing.Users;
using RideSharing.Vehicles;
using RideSharing.Management;
using RideSharing.Rides;
using RideSharing.Pricing;
using RideSharing.Payments;
using RideSharing.Observers;

namespace RideSharing
{
    class Program
    {
        private static VehicleFactory _vehicleFactory = new VehicleFactory();
        private static RideManager _rideManager = RideManager.Instance;
        private static List<Rider> _riders = new List<Rider>();
        private static List<Ride> _rides = new List<Ride>();
        private static int _riderCounter = 1;
        private static int _driverCounter = 1;
        private static int _rideCounter = 1;

        static void Main(string[] args)
        {
            Console.WriteLine("========================================");
            Console.WriteLine("   Welcome to Ride Sharing System");
            Console.WriteLine("========================================");

            bool running = true;
            while (running)
            {
                DisplayMainMenu();
                string choice = Console.ReadLine() ?? "";

                switch (choice)
                {
                    case "1":
                        RegisterRider();
                        break;
                    case "2":
                        RegisterDriver();
                        break;
                    case "3":
                        ViewAllRiders();
                        break;
                    case "4":
                        ViewAllDrivers();
                        break;
                    case "5":
                        ViewAvailableDrivers();
                        break;
                    case "6":
                        CreateRide();
                        break;
                    case "7":
                        ViewAllRides();
                        break;
                    case "8":
                        UpdateRideStatus();
                        break;
                    case "9":
                        CalculateRideFare();
                        break;
                    case "10":
                        ProcessPayment();
                        break;
                    case "0":
                        running = false;
                        Console.WriteLine("\nThank you for using Ride Sharing System. Goodbye!");
                        break;
                    default:
                        Console.WriteLine("\nInvalid option. Please try again.");
                        break;
                }

                if (running)
                {
                    Console.WriteLine("\nPress any key to continue...");
                    Console.ReadKey();
                    Console.Clear();
                }
            }
        }

        static void DisplayMainMenu()
        {
            Console.WriteLine("\n============ Main Menu ============");
            Console.WriteLine("1.  Register Rider");
            Console.WriteLine("2.  Register Driver");
            Console.WriteLine("3.  View All Riders");
            Console.WriteLine("4.  View All Drivers");
            Console.WriteLine("5.  View Available Drivers by Vehicle Type");
            Console.WriteLine("6.  Create Ride");
            Console.WriteLine("7.  View All Rides");
            Console.WriteLine("8.  Update Ride Status");
            Console.WriteLine("9.  Calculate Ride Fare");
            Console.WriteLine("10. Process Payment");
            Console.WriteLine("0.  Exit");
            Console.WriteLine("===================================");
            Console.Write("Enter your choice: ");
        }

        static void RegisterRider()
        {
            Console.WriteLine("\n--- Register New Rider ---");
            
            Console.Write("Enter Name: ");
            string name = Console.ReadLine() ?? "";
            
            Console.Write("Enter Phone: ");
            string phone = Console.ReadLine() ?? "";
            
            Console.Write("Enter Initial Wallet Balance: $");
            if (!decimal.TryParse(Console.ReadLine(), out decimal balance))
            {
                balance = 0;
            }

            string id = $"R{_riderCounter++:D3}";
            var rider = new Rider(id, name, phone, balance);
            _riders.Add(rider);

            Console.WriteLine($"\nRider registered successfully!");
            rider.DisplayInfo();
        }

        static void RegisterDriver()
        {
            Console.WriteLine("\n--- Register New Driver ---");
            
            Console.Write("Enter Name: ");
            string name = Console.ReadLine() ?? "";
            
            Console.Write("Enter Phone: ");
            string phone = Console.ReadLine() ?? "";
            
            Console.WriteLine("Select Vehicle Type:");
            Console.WriteLine("1. Bike ($2/base fare)");
            Console.WriteLine("2. CNG ($3/base fare)");
            Console.WriteLine("3. Car ($5/base fare)");
            Console.Write("Enter choice: ");
            
            string vehicleChoice = Console.ReadLine() ?? "";
            IVehicle vehicle;
            
            try
            {
                vehicle = vehicleChoice switch
                {
                    "1" => _vehicleFactory.CreateVehicle("bike"),
                    "2" => _vehicleFactory.CreateVehicle("cng"),
                    "3" => _vehicleFactory.CreateVehicle("car"),
                    _ => throw new ArgumentException("Invalid vehicle choice")
                };
            }
            catch (ArgumentException ex)
            {
                Console.WriteLine($"Error: {ex.Message}");
                return;
            }

            string id = $"D{_driverCounter++:D3}";
            var driver = new Driver(id, name, phone, vehicle);
            _rideManager.RegisterDriver(driver);

            Console.WriteLine($"\nDriver registered successfully!");
            driver.DisplayInfo();
        }

        static void ViewAllRiders()
        {
            Console.WriteLine("\n--- All Registered Riders ---");
            
            if (_riders.Count == 0)
            {
                Console.WriteLine("No riders registered yet.");
                return;
            }

            foreach (var rider in _riders)
            {
                rider.DisplayInfo();
            }
        }

        static void ViewAllDrivers()
        {
            Console.WriteLine("\n--- All Registered Drivers ---");
            
            var drivers = _rideManager.GetAllDrivers();
            if (drivers.Count == 0)
            {
                Console.WriteLine("No drivers registered yet.");
                return;
            }

            foreach (var driver in drivers)
            {
                driver.DisplayInfo();
            }
        }

        static void ViewAvailableDrivers()
        {
            Console.WriteLine("\n--- Available Drivers by Vehicle Type ---");
            Console.WriteLine("Select Vehicle Type:");
            Console.WriteLine("1. Bike");
            Console.WriteLine("2. CNG");
            Console.WriteLine("3. Car");
            Console.Write("Enter choice: ");

            string choice = Console.ReadLine() ?? "";
            string vehicleType = choice switch
            {
                "1" => "Bike",
                "2" => "CNG",
                "3" => "Car",
                _ => ""
            };

            if (string.IsNullOrEmpty(vehicleType))
            {
                Console.WriteLine("Invalid choice.");
                return;
            }

            var availableDrivers = _rideManager.GetAvailableDrivers(vehicleType);
            
            if (availableDrivers.Count == 0)
            {
                Console.WriteLine($"No available drivers with {vehicleType}.");
                return;
            }

            Console.WriteLine($"\nAvailable {vehicleType} Drivers:");
            foreach (var driver in availableDrivers)
            {
                driver.DisplayInfo();
            }
        }

        static void CreateRide()
        {
            Console.WriteLine("\n--- Create New Ride ---");

            if (_riders.Count == 0)
            {
                Console.WriteLine("No riders available. Please register a rider first.");
                return;
            }

            var allDrivers = _rideManager.GetAllDrivers();
            if (allDrivers.Count == 0)
            {
                Console.WriteLine("No drivers available. Please register a driver first.");
                return;
            }

            // Select Rider
            Console.WriteLine("\nSelect Rider:");
            for (int i = 0; i < _riders.Count; i++)
            {
                Console.WriteLine($"{i + 1}. {_riders[i].Name} (ID: {_riders[i].ID})");
            }
            Console.Write("Enter choice: ");
            
            if (!int.TryParse(Console.ReadLine(), out int riderChoice) || 
                riderChoice < 1 || riderChoice > _riders.Count)
            {
                Console.WriteLine("Invalid rider selection.");
                return;
            }
            var selectedRider = _riders[riderChoice - 1];

            // Select Vehicle Type
            Console.WriteLine("\nSelect Vehicle Type:");
            Console.WriteLine("1. Bike");
            Console.WriteLine("2. CNG");
            Console.WriteLine("3. Car");
            Console.Write("Enter choice: ");

            string vehicleChoice = Console.ReadLine() ?? "";
            string vehicleType = vehicleChoice switch
            {
                "1" => "Bike",
                "2" => "CNG",
                "3" => "Car",
                _ => ""
            };

            if (string.IsNullOrEmpty(vehicleType))
            {
                Console.WriteLine("Invalid vehicle type.");
                return;
            }

            var availableDrivers = _rideManager.GetAvailableDrivers(vehicleType);
            if (availableDrivers.Count == 0)
            {
                Console.WriteLine($"No available {vehicleType} drivers.");
                return;
            }

            // Select Driver
            Console.WriteLine($"\nAvailable {vehicleType} Drivers:");
            for (int i = 0; i < availableDrivers.Count; i++)
            {
                Console.WriteLine($"{i + 1}. {availableDrivers[i].Name} (ID: {availableDrivers[i].ID})");
            }
            Console.Write("Enter choice: ");

            if (!int.TryParse(Console.ReadLine(), out int driverChoice) || 
                driverChoice < 1 || driverChoice > availableDrivers.Count)
            {
                Console.WriteLine("Invalid driver selection.");
                return;
            }
            var selectedDriver = availableDrivers[driverChoice - 1];

            // Enter Distance
            Console.Write("\nEnter Distance (km): ");
            if (!double.TryParse(Console.ReadLine(), out double distance) || distance <= 0)
            {
                Console.WriteLine("Invalid distance.");
                return;
            }

            // Create Ride
            string rideId = $"RIDE{_rideCounter++:D3}";
            var ride = new Ride(rideId, selectedRider, selectedDriver, distance);

            // Add Observers
            ride.AddObserver(new RiderNotifier(selectedRider.Name, selectedRider.Phone));
            ride.AddObserver(new DriverNotifier(selectedDriver.Name, selectedDriver.Phone));

            // Set driver unavailable
            selectedDriver.IsAvailable = false;

            _rides.Add(ride);

            Console.WriteLine($"\nRide created successfully!");
            Console.WriteLine($"Ride ID: {rideId}");
            Console.WriteLine($"Rider: {selectedRider.Name}");
            Console.WriteLine($"Driver: {selectedDriver.Name}");
            Console.WriteLine($"Vehicle: {selectedDriver.Vehicle.GetVehicleType()}");
            Console.WriteLine($"Distance: {distance} km");
            Console.WriteLine($"Status: {ride.Status}");
        }

        static void ViewAllRides()
        {
            Console.WriteLine("\n--- All Rides ---");

            if (_rides.Count == 0)
            {
                Console.WriteLine("No rides created yet.");
                return;
            }

            foreach (var ride in _rides)
            {
                Console.WriteLine($"\nRide ID: {ride.ID}");
                Console.WriteLine($"  Rider: {ride.Rider.Name}");
                Console.WriteLine($"  Driver: {ride.Driver.Name}");
                Console.WriteLine($"  Vehicle: {ride.Driver.Vehicle.GetVehicleType()}");
                Console.WriteLine($"  Distance: {ride.Distance} km");
                Console.WriteLine($"  Status: {ride.Status}");
            }
        }

        static void UpdateRideStatus()
        {
            Console.WriteLine("\n--- Update Ride Status ---");

            if (_rides.Count == 0)
            {
                Console.WriteLine("No rides available.");
                return;
            }

            // Select Ride
            Console.WriteLine("\nSelect Ride:");
            for (int i = 0; i < _rides.Count; i++)
            {
                Console.WriteLine($"{i + 1}. {_rides[i].ID} - {_rides[i].Rider.Name} to {_rides[i].Driver.Name} ({_rides[i].Status})");
            }
            Console.Write("Enter choice: ");

            if (!int.TryParse(Console.ReadLine(), out int rideChoice) || 
                rideChoice < 1 || rideChoice > _rides.Count)
            {
                Console.WriteLine("Invalid ride selection.");
                return;
            }
            var selectedRide = _rides[rideChoice - 1];

            // Select New Status
            Console.WriteLine("\nSelect New Status:");
            Console.WriteLine("1. Requested");
            Console.WriteLine("2. Accepted");
            Console.WriteLine("3. In Progress");
            Console.WriteLine("4. Completed");
            Console.Write("Enter choice: ");

            string statusChoice = Console.ReadLine() ?? "";
            RideStatus newStatus;

            switch (statusChoice)
            {
                case "1":
                    newStatus = RideStatus.Requested;
                    break;
                case "2":
                    newStatus = RideStatus.Accepted;
                    break;
                case "3":
                    newStatus = RideStatus.InProgress;
                    break;
                case "4":
                    newStatus = RideStatus.Completed;
                    selectedRide.Driver.IsAvailable = true; // Make driver available again
                    break;
                default:
                    Console.WriteLine("Invalid status selection.");
                    return;
            }

            selectedRide.SetStatus(newStatus);
            Console.WriteLine($"\nRide status updated to: {newStatus}");
        }

        static void CalculateRideFare()
        {
            Console.WriteLine("\n--- Calculate Ride Fare ---");

            if (_rides.Count == 0)
            {
                Console.WriteLine("No rides available.");
                return;
            }

            // Select Ride
            Console.WriteLine("\nSelect Ride:");
            for (int i = 0; i < _rides.Count; i++)
            {
                Console.WriteLine($"{i + 1}. {_rides[i].ID} - {_rides[i].Distance} km ({_rides[i].Driver.Vehicle.GetVehicleType()})");
            }
            Console.Write("Enter choice: ");

            if (!int.TryParse(Console.ReadLine(), out int rideChoice) || 
                rideChoice < 1 || rideChoice > _rides.Count)
            {
                Console.WriteLine("Invalid ride selection.");
                return;
            }
            var selectedRide = _rides[rideChoice - 1];

            // Select Pricing Strategy
            Console.WriteLine("\nSelect Pricing Strategy:");
            Console.WriteLine("1. Standard ($0.50/km)");
            Console.WriteLine("2. Rush Hour ($1.00/km)");
            Console.WriteLine("3. Midnight ($0.75/km)");
            Console.Write("Enter choice: ");

            string strategyChoice = Console.ReadLine() ?? "";
            IPricingStrategy strategy;

            switch (strategyChoice)
            {
                case "1":
                    strategy = new StandardPricing();
                    break;
                case "2":
                    strategy = new RushHourPricing();
                    break;
                case "3":
                    strategy = new MidnightPricing();
                    break;
                default:
                    Console.WriteLine("Invalid strategy selection.");
                    return;
            }

            selectedRide.SetPricingStrategy(strategy);
            decimal fare = selectedRide.CalculateFare();

            Console.WriteLine($"\n--- Fare Calculation ---");
            Console.WriteLine($"Ride ID: {selectedRide.ID}");
            Console.WriteLine($"Distance: {selectedRide.Distance} km");
            Console.WriteLine($"Vehicle: {selectedRide.Driver.Vehicle.GetVehicleType()}");
            Console.WriteLine($"Base Fare: ${selectedRide.Driver.Vehicle.GetBaseFare()}");
            Console.WriteLine($"Pricing Strategy: {strategy.GetStrategyName()}");
            Console.WriteLine($"Total Fare: ${fare:F2}");
        }

        static void ProcessPayment()
        {
            Console.WriteLine("\n--- Process Payment ---");

            if (_rides.Count == 0)
            {
                Console.WriteLine("No rides available.");
                return;
            }

            // Select Ride
            Console.WriteLine("\nSelect Ride:");
            for (int i = 0; i < _rides.Count; i++)
            {
                Console.WriteLine($"{i + 1}. {_rides[i].ID} - {_rides[i].Rider.Name} ({_rides[i].Status})");
            }
            Console.Write("Enter choice: ");

            if (!int.TryParse(Console.ReadLine(), out int rideChoice) || 
                rideChoice < 1 || rideChoice > _rides.Count)
            {
                Console.WriteLine("Invalid ride selection.");
                return;
            }
            var selectedRide = _rides[rideChoice - 1];

            // Calculate fare
            decimal fare = selectedRide.CalculateFare();
            Console.WriteLine($"\nTotal Fare: ${fare:F2}");

            // Select Payment Method
            Console.WriteLine("\nSelect Payment Method:");
            Console.WriteLine("1. bKash");
            Console.WriteLine("2. Credit Card");
            Console.Write("Enter choice: ");

            string paymentChoice = Console.ReadLine() ?? "";
            IPaymentProcessor processor;

            switch (paymentChoice)
            {
                case "1":
                    processor = new BkashPaymentAdapter();
                    break;
                case "2":
                    processor = new CreditCardProcessor();
                    break;
                default:
                    Console.WriteLine("Invalid payment method.");
                    return;
            }

            Console.Write("\nEnter Payment Info (account/card number): ");
            string paymentInfo = Console.ReadLine() ?? "";

            Console.WriteLine($"\nProcessing payment via {processor.GetPaymentMethod()}...");
            processor.Pay(paymentInfo, fare);
            Console.WriteLine("Payment processed successfully!");
        }
    }
}