
public class Bus
{
  public Guid BusId{ get; set; } = Guid.NewGuid();

  public string CoachNumber{ get; set; } = string.Empty;
  public string BusType{ get; set; } = string.Empty;

  public int NumberOfSeats{ get; set; } 

}