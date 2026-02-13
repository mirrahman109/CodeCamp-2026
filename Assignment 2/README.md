# Ride Sharing System

A console-based ride-sharing application built with C# demonstrating Object-Oriented Programming principles and Design Patterns.

## Table of Contents

- [How to Run the Application](#how-to-run-the-application)
- [Project Structure](#project-structure)
- [Design Patterns Used](#design-patterns-used)
- [Features](#features)
- [Usage Guide](#usage-guide)

---

## How to Run the Application

### Prerequisites

- [.NET 8.0 SDK](https://dotnet.microsoft.com/download/dotnet/8.0) or later
- Visual Studio Code or Visual Studio 2022

### Steps to Run

1. **Clone or download** the project to your local machine

2. **Open terminal** in the project directory:
   ```bash
   cd "d:\Projects\CodeCamp\Assignment 2"
   ```

3. **Restore dependencies**:
   ```bash
   dotnet restore
   ```

4. **Build the project**:
   ```bash
   dotnet build
   ```

5. **Run the application**:
   ```bash
   dotnet run
   ```

### Alternative: Run in Visual Studio Code

1. Open the folder in VS Code
2. Press `Ctrl + F5` to run without debugging
3. Or press `F5` to run with debugging

---

## Project Structure

```
RideSharing/
├── Program.cs                      # Main entry point with menu-driven console
├── Users/
│   ├── User.cs                     # Abstract base class for users
│   ├── Rider.cs                    # Rider implementation
│   └── Driver.cs                   # Driver implementation
├── Vehicles/
│   ├── IVehicle.cs                 # Vehicle interface
│   ├── Bike.cs                     # Bike implementation ($2 base fare)
│   ├── CNG.cs                      # CNG implementation ($3 base fare)
│   ├── Car.cs                      # Car implementation ($5 base fare)
│   └── VehicleFactory.cs           # Factory for creating vehicles
├── Management/
│   └── RideManager.cs              # Singleton manager for drivers
├── Rides/
│   └── Ride.cs                     # Ride entity with status management
├── Pricing/
│   ├── IPricingStrategy.cs         # Pricing strategy interface
│   ├── StandardPricing.cs          # Standard pricing ($0.50/km)
│   ├── RushHourPricing.cs          # Rush hour pricing ($1.00/km)
│   └── MidnightPricing.cs          # Midnight pricing ($0.75/km)
├── Payments/
│   ├── IPaymentProcessor.cs        # Payment processor interface
│   ├── BkashPaymentGateway.cs      # External bKash gateway (simulated)
│   ├── BkashPaymentAdapter.cs      # Adapter for bKash integration
│   └── CreditCardProcessor.cs      # Credit card processor
├── Observers/
│   ├── IRideObserver.cs            # Observer interface
│   ├── RiderNotifier.cs            # SMS notification for riders
│   └── DriverNotifier.cs           # App notification for drivers
└── RideSharing.csproj              # Project configuration
```

---

## Design Patterns Used

### 1. Factory Pattern

**Location:** `Vehicles/VehicleFactory.cs`

**Purpose:** Creates different types of vehicles without exposing creation logic.

**Implementation:**
```csharp
public class VehicleFactory
{
    public IVehicle CreateVehicle(string type)
    {
        return type.ToLower() switch
        {
            "bike" => new Bike(),
            "cng" => new CNG(),
            "car" => new Car(),
            _ => throw new ArgumentException("Invalid vehicle type")
        };
    }
}
```

**Usage:**
```csharp
var factory = new VehicleFactory();
IVehicle bike = factory.CreateVehicle("bike");
IVehicle car = factory.CreateVehicle("car");
```

---

### 2. Singleton Pattern

**Location:** `Management/RideManager.cs`

**Purpose:** Ensures only one instance of RideManager exists throughout the application.

**Implementation:**
```csharp
public sealed class RideManager
{
    private static RideManager? _instance;
    private static readonly object _lock = new object();

    private RideManager() { }

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
}
```

**Usage:**
```csharp
var manager = RideManager.Instance;
manager.RegisterDriver(driver);
```

---

### 3. Strategy Pattern

**Location:** `Pricing/` folder

**Purpose:** Allows dynamic selection of pricing algorithms at runtime.

**Components:**
| File | Description |
|------|-------------|
| `IPricingStrategy.cs` | Interface defining pricing contract |
| `StandardPricing.cs` | Standard rate: $0.50/km |
| `RushHourPricing.cs` | Rush hour rate: $1.00/km |
| `MidnightPricing.cs` | Midnight rate: $0.75/km |

**Implementation:**
```csharp
public interface IPricingStrategy
{
    decimal CalculateFare(double distance, decimal baseFare);
    string GetStrategyName();
}

public class RushHourPricing : IPricingStrategy
{
    public decimal CalculateFare(double distance, decimal baseFare)
    {
        return baseFare + (decimal)distance * 1.00m;
    }
    public string GetStrategyName() => "Rush Hour Pricing";
}
```

**Usage:**
```csharp
ride.SetPricingStrategy(new RushHourPricing());
decimal fare = ride.CalculateFare();
```

---

### 4. Adapter Pattern

**Location:** `Payments/BkashPaymentAdapter.cs`

**Purpose:** Integrates external bKash payment gateway with our payment system.

**Components:**
| File | Description |
|------|-------------|
| `IPaymentProcessor.cs` | Common payment interface |
| `BkashPaymentGateway.cs` | External gateway (incompatible interface) |
| `BkashPaymentAdapter.cs` | Adapter to make bKash compatible |
| `CreditCardProcessor.cs` | Direct implementation |

**Implementation:**
```csharp
// External gateway with different method signature
public class BkashPaymentGateway
{
    public void MakePayment(string accountNumber, decimal amount) { }
}

// Adapter makes it compatible with IPaymentProcessor
public class BkashPaymentAdapter : IPaymentProcessor
{
    private BkashPaymentGateway _gateway = new BkashPaymentGateway();

    public void Pay(string paymentInfo, decimal amount)
    {
        _gateway.MakePayment(paymentInfo, amount);
    }
    public string GetPaymentMethod() => "bKash";
}
```

**Usage:**
```csharp
IPaymentProcessor processor = new BkashPaymentAdapter();
processor.Pay("01712345678", 150.00m);
```

---

### 5. Observer Pattern

**Location:** `Observers/` folder

**Purpose:** Notifies riders and drivers when ride status changes.

**Components:**
| File | Description |
|------|-------------|
| `IRideObserver.cs` | Observer interface |
| `RiderNotifier.cs` | Sends SMS to riders |
| `DriverNotifier.cs` | Sends app notification to drivers |

**Implementation:**
```csharp
public interface IRideObserver
{
    void Update(string rideId, string status);
}

public class RiderNotifier : IRideObserver
{
    public void Update(string rideId, string status)
    {
        Console.WriteLine($"[SMS]: Ride {rideId} is now {status}");
    }
}
```

**Usage:**
```csharp
ride.AddObserver(new RiderNotifier(rider.Name, rider.Phone));
ride.AddObserver(new DriverNotifier(driver.Name, driver.Phone));
ride.SetStatus(RideStatus.Accepted); // Both observers notified
```

---

### 6. Inheritance & Polymorphism

**Location:** `Users/` folder

**Purpose:** Creates a user hierarchy with shared properties and specialized behavior.

**Components:**
| File | Description |
|------|-------------|
| `User.cs` | Abstract base class |
| `Rider.cs` | Extends User with WalletBalance |
| `Driver.cs` | Extends User with Vehicle, IsAvailable |

**Implementation:**
```csharp
public abstract class User
{
    public string ID { get; set; }
    public string Name { get; set; }
    public string Phone { get; set; }

    public abstract void DisplayInfo();
    public abstract string GetRole();
}

public class Rider : User
{
    public decimal WalletBalance { get; set; }
    
    public override void DisplayInfo() { /* Rider-specific display */ }
    public override string GetRole() => "Rider";
}
```

---

## Features

| Feature | Description |
|---------|-------------|
| **User Management** | Register riders and drivers |
| **Vehicle Types** | Bike, CNG, Car with different base fares |
| **Ride Creation** | Match riders with available drivers |
| **Dynamic Pricing** | Standard, Rush Hour, Midnight pricing |
| **Payment Processing** | bKash and Credit Card support |
| **Notifications** | SMS for riders, App notifications for drivers |
| **Status Tracking** | Requested → Accepted → In Progress → Completed |

---

## Usage Guide

### Main Menu Options

```
============ Main Menu ============
1.  Register Rider
2.  Register Driver
3.  View All Riders
4.  View All Drivers
5.  View Available Drivers by Vehicle Type
6.  Create Ride
7.  View All Rides
8.  Update Ride Status
9.  Calculate Ride Fare
10. Process Payment
0.  Exit
===================================
```

### Sample Workflow

1. **Register a Rider** (Option 1)
   - Enter name, phone, and wallet balance

2. **Register a Driver** (Option 2)
   - Enter name, phone, and select vehicle type

3. **Create a Ride** (Option 6)
   - Select rider, vehicle type, driver, and distance

4. **Update Ride Status** (Option 8)
   - Change status and observe notifications

5. **Calculate Fare** (Option 9)
   - Select pricing strategy and view fare breakdown

6. **Process Payment** (Option 10)
   - Choose bKash or Credit Card

---

## Pricing Formula

```
Total Fare = Base Fare + (Distance × Rate per km)
```

| Vehicle | Base Fare |
|---------|-----------|
| Bike    | $2.00     |
| CNG     | $3.00     |
| Car     | $5.00     |

| Strategy | Rate per km |
|----------|-------------|
| Standard | $0.50       |
| Rush Hour| $1.00       |
| Midnight | $0.75       |

**Example:** Car ride, 10 km, Rush Hour
```
Fare = $5.00 + (10 × $1.00) = $15.00
```

---

## Author

Assignment 2 - Code Camp

## License

This project is for educational purposes.