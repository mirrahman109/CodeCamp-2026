namespace BusTicketingSystem;

public class Ticket
{
    public Guid TicketId { get; set; } = Guid.NewGuid();
    public Guid UserId { get; set; }
    public Guid ScheduleId { get; set; }
    public List<int> SeatNumbers { get; set; } = new List<int>();
    public DateTime BookingDate { get; set; } = DateTime.Now;
    public decimal TotalPrice { get; set; }
    public bool IsPaid { get; set; } = false;


}