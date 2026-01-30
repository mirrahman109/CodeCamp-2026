namespace BusTicketingSystem;

public class Invoice
{
    public Guid InvoiceId { get; set; } = Guid.NewGuid();
    public Guid UserId { get; set; }
    public Guid TicketId { get; set; }
    public decimal Amount { get; set; }
    public DateTime CreatedDate { get; set; } = DateTime.Now;
    public bool IsPaid { get; set; } = false;
    public DateTime? PaidDate { get; set; }
}