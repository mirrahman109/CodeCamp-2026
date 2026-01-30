
public class Schedule

{ public Guid ScheduleId{ get; set; } = Guid.NewGuid();

  public Guid BusId{ get; set; }
  public string DepartureCity{ get; set; } = string.Empty;

  public string ArrivalCity{ get; set; } = string.Empty;

  public DateTime DepartureTime{ get; set; } 

  public int TicketPrice{ get; set; }

  public int AvailableSeats { get; set; }

// Held seats (not paid yet) - shows 'H'
  public List<int> HeldSeats { get; set; } = new List<int>();
    
// Booked seats (paid) - shows 'X'
  public List<int> BookedSeats { get; set; } = new List<int>();
  
}