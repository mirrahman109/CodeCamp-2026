namespace BusTicketingSystem;

public class TicketService
{
    private List<Ticket> _tickets = new List<Ticket>();

    public void AddTicket(Ticket ticket)
    {
        _tickets.Add(ticket);
    }

    public List<Ticket> GetAllTickets()
    {
        return _tickets;
    }

    public Ticket? GetTicketById(Guid ticketId)
    {
        return _tickets.FirstOrDefault(t => t.TicketId == ticketId);
    }

    public List<Ticket> GetTicketsByUserId(Guid userId)
    {
        return _tickets.Where(t => t.UserId == userId).ToList();
    }

    public List<Ticket> GetTicketsByScheduleId(Guid scheduleId)
    {
        return _tickets.Where(t => t.ScheduleId == scheduleId).ToList();
    }
}