namespace BusTicketingSystem;

public static class BusOperations
{
  public static void CreateBus(BusService busService)
    {
        
        Console.WriteLine("Enter CoachNumber:");
        string coachNumber = Console.ReadLine() ?? string.Empty;

        Console.WriteLine("Enter Bus Type:");
        string busType = Console.ReadLine() ?? string.Empty;

        int numberOfSeats = busType.ToLower() == "economic" ? 40 : 30;

        Bus newBus = new Bus
        {
            CoachNumber = coachNumber,
            BusType = busType,
            NumberOfSeats = numberOfSeats
        };

        busService.AddBus(newBus);
        Console.WriteLine("Bus created successfully!");
    }
    public static void ShowBuses(BusService busService)
    {
        Console.WriteLine("List of Buses:");
        busService.ShowBuses();
    }

}