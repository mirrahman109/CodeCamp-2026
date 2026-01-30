namespace BusTicketingSystem;

public class BusService
{
    private readonly List<Bus>_buses = new List<Bus>();
    public void AddBus(Bus bus)
    {
        _buses.Add(bus);
    }
    public List<Bus> GetAllBuses()
    {
        return _buses;
    }

    public Bus? GetBusById(Guid busId)
    {
        return _buses.FirstOrDefault(b => b.BusId == busId);
    }

    public Bus? GetBusByCoachNumber(string coachNumber)
    {
        return _buses.FirstOrDefault(b => b.CoachNumber == coachNumber);
    }

    public void ShowBuses()
    {
        foreach (var bus in _buses) 
        {
            Console.WriteLine($"CoachNumber: {bus.CoachNumber}, BusType: {bus.BusType}, NumberOfSeats: {bus.NumberOfSeats}");
        }
    }
}